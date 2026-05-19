using EZAccess.SourceGenerator.Builders;
using EZAccess.SourceGenerator.DatabaseDefinition;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace EZAccess.SourceGenerator.Generators;

[Generator]
public class DatabaseDefReader : IIncrementalGenerator
{
	private static readonly DiagnosticDescriptor _emptyFileRule = new (
		id: "EZ001",
		title: "Empty DatabaseDef.json file",
		messageFormat: "The DatabaseDef.json file at '{0}' is empty or contains only whitespace",
		category: "CodeGenerator",
		DiagnosticSeverity.Error,
		isEnabledByDefault: true
		);

	private static readonly DiagnosticDescriptor _jsonParseError = new (
		id: "EZ002",
		title: "invalid JSON in DatabaseDef.json",
		messageFormat: "Error parsing JSON: {0}",
		category: "CodeGenerator",
		DiagnosticSeverity.Error,
		isEnabledByDefault: true
		);

	private static readonly DiagnosticDescriptor _missingRequiredProperty = new(
		id: "EZ003",
		title: "Missing required property",
		messageFormat: "DatabaseDef.json is missing required property: {0}",
		category: "CodeGenerator",
		DiagnosticSeverity.Error,
		isEnabledByDefault: true);

	private static readonly DiagnosticDescriptor _emptyList = new(
		id: "EZ004",
		title: "Empty List",
		messageFormat: "The list '{0}' in DatabaseDef.json is empty",
		category: "CodeGenerator",
		DiagnosticSeverity.Warning,
		isEnabledByDefault: true
		);

	private static readonly DiagnosticDescriptor _generatorError = new(
		id: "EZ005",
		title: "Generator error",
		messageFormat: "Error generating source: {0}",
		category: "CodeGenerator",
		DiagnosticSeverity.Error,
		isEnabledByDefault: true);

	private class GeneratorSettings
	{
		public bool GenerateDataServicesForBFF { get; }
		public bool GenerateModels { get; }

		public GeneratorSettings(bool generateDataServicesForBFF, bool generateModels)
		{
			GenerateDataServicesForBFF = generateDataServicesForBFF;
			GenerateModels = generateModels;
		}
	}

	public void Initialize(IncrementalGeneratorInitializationContext context)
	{

		// Read MSBuild properties
		var generateDataServices = context.AnalyzerConfigOptionsProvider
			.Select((provider, _) => {
				provider.GlobalOptions.TryGetValue("build_property.GenerateDataServicesForBFF", out var value);
				provider.GlobalOptions.TryGetValue("build_property.GenerateModels", out var generateModels);
				
				return new GeneratorSettings(
					generateDataServicesForBFF: value?.Equals("true", StringComparison.OrdinalIgnoreCase) ?? false,
					generateModels: generateModels?.Equals("true", StringComparison.OrdinalIgnoreCase) ?? false
				);
			});

		var assemblyName = context.CompilationProvider
			.Select((compilation, _) => compilation.AssemblyName);

		// Read the DatabaseDef.json file(s) from AdditionalFiles, filter for those that end with "DatabaseDef.json", and combine with the compilation
		var databaseDefFiles = context.AdditionalTextsProvider
			.Where(file => file.Path.EndsWith("DatabaseDef.json", StringComparison.OrdinalIgnoreCase))
			.Select((file, ct) => (Text: file.GetText(ct)?.ToString(), file.Path));

		// Combine the compilation and the DatabaseDef.json files into a single source output
		var combined =
			assemblyName
			.Combine(databaseDefFiles.Collect())
			.Combine(generateDataServices);

		// Register a source output that processes the combined compilation and DatabaseDef.json files
		context.RegisterSourceOutput(combined, (spc, source) => {
			var assemblyName = source.Left.Left;
			var files = source.Left.Right;
			var settings = source.Right;

			if (files.Length == 0) return; // No DatabaseDef.json file found, so we can skip processing
			var (Text, Path) = files[0]; // We only process the first DatabaseDef.json file found, ignoring any others

			if (string.IsNullOrWhiteSpace(Text)) {
				spc.ReportDiagnostic(Diagnostic.Create(_emptyFileRule, Location.None, Path));
				return;
			}

			string errorMessage = string.Empty;

			DatabaseDef? databaseDef = null;
			try {
				databaseDef = JsonConvert.DeserializeObject<DatabaseDef>(Text!); // text is not null or whitespace due to the earlier check, so we can safely use the null-forgiving operator here
			}
			catch (Exception ex) {
				spc.ReportDiagnostic(Diagnostic.Create(_jsonParseError, Location.None, ex.Message));
				return;
			}

			if (databaseDef is null) {
				spc.ReportDiagnostic(Diagnostic.Create(_jsonParseError, Location.None, "Deserialization resulted in null"));
				return;
			}

			if (databaseDef.DatabaseName is null) {
				spc.ReportDiagnostic(Diagnostic.Create(_missingRequiredProperty, Location.None, "DatabaseName"));
				return;
			}

			if (databaseDef.Tables is null) {
				spc.ReportDiagnostic(Diagnostic.Create(_missingRequiredProperty, Location.None, "Tables"));
				return;
			}

			if (databaseDef.Tables.Count == 0) {
				spc.ReportDiagnostic(Diagnostic.Create(_emptyList, Location.None, "Tables"));
				return;
			}

			var rootNamespace = assemblyName ?? "Generated";

			try {
				// Generate Services for BFF if the setting is enabled. This includes a DataServicesExtensions class and individual service classes for each table that is not marked as AccessTableOnly.
				if (settings.GenerateDataServicesForBFF) {
					spc.AddSource("DataServicesExtensions.g.cs", DataServicesExtensionsBuilder.Build(databaseDef.Tables, databaseDef.Views, rootNamespace));

					var tables = databaseDef.Tables.Where(t => !t.AccessTableOnly).ToList();

					foreach (var table in tables) {
						var code = BFFServiceClassBuilder.Build(table, rootNamespace);
						spc.AddSource($"{table.EntityNameSingular}Service2.g.cs", code);
					}

					var views = databaseDef.Views.Where(t => !t.AccessViewOnly).ToList();

					foreach (var view in views) {
						var code = BFFServiceROClassBuilder.Build(view, rootNamespace);
						spc.AddSource($"{view.EntityNameSingular}Service2.g.cs", code);
					}
				}
			}
			catch (Exception ex) {
				spc.ReportDiagnostic(Diagnostic.Create(_generatorError, Location.None, ex.Message));
			}
		});
	}
}

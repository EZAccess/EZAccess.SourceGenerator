using CodeGenerator.Builders;
using CodeGenerator.DatabaseDefinition;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using System;

namespace CodeGenerator;

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

	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		// Read MSBuild properties
		var generateDataServices = context.AnalyzerConfigOptionsProvider
			.Select((provider, _) => {
				provider.GlobalOptions.TryGetValue("build_property.GenerateDataServicesForBFF", out var value);
				return value?.Equals("true", StringComparison.OrdinalIgnoreCase) ?? false;
			});

		// Read the DatabaseDef.json file(s) from AdditionalFiles, filter for those that end with "DatabaseDef.json", and combine with the compilation
		var databaseDefFiles = context.AdditionalTextsProvider
			.Where(file => file.Path.EndsWith("DatabaseDef.json", StringComparison.OrdinalIgnoreCase))
			.Select((file, ct) => (Text: file.GetText(ct)?.ToString(), file.Path));

		// Combine the compilation and the DatabaseDef.json files into a single source output
		var combined = 
			context.CompilationProvider
			.Combine(databaseDefFiles.Collect())
			.Combine(generateDataServices);

		// Register a source output that processes the combined compilation and DatabaseDef.json files
		context.RegisterSourceOutput(combined, (spc, source) => {
			var compilation = source.Left.Left;
			var files = source.Left.Right;
			var shouldGenerate = source.Right;

			//// Debug diagnostic to see what's happening
			//var debugDiagnostic = new DiagnosticDescriptor(
			//	id: "EZ999",
			//	title: "Generator Debug Info",
			//	messageFormat: "GenerateDataServicesForBFF={0}, ShouldGenerate={1}",
			//	category: "CodeGenerator",
			//	DiagnosticSeverity.Info,
			//	isEnabledByDefault: true);
			//spc.ReportDiagnostic(Diagnostic.Create(debugDiagnostic, Location.None, shouldGenerate, shouldGenerate));

			if (!shouldGenerate) return; // If the user has not opted in to generating data services, we can skip processing entirely

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

			var rootNamespace = compilation.AssemblyName ?? "Generated";

			var dataServicesExtensions = 
				new DataServicesExtensionsBuilder(databaseDef.Tables, databaseDef.Views, rootNamespace);
			spc.AddSource("DataServicesExtensions.g.cs", dataServicesExtensions.Build());
		});
	}
}

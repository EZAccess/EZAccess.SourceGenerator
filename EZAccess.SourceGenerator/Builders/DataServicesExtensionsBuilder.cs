using CodeGenerator.DatabaseDefinition;
using System.Collections.Generic;
using System.Linq;

namespace CodeGenerator.Builders;

internal class DataServicesExtensionsBuilder : CodeBuilderBase
{
	private readonly IEnumerable<TableDef> _tables;
	private readonly IEnumerable<ViewDef> _views;

	public DataServicesExtensionsBuilder(IEnumerable<TableDef> tables, IEnumerable<ViewDef>? views, string namespaceName) : base(namespaceName)
	{
		_tables = tables;
		_views = views ?? [];
	}

	internal string Build()
	{
		WriteDisclaimer();
		AppendLine("using Microsoft.Extensions.DependencyInjection;");
		BreakLine();
		AppendLine($"namespace {_namespaceName}.Services;");
		BreakLine();
		StartBlock("public static class DataServicesExtensions");
		{
			StartSummary("Generated extension method to add all data repositories to the service factory");
			EndSummary();
			StartBlock("public static void AddEZGeneratedDataServices(this IServiceCollection services)");
			{
				WriteComment("Add services for tables. Those are writable");
				var clientSideTables = _tables.Where(t => !t.AccessTableOnly && !string.IsNullOrEmpty(t.EntityNameSingular)).OrderBy(t => t.EntityNameSingular);
				foreach (var table in clientSideTables) {
					AppendLine($"services.AddScoped<I{table.EntityNameSingular}Service, {table.EntityNameSingular}Service>();");
				}

				if (_views.Any()) {
					BreakLine();
					WriteComment("Add services for views. Those are readonly");
					var clientSideViews = _views.Where(v => !v.AccessViewOnly && !string.IsNullOrEmpty(v.EntityNameSingular)).OrderBy(v => v.EntityNameSingular);
					foreach (var view in clientSideViews) { 
						AppendLine($"services.AddScoped<I{view.EntityNameSingular}Service, {view.EntityNameSingular}Service>();");
					}
				}
			}
			EndBlock();
		}
		EndBlock();

		return builder.ToString();
	}
}

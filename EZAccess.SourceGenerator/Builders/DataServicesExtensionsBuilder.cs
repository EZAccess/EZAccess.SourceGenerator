using EZAccess.SourceGenerator.DatabaseDefinition;
using System.Collections.Generic;
using System.Linq;

namespace EZAccess.SourceGenerator.Builders;

internal class DataServicesExtensionsBuilder : CodeBuilderBase
{
	private readonly List<TableDef> _tables;
	private readonly List<ViewDef> _views;
	protected readonly string _namespaceName;

	public DataServicesExtensionsBuilder(List<TableDef> tables, List<ViewDef>? views, string namespaceName) 
	{
		_tables = tables;
		_views = views ?? [];
		_namespaceName = namespaceName;
	}

	internal static string Build(List<TableDef> tables, List<ViewDef>? views, string namespaceName)
		=> new DataServicesExtensionsBuilder(tables, views, namespaceName).BuildInternal();

	private string BuildInternal()
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
				var clientSideTables = _tables
					.Where(t => !t.AccessTableOnly && !string.IsNullOrEmpty(t.EntityNameSingular))
					.OrderBy(t => t.EntityNameSingular)
					.ToList();
				foreach (var table in clientSideTables) {
					AppendLine($"services.AddScoped<I{table.EntityNameSingular}Service2, {table.EntityNameSingular}Service2>();");
				}

				if (_views.Any()) {
					BreakLine();
					WriteComment("Add services for views. Those are readonly");
					var clientSideViews = _views
						.Where(v => !v.AccessViewOnly && !string.IsNullOrEmpty(v.EntityNameSingular))
						.OrderBy(v => v.EntityNameSingular)
						.ToList();
					foreach (var view in clientSideViews) { 
						AppendLine($"services.AddScoped<I{view.EntityNameSingular}Service2, {view.EntityNameSingular}Service2>();");
					}
				}
			}
			EndBlock();
		}
		EndBlock();

		return builder.ToString();
	}
}

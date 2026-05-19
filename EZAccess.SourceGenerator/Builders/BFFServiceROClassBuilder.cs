using EZAccess.SourceGenerator.DatabaseDefinition;
using System.Linq;

namespace EZAccess.SourceGenerator.Builders;

internal class BFFServiceROClassBuilder : CodeBuilderBase
{
	private readonly ViewDef _view;
	private readonly string _namespaceName;

	public BFFServiceROClassBuilder(ViewDef view, string namespaceName)
	{
		_view = view;
		_namespaceName = namespaceName;
	}

	internal static string Build(ViewDef view, string namespaceName)
		=> new BFFServiceROClassBuilder(view, namespaceName).BuildInternal();

	private string BuildInternal()
	{
		var columns = _view.Columns.Where(c => !c.BackendOnly).ToList();
		var keyFields = _view.Columns.Where(c => c.IsKey).ToList();
		string keyFieldType;
		if (keyFields.Count > 1) { 
			keyFieldType = $"({string.Join(", ", keyFields.Select(c => c.GetDotNetDataType()))})";
		}
		else if (keyFields.Count == 1)
		{
			keyFieldType = keyFields.First().GetDotNetDataType();
		}
		else
		{
			keyFieldType = "int"; // Default to int if no key field is defined, but this should probably be handled differently (e.g. throw an error or use a different default type)
		}
		var baseClassName = $"BFFServiceBaseRO<{_view.EntityNameSingular}, {_view.EntityNameSingular}_CoreDTO, {_view.EntityNameSingular}_RequestDTO, {keyFieldType}>";

		WriteDisclaimer();
		AppendLine("using EZAccess.SourceGenerator.Abstractions.BaseClasses;");
		AppendLine("using EZAccess.SourceGenerator.Abstractions.Extensions;");
		AppendLine("using Microsoft.Extensions.Caching.Memory;");
		AppendLine("using System.Text;");
		BreakLine();
		AppendLine($"namespace {_namespaceName}.Services;");

		BreakLine();
		StartBlock($"public partial interface I{_view.EntityNameSingular}Service2 : IBFFServiceBase<{_view.EntityNameSingular}, {_view.EntityNameSingular}_CoreDTO, {_view.EntityNameSingular}_RequestDTO, {keyFieldType}>");
		{
		}
		EndBlock();

		BreakLine();
		StartBlock($"public partial class {_view.EntityNameSingular}Service2 : {baseClassName}");
		{
			AppendLine($"protected override string RequestUri => \"api/{_view.EntityNameSingular}\";");
			AppendLine($"protected override string EntityName => \"{_view.EntityNameSingular}\";");
			BreakLine();

			StartBlock($"public {_view.EntityNameSingular}Service2(HttpClient httpClient, ILogger<{baseClassName}> logger, IMemoryCache cache) : base(httpClient, logger, cache)");
			EndBlock();
			BreakLine();

			StartBlock($"protected override string GetQueryForModel({_view.EntityNameSingular}_RequestDTO request)");
			{
				WriteComment("Write the query as a http query string");
				AppendLine("StringBuilder queryBuilder = new();");
				foreach(var column in columns) {
					if (column.GetDotNetDataType() == "byte[]") {
						WriteComment($"{column.GetSaveVariableName()}: ByteArrays are excluded from queries");
						continue;
					}
					if (column.Depricated) {
						AppendLine("#pragma warning disable CS0618 // Type or member is obsolete. Disable warning in generated code.");
					}
					AppendLine($"if(request.{column.GetSaveVariableName()} is not null)");
					Indent();
					if (column.GetDotNetDataType() == "string") {
						AppendLine($"queryBuilder.Append($\"&{column.GetSaveVariableName()}={{request.{column.GetSaveVariableName()}.EzUrlEncode()}}\");");
					}
					else {
						AppendLine($"queryBuilder.Append($\"&{column.GetSaveVariableName()}={{request.{column.GetSaveVariableName()}.Value.EzUrlEncode()}}\");");
					}
					Unindent();
					if (column.Depricated) {
						AppendLine("#pragma warning restore CS0618 // Type or member is obsolete");
					}
				}
				AppendLine("return queryBuilder.ToString();");
			}
			EndBlock();
			BreakLine();

			StartBlock($"protected override string GetQueryForKey({keyFieldType} key, string seperator = \"/\", bool includeFieldname = false)");
			{
				WriteComment("Write the query as a http query string");
				if (keyFields.Count > 1) {
					AppendLine("StringBuilder queryBuilder = new();");
					for(int i = 0; i < keyFields.Count; i++) {
						var column = keyFields[i];
						AppendLine($"string fieldname{i} = includeFieldname ? \"{column.GetSaveVariableName()}=\" : string.Empty;");
						AppendLine($"queryBuilder.Append($\"{{seperator}}{{fieldname{i}}}{{key.Item{i + 1}.EzUrlEncode()}}\");");
					}
					AppendLine("return queryBuilder.ToString();");
				}
				else {
					var column = keyFields.FirstOrDefault();
					AppendLine($"string fieldname = includeFieldname ? \"{column.GetSaveVariableName()}=\" : string.Empty;");
					if (column != null) {
						AppendLine("return $\"{seperator}{fieldname}{key.EzUrlEncode()}\";");
					}
					else {
						WriteComment("No key fields defined, return empty query string");
						AppendLine("return string.Empty;");
					}
				}
			}
			EndBlock();
		}
		EndBlock();

		return builder.ToString();
	}
}

using System.Collections.Generic;

namespace EZAccess.SourceGenerator.DatabaseDefinition;

/// <summary>
/// DatabaseDef is the main class in the database definition. All components inside the database
/// definition should be accessible from this class.
/// </summary>
internal class DatabaseDef 
{
	/// <summary>
	/// Database name
	/// </summary>
	public string? DatabaseName { get; set; }
	/// <summary>
	/// Current version of the database
	/// </summary>
	public int CurrentVersion { get; set; }
	/// <summary>
	/// Property to indicate if stored procedures should be generated
	/// </summary>
	public bool UseStoredProcedures { get; set; } = true;

	/// <summary>
	/// List of tables in the database
	/// </summary>
	public List<TableDef> Tables { get; set; } = [];

	///// <summary>
	///// List of relations in the database
	///// </summary>
	public List<RelationDef> Relations { get; set; } = [];

	///// <summary>
	///// List of views in the database
	///// </summary>
	public List<ViewDef> Views { get; set; } = [];
}

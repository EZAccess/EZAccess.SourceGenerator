using System;

namespace CodeGenerator.DatabaseDefinition;

/// <summary>
/// Model of Field in a relation. Defining the relation between two fields in two tables.
/// </summary>
public class RelationField : ICloneable
{
	/// <summary>
	/// Name of the field in the parent table
	/// </summary>
	public string? ParentFieldName { get; set; }
	/// <summary>
	/// Name of the field in the referenced tables
	/// </summary>
	public string? ReferencedFieldName { get; set; }

	public object Clone()
	{
		return this.MemberwiseClone();
	}
}

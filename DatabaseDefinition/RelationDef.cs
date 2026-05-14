using System;
using System.Collections.Generic;

namespace CodeGenerator.DatabaseDefinition;

/// <summary>
/// Model class for a relation between two tables
/// </summary>
public class RelationDef : ICloneable
{
	/// <summary>
	/// Unique name of the relation
	/// </summary>
	public string? RelationName { get; set; }
	/// <summary>
	/// Name of the parent table in the relation
	/// </summary>
	public string? ParentTableName { get; set; }
	/// <summary>
	/// Name of the referenced table in the relation
	/// </summary>
	public string? ReferencedTableName { get; set; }
	/// <summary>
	/// List of fields in the relation defining the relation
	/// </summary>
	public List<RelationField> Fields { get; set; } = [];
	/// <summary>
	/// Property defining the action to be taken when the parent record is deleted
	/// </summary>
	public string OnDelete { get; set; } = "NO ACTION";
	/// <summary>
	/// Property defining the action to be taken when the parent record is updated
	/// </summary>
	public string OnUpdate { get; set; } = "NO ACTION";
	/// <summary>
	/// Property indicating that the relation is deleted or should be deleted
	/// </summary>
	public bool IsDeleted { get; set; }
	/// <summary>
	/// The 'parent' table contains the 'child' records of the referenced table. 
	/// Meaning that each record in the referenced table can have multiple 'child' records in the 'parent' table.
	/// The word 'child' is used to indicate that the record is dependent on the referenced record.
	/// Mind the fact that the 'parent' table is not the same as the parent table in the relation.
	/// </summary>
	public bool IsChildOf { get; set; }
	/// <summary>
	/// Display LookUp field of the referenced table
	/// </summary>
	public bool IsLookUp { get; set; }

	/// <summary>
	/// If the relation is a Lookup field, the parent object may get a child property by the source generator.
	/// AspNetObjectName is an instruction for the source generator to determine the name of the child property in the parent object.
	/// </summary>
	public string? AspNetObjectName { get; set; }

	/// <summary>
	/// Databse Version in which the relation is addeds
	/// </summary>
	public int? DatabaseVersionCreated { get; set; }
	/// <summary>
	/// Databse Version in which the relation is updated
	/// </summary>
	public int? DatabaseVersionUpdated { get; set; }
	/// <summary>
	/// Databse Version in which the relation is deleted
	/// </summary>
	public int? DatabaseVersionDeleted { get; set; }

	public object Clone()
	{
		var clone = (RelationDef)this.MemberwiseClone();
		clone.Fields.AddRange(this.Fields.ConvertAll(field => (RelationField)field.Clone()));
		return clone;
	}
}

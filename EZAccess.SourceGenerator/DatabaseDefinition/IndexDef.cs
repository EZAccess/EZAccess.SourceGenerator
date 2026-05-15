using System;
using System.Collections.Generic;

namespace EZAccess.SourceGenerator.DatabaseDefinition;

/// <summary>
/// IndexDef is the class that defines an index in the database definition
/// </summary>
public class IndexDef : ICloneable
{
	/// <summary>
	/// Unique name of the index
	/// </summary>
	public string? IndexName { get; set; }

	/// <summary>
	/// Property indicating that the index is primary
	/// </summary>
	public bool IsPrimary { get; set; }

	/// <summary>
	/// Property indicating that the index is unique
	/// </summary>
	public bool IsUnique { get; set; }

	/// <summary>
	/// Property indicating that the index is clustered
	/// </summary>
	public bool IsClustered { get; set; }

	/// <summary>
	/// Property indicating that the value of the field is required. This is only used for primary keys.
	/// </summary>
	public bool IsRequired { get; set; }

	/// <summary>
	/// List of fields in the index
	/// </summary>
	public List<IndexField> Fields { get; set; } = [];

	/// <summary>
	/// Property indicating that the index is or should be deleted
	/// </summary>
	public bool IsDeleted { get; set; }

	/// <summary>
	/// Databse Version in which the field is added
	/// </summary>
	public int? DatabaseVersionCreated { get; set; }

	/// <summary>
	/// Databse Version in which the field is updated
	/// </summary>
	public int? DatabaseVersionUpdated { get; set; } 

	/// <summary>
	/// Databse Version in which the field is deleted
	/// </summary>
	public int? DatabaseVersionDeleted { get; set; }

	public object Clone()
	{
		IndexDef clone = (IndexDef)this.MemberwiseClone();
		clone.Fields.AddRange(this.Fields.ConvertAll(field => (IndexField)field.Clone()));
		return clone;
	}
}

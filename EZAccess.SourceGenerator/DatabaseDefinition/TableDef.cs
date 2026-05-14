using System;
using System.Collections.Generic;

namespace CodeGenerator.DatabaseDefinition;

internal class TableDef : ICloneable
{
	/// <summary>
	/// Unique name of the table
	/// </summary>
	public string? TableName { get; set; }

	/// <summary>
	/// Database schema in which the table is located
	/// </summary>
	public string? Schema { get; set; }

	/// <summary>
	/// Name of the entity represented by the table in singular form
	/// </summary>
	public string? EntityNameSingular { get; set; }

	/// <summary>
	/// Name of the entity represented by the table in plural form
	/// </summary>
	public string? EntityNamePlural { get; set; }

	/// <summary>
	/// Flag indicating that the table is a system table. System tables are tables that are used by the database system and should not be modified by the user.
	/// </summary>
	public bool IsSystemTable { get; set; }

	/// <summary>
	/// Description of the table
	/// </summary>
	public string? Description { get; set; }

	/// <summary>
	/// Name of the field that is used as the display field for the table
	/// </summary>
	public string? LookupDisplayField { get; set; }

	/// <summary>
	/// Property indicating that the table is depricated or obsolete
	/// </summary>
	public bool Depricated { get; set; }

	/// <summary>
	/// Database version in which the field is added
	/// </summary>
	public int? DatabaseVersionCreated { get; set; }

	/// <summary>
	/// Database version in which the field is updated
	/// </summary>
	public int? DatabaseVersionUpdated { get; set; }

	/// <summary>
	/// Database version in which the field is depricated
	/// </summary>
	public int? DatabaseVersionDepricated { get; set; }

	/// <summary>
	/// Indicator that the table is only meant to be used in ASP based applications
	/// </summary>
	public bool AspTableOnly { get; set; }

	/// <summary>
	/// Indicator that the table is only meant to be used in MS Access based applications
	/// </summary>
	public bool AccessTableOnly { get; set; }

	/// <summary>
	/// List of fields in the table
	/// </summary>
	public List<FieldDef> Fields { get; set; } = [];

	/// <summary>
	/// List of indexes in the table
	/// </summary>
	public List<IndexDef> Indexes { get; set; } = [];

	public object Clone()
	{
		var clone = (TableDef)this.MemberwiseClone();
		clone.Fields.AddRange(this.Fields.ConvertAll(field => (FieldDef)field.Clone()));
		clone.Indexes.AddRange(this.Indexes.ConvertAll(index => (IndexDef)index.Clone()));
		return clone;
	}
}

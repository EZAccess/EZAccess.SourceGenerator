using System;
using System.Collections.Generic;

namespace EZAccess.SourceGenerator.DatabaseDefinition;

/// <summary>
/// TableDef is the class that defines a table in the database definition
/// </summary>
public partial class ViewDef : ICloneable
{
	/// <summary>
	/// Unique name of the view
	/// </summary>
	public string? ViewName { get; set; }

	/// <summary>
	/// Database schema in which the view is located
	/// </summary>
	public string? Schema { get; set; }

	/// <summary>
	/// Name of the entity represented by the view in singular form
	/// </summary>
	public string? EntityNameSingular { get; set; }

	/// <summary>
	/// Name of the entity represented by the view in plural form
	/// </summary>
	public string? EntityNamePlural { get; set; }

	/// <summary>
	/// SQL script that is used for the body of the view in TSQL
	/// </summary>
	public string? Script { get; set; }

	/// <summary>
	/// Description of the view
	/// </summary>
	public string? Description { get; set; }

	/// <summary>
	/// Property indicating that the view is depricated or obsolete
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
	/// Indicator that the view is only meant to be used in ASP based applications
	/// </summary>
	public bool AspViewOnly { get; set; }

	/// <summary>
	/// Indicator that the view is only meant to be used in MS Access based applications
	/// </summary>
	public bool AccessViewOnly { get; set; }

	/// <summary>
	/// List of columns in the view
	/// </summary>
	public List<ViewColumnDef> Columns { get; set; } = [];

	public object Clone()
	{
		var clone = (ViewDef)this.MemberwiseClone();
		clone.Columns.AddRange(this.Columns.ConvertAll(c => (ViewColumnDef)c.Clone()));
		return clone;
	}
}

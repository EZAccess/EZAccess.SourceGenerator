namespace EZAccess.SourceGenerator.DatabaseDefinition;

public interface IColumnDef
{
	#region Functional info
	/// <summary>
	/// Sequence in which fields or columns are displayed when auto generated
	/// </summary>
	public int Sequence { get; set; }
	/// <summary>
	/// ColumnName must be unique
	/// </summary>
	public string? ColumnName { get; set; }
	/// <summary>
	/// SQL server datatype
	/// </summary>
	public string? DataType { get; set; }
	/// <summary>
	/// Length of the field for variable length fields, or precision for decimal fields. This is used for SQL server datatype: NVARCHAR(50) or DECIMAL(18,2)
	/// </summary>
	public int? FieldLength { get; set; }
	/// <summary>
	/// Decimal scale
	/// </summary>
	public int? Scale { get; set; }
	/// <summary>
	/// Primary key field
	/// </summary>
	public bool IsKey { get; set; }
	#endregion

	// Display info
	#region Display info
	/// <summary>
	/// Label for an input field
	/// </summary>
	public string? Label { get; set; }
	/// <summary>
	/// Caption to be used as column header
	/// </summary>
	public string? LabelShort { get; set; }
	/// <summary>
	/// Extra info about the field
	/// </summary>
	public string? Description { get; set; }
	/// <summary>
	/// Logical group name for a set of properties
	/// </summary>
	public string? GroupName { get; set; }
	/// <summary>
	/// Display format for a date field: dd-mm-yyyy
	/// </summary>
	public string? DisplayFormat { get; set; }
	/// <summary>
	/// This field is used by emplates to write comments
	/// </summary>
	public string? Comments { get; set; }
	#endregion

	// Even though views don't need validation for input fields, it can be useful for the front-end.
	#region Validation Info

	/// <summary>
	/// Validate as a specific data type. This translates to AnnotationDataType in C#.
	/// </summary>
	public string? AnnotationDataType { get; set; }

	#endregion

	// Technical data
	#region Technical data
	/// <summary>
	/// Identity column for SQL server. This column is automatically incremented by the database, and should not be set by the user.
	/// </summary>
	public bool IsIdentityColumn { get; set; }

	/// <summary>
	/// Determines if the property is exposed outside the backend
	/// </summary>
	public bool BackendOnly { get; set; }

	/// <summary>
	/// Can be used by the front-end, but also back-ends to make the field (non)editable
	/// </summary>
	public bool IsEditable { get; set; }

	/// <summary>
	/// When no longer in use. In C# the property is marked as obsolete.
	/// </summary>
	public bool Depricated { get; set; }
	#endregion

}

using System;

namespace CodeGenerator.DatabaseDefinition;

/// <summary>
/// FieldDef is the class that defines a field in the database definition
/// </summary>
public partial class FieldDef : IColumnDef, ICloneable
{
	// Functional info
	#region Functional info
	/// <summary>
	/// Sequence in which fields or columns are displayed when auto generated
	/// </summary>
	public int Sequence { get; set; }
	/// <summary>
	/// Fieldname must be unique
	/// </summary>
	public string? FieldName { get; set; }
	/// <summary>
	/// Sinonym for FieldName. This is used for compatibility with the IColumnDef interface.
	/// </summary>
	public string? ColumnName { get => FieldName; set => FieldName = value; }

	/// <summary>
	/// SQL server datatype
	/// </summary>
	public string? DataType { get; set; }
	/// <summary>
	/// Input validation for minimal length
	/// </summary>
	public int? MinLength { get; set; }
	/// <summary>
	/// Maximum length of the data. Also used for SQL server datatype: NVARCHAR(50)
	/// </summary>
	public int? MaxLength { get; set; }
	/// <summary>
	/// Field length is a wrapper for MaxLength. This is used for compatibility with the IColumnDef interface.
	/// </summary>
	public int? FieldLength { get => MaxLength; set => MaxLength = value; }
	/// <summary>
	/// Decimal scale
	/// </summary>
	public int? Scale { get; set; }
	/// <summary>
	/// Primary key field
	/// </summary>
	public bool IsKey { get; set; }
	/// <summary>
	/// Field is required
	/// </summary>
	public bool IsRequired { get; set; }
	/// <summary>
	/// Field refers to a file on disk. This property can by used for additional functionality related to processing files.
	/// </summary>
	public bool IsFile { get; set; }
	/// <summary>
	/// Default constraint for SQL server
	/// </summary>
	public string? DefaultConstraint { get; set; }
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
	/// Select the type of input field: text, password, checkbox, radio, select, textarea, file, hidden, etc.
	/// </summary>
	public string? InputType { get; set; }
	/// <summary>
	/// This field is used by templates to write comments
	/// </summary>
	public string? Comments { get; set; }
	#endregion

	// Validation info
	#region Validation info
	/// <summary>
	/// Validate as Email, Url, Phone or Creditcard. This translates to Annotation Attributes in C# like EmailAddress, Url, Phone, CreditCard.
	/// This property is similar to the AnnotationDataType property, but has less options.
	/// </summary>
	public string? ValidateAsDataType { get; set; }

	/// <summary>
	/// Validate as a specific data type. This translates to AnnotationDataType in C#.
	/// </summary>
	public string? AnnotationDataType { get; set; }

	/// <summary>
	/// Valiate range: Minimum value
	/// </summary>
	public string? LowerRange { get; set; }

	/// <summary>
	/// Validate range: Maximum value
	/// </summary>
	public string? UpperRange { get; set; }

	/// <summary>
	/// Validate as regular expression.
	/// </summary>
	public string? RegularExpression { get; set; }

	/// <summary>
	/// List of values for a lookup field. Values need to be separated by a semicolon.
	/// </summary>
	public string? SelectFromList { get; set; }

	/// <summary>
	/// Number of columns to be used for a lookup field. If omitted 1 is assumed. 
	/// This is used as display instruction for the LookupList field. 
	/// If the LookupList contains 100 values and the LookupListColumnCount is set to 2, 
	/// the lookup list will be displayed as a 2 column list with 50 rows.
	/// </summary>
	public int? SelectFromList_ColumnCount { get; set; }
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

	#endregion

	public object Clone()
	{
		// Create a new instance and copy all properties
		return this.MemberwiseClone();
	}
}

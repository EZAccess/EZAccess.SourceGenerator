using System;

namespace CodeGenerator.DatabaseDefinition;

/// <summary>
/// Model class for IndexField
/// </summary>
public class IndexField : ICloneable
{
	/// <summary>
	/// Name of the field
	/// </summary>
	public string? FieldName { get; set; }
	/// <summary>
	/// Sort order for the field is either ASC or DESC
	/// </summary>
	public string? SortOrder { get; set; }

	public object Clone()
	{
		return this.MemberwiseClone();
	}
}

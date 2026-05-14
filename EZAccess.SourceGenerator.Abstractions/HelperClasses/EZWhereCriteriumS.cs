using System.Collections.Generic;
using System.Data;

namespace EZAccess.SourceGenerator.Abstractions.HelperClasses;

public struct EZWhereCriteriumS
{
    /// <summary>
    /// When the list OrCriteria has items, any other property is ignored.
    /// </summary>
    public List<EZWhereCriteriumS> OrCriteria { get; set; }

	/// <summary>
	/// When the list AndCriteria has items, any other property is ignored. OrCriteria is evaluated first.
	/// </summary>
	public List<EZWhereCriteriumS> AndCriteria { get; set; }

	public string? TableName { get; set; }

	public string FieldName { get; set; } 

	/// <summary>
	/// Operator to use for the filter. Use SQL operators like =, <>, >, <, >=, <=, LIKE, NOT LIKE, IN, NOT IN, IS, IS NOT, BETWEEN, NOT BETWEEN
	/// </summary>
	public string SqlOperator { get; set; }

	public string SqlExpression { get; set; }

	/// <summary>
	/// Value to filter on. Use this for =, <>, >, <, >=, <=, LIKE, NOT LIKE, IS, IS NOT operators
	/// </summary>
	public string? Value { get; set; }

	/// <summary>
	/// Array of values to filter on. Use this for IN, NOT IN, BETWEEN and NOT BETWEEN operators
	/// </summary>
	public object[] Values { get; set; } 

	/// <summary>
	/// Helper property to set the DbType of the Value property
	/// </summary>
	public DbType DbType { get; set; } 

}

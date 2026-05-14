using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

namespace EZAccess.SourceGenerator.Abstractions.HelperClasses;

public struct EZWhereCriterium
{
	private readonly string? _sqlExpression;

	#region Constructors
	/// <summary>
	/// Initialize a new WhereCriterium. Use the properties to set the values.
	/// </summary>
	public EZWhereCriterium()	
	{
		OrCriteria = [];
		AndCriteria = [];
	}

	public EZWhereCriterium(EZWhereCriteriumS criteria)
	{
		OrCriteria = [];
		AndCriteria = [];
		if (criteria.OrCriteria.Count > 0) {
			foreach (var orCriterium in criteria.OrCriteria) {
				OrCriteria.Add(new EZWhereCriterium(orCriterium));
			}
		}
		else if (criteria.AndCriteria.Count > 0) {
			foreach (var andCriterium in criteria.AndCriteria) {
				AndCriteria.Add(new EZWhereCriterium(andCriterium));
			}
		}
		else {
			TableName = criteria.TableName;
			FieldName = criteria.FieldName;
			if (!string.IsNullOrEmpty(criteria.SqlExpression) && criteria.SqlExpression != criteria.FieldName) {
				_sqlExpression = criteria.SqlExpression;
			}
			SqlOperator = criteria.SqlOperator;
			Value = criteria.Value;
			Values = criteria.Values;
			DbType = criteria.DbType;
		}
	}

	/// <summary>
	/// Initialize a new WhereCriterium with a list of other WhereCriterium objects.
	/// The where criteria will be combined with an OR operator.
	/// </summary>
	/// <param name="orCriteria"></param>
	public EZWhereCriterium(IEnumerable<EZWhereCriterium> criteria, bool isAnd = false) 
	{
		if (isAnd) { 
			AndCriteria = [.. criteria]; 
			OrCriteria = [];
		}
		else {
			OrCriteria = [.. criteria]; 
			AndCriteria = [];
		}
	}

	/// <summary>
	/// Initialize a new WhereCriterium with a fieldname and a value. 
	/// The fieldname will be used as the SQL expression. 
	/// When the fieldname is an alias, use the SqlExpression property.
	/// The operator will be set to =.
	/// </summary>
	/// <param name="fieldName">The fieldname as in the SQL database.</param>
	/// <param name="Value">The value to filter as a string. Use the DbType property to set the type of the value.</param>
	public EZWhereCriterium(string fieldName, string? value) : this()
	{
		FieldName = fieldName;
		if (value is not null) { 
			Value = value; 
		}
		else {
			SqlOperator = "IS";
			Value = "Null";
		}
	}

	/// <summary>
	/// Initialize a new WhereCriterium with a fieldname and a value.
	/// The fieldname will be used as the SQL expression.
	/// When the fieldname is an alias, use the SqlExpression property.
	/// The operator will be set to =.
	/// </summary>
	/// <param name="fieldName">The fieldname as in the SQL database.</param>
	/// <param name="Value">The value to filter as an integer.</param>
	public EZWhereCriterium(string fieldName, int? value) : this()
	{
		FieldName = fieldName;
		if (value is not null) {
			Value = value.ToString(); 
		}
		else {
			SqlOperator = "IS";
			Value = "Null";
		}
		DbType = DbType.Int32;
	}

	/// <summary>
	/// Initialize a new WhereCriterium with a fieldname and a value.
	/// The fieldname will be used as the SQL expression.
	/// When the fieldname is an alias, use the SqlExpression property.
	/// The operator will be set to =.
	/// </summary>
	/// <param name="fieldName">The fieldname as in the SQL database.</param>
	/// <param name="Value">The value to filter as a long.</param>
	public EZWhereCriterium(string fieldName, long? value) : this()
	{
		FieldName = fieldName;
		if (value is not null) {
			Value = value.ToString();
		}
		else {
			SqlOperator = "IS";
			Value = "Null";
		}
		DbType = DbType.Int64;
	}

	/// <summary>
	/// Initialize a new WhereCriterium with a fieldname and a value.
	/// The fieldname will be used as the SQL expression.
	/// When the fieldname is an alias, use the SqlExpression property.
	/// The operator will be set to =.
	/// </summary>
	/// <param name="fieldName">The fieldname as in the SQL database.</param>
	/// <param name="Value">The value to filter as a double.</param>
	public EZWhereCriterium(string fieldName, double? value) : this()
	{
		FieldName = fieldName;
		if (value is not null) { 
			Value = value.ToString(); 
		}
		else
		{
			SqlOperator = "IS";
			Value = "Null";
		}
		DbType = DbType.Double;
	}

	/// <summary>
	/// Initialize a new WhereCriterium with a fieldname and a value.
	/// The fieldname will be used as the SQL expression.
	/// When the fieldname is an alias, use the SqlExpression property.
	/// The operator will be set to =.
	/// </summary>
	/// <param name="fieldName">The fieldname as in the SQL database.</param>
	/// <param name="Value">The value to filter as a decimal.</param>
	public EZWhereCriterium(string fieldName, decimal? value) : this()
	{
		FieldName = fieldName;
		if (value is not null) { 
			Value = value.ToString(); 
		}
		else {
			SqlOperator = "IS";
			Value = "Null";
		}
		DbType = DbType.Decimal;
	}

	/// <summary>
	/// Initialize a new WhereCriterium with a fieldname and a value.
	/// The fieldname will be used as the SQL expression.
	/// When the fieldname is an alias, use the SqlExpression property.
	/// The operator will be set to =.
	/// </summary>
	/// <param name="fieldName">The fieldname as in the SQL database.</param>
	/// <param name="Value">The value to filter as a DateTime.</param>
	public EZWhereCriterium(string fieldName, DateTime? value) : this()
	{
		FieldName = fieldName;
		if (value is not null) { 
			Value = value.Value.ToString("o", CultureInfo.InvariantCulture); 
		}
		else {
			SqlOperator = "IS";
			Value = "Null";
		}
		DbType = DbType.DateTime;
	}

	/// <summary>
	/// Initialize a new WhereCriterium with a fieldname and a value.
	/// The fieldname will be used as the SQL expression.
	/// When the fieldname is an alias, use the SqlExpression property.
	/// The operator will be set to =.
	/// </summary>
	/// <param name="fieldName">The fieldname as in the SQL database.</param>
	/// <param name="Value">The value to filter as a boolean.</param>
	public EZWhereCriterium(string fieldName, bool? value) : this()
	{
		FieldName = fieldName;
		if (value is not null) { 
			Value = value.ToString(); 
		}
		else {
			SqlOperator = "IS";
			Value = "Null";
		}
		DbType = DbType.Boolean;
	}

	/// <summary>
	/// Initialize a new WhereCriterium with a fieldname, a value and a SQL operator.
	/// The fieldName will be used as the SQL expression.
	/// When the fieldname is an alias, use the SqlExpression property.
	/// </summary>
	/// <param name="fieldName">The fieldname as in the SQL database.</param>
	/// <param name="sqlOperator">Any valid SQL operator like =, <>, >, <, >=, <=, LIKE, NOT LIKE, IN, NOT IN, IS, IS NOT, BETWEEN, NOT BETWEEN</param>
	/// <param name="Value">The value to filter as a string. Use the DbType property to set the type of the value.</param>
	/// <param name="dbType">The type of the value. Default is DbType.String.</param>
	public EZWhereCriterium(string fieldName, string sqlOperator, string? value, DbType dbType = DbType.String) : this()
	{
		FieldName = fieldName;
		SqlOperator = sqlOperator;
		if (value is not null) { 
			Value = value; 
		}
		else {
			if (sqlOperator == "=") SqlOperator = "IS";
			if (sqlOperator == "<>") SqlOperator = "IS NOT";
			Value = "Null";
		}
		DbType = dbType;
	}

	/// <summary>
	/// Initialize a new WhereCriterium with a fieldname, a value and a SQL operator.
	/// The fieldName will be used as the SQL expression.
	/// When the fieldname is an alias, use the SqlExpression property.
	/// </summary>
	/// <param name="fieldName">The fieldname as in the SQL database.</param>
	/// <param name="sqlOperator">Any valid SQL operator like =, <>, >, <, >=, <=, LIKE, NOT LIKE, IN, NOT IN, IS, IS NOT, BETWEEN, NOT BETWEEN</param>
	/// <param name="Value">The value to filter as an integer.</param>
	public EZWhereCriterium(string fieldName, string sqlOperator, int? value) : this()
	{
		FieldName = fieldName;
		SqlOperator = sqlOperator;
		if (value is not null) { 
			Value = value.ToString(); 
		}
		else {
			if (sqlOperator == "=") SqlOperator = "IS";
			if (sqlOperator == "<>") SqlOperator = "IS NOT";
			Value = "Null";
		}
		DbType = DbType.Int32;
	}

	/// <summary>
	/// Initialize a new WhereCriterium with a fieldname, a value and a SQL operator.
	/// The fieldName will be used as the SQL expression.
	/// When the fieldname is an alias, use the SqlExpression property.
	/// </summary>
	/// <param name="fieldName">The fieldname as in the SQL database.</param>
	/// <param name="sqlOperator">Any valid SQL operator like =, <>, >, <, >=, <=, LIKE, NOT LIKE, IN, NOT IN, IS, IS NOT, BETWEEN, NOT BETWEEN</param>
	/// <param name="Value">The value to filter as a long.</param>
	public EZWhereCriterium(string fieldName, string sqlOperator, long? value) : this()
	{
		FieldName = fieldName;
		SqlOperator = sqlOperator;
		if (value is not null) { 
			Value = value.ToString(); 
		}
		else {
			if (sqlOperator == "=") SqlOperator = "IS";
			if (sqlOperator == "<>") SqlOperator = "IS NOT";
			Value = "Null";
		}
		DbType = DbType.Int64;
	}

	/// <summary>
	/// Initialize a new WhereCriterium with a fieldname, a value and a SQL operator.
	/// The fieldName will be used as the SQL expression.
	/// When the fieldname is an alias, use the SqlExpression property.
	/// </summary>
	/// <param name="fieldName">The fieldname as in the SQL database.</param>
	/// <param name="sqlOperator">Any valid SQL operator like =, <>, >, <, >=, <=, LIKE, NOT LIKE, IN, NOT IN, IS, IS NOT, BETWEEN, NOT BETWEEN</param>
	/// <param name="Value">The value to filter as a double.</param>
	public EZWhereCriterium(string fieldName, string sqlOperator, double? value) : this()
	{
		FieldName = fieldName;
		SqlOperator = sqlOperator;
		if (value is not null) { 
			Value = value.ToString(); 
		}
		else
		{
			if (sqlOperator == "=") SqlOperator = "IS";
			if (sqlOperator == "<>") SqlOperator = "IS NOT";
			Value = "Null";
		}
		DbType = DbType.Double;
	}

	/// <summary>
	/// Initialize a new WhereCriterium with a fieldname, a value and a SQL operator.
	/// The fieldName will be used as the SQL expression.
	/// When the fieldname is an alias, use the SqlExpression property.
	/// </summary>
	/// <param name="fieldName">The fieldname as in the SQL database.</param>
	/// <param name="sqlOperator">Any valid SQL operator like =, <>, >, <, >=, <=, LIKE, NOT LIKE, IN, NOT IN, IS, IS NOT, BETWEEN, NOT BETWEEN</param>
	/// <param name="Value">The value to filter as a decimal.</param>
	public EZWhereCriterium(string fieldName, string sqlOperator, decimal? value) : this()
	{
		FieldName = fieldName;
		SqlOperator = sqlOperator;
		if (value is not null)
			Value = value.ToString();
		else {
			if (sqlOperator == "=") SqlOperator = "IS";
			if (sqlOperator == "<>") SqlOperator = "IS NOT";
			Value = "Null";
		}
		DbType = DbType.Decimal;
	}

	/// <summary>
	/// Initialize a new WhereCriterium with a fieldname, a value and a SQL operator.
	/// The fieldName will be used as the SQL expression.
	/// When the fieldname is an alias, use the SqlExpression property.
	/// </summary>
	/// <param name="fieldName">The fieldname as in the SQL database.</param>
	/// <param name="sqlOperator">Any valid SQL operator like =, <>, >, <, >=, <=, LIKE, NOT LIKE, IN, NOT IN, IS, IS NOT, BETWEEN, NOT BETWEEN</param>
	/// <param name="Value">The value to filter as a DateTime.</param>
	public EZWhereCriterium(string fieldName, string sqlOperator, DateTime? value) : this()
	{
		FieldName = fieldName;
		SqlOperator = sqlOperator;
		if (value is not null) { 
			Value = value.Value.ToString("o", CultureInfo.InvariantCulture); 
		}
		else {
			if (sqlOperator == "=") SqlOperator = "IS";
			if (sqlOperator == "<>") SqlOperator = "IS NOT";
			Value = "Null";
		}
		DbType = DbType.DateTime;
	}

	/// <summary>
	/// Initialize a new WhereCriterium with a fieldname, a value and a SQL operator.
	/// The fieldName will be used as the SQL expression.
	/// When the fieldname is an alias, use the SqlExpression property.
	/// </summary>
	/// <param name="fieldName"><The fieldname as in the SQL database.</param>
	/// <param name="sqlOperator">Any valid SQL operator like =, <>, >, <, >=, <=, LIKE, NOT LIKE, IN, NOT IN, IS, IS NOT, BETWEEN, NOT BETWEEN</param>
	/// <param name="value">The value to filter as a DateTime.</param>
	public EZWhereCriterium(string fieldName, string sqlOperator, bool? value) : this()
	{
		FieldName = fieldName;
		SqlOperator = sqlOperator;
		if (value is not null) { 
			Value = value.ToString(); 
		}
		else
		{
			if (sqlOperator == "=") SqlOperator = "IS";
			if (sqlOperator == "<>") SqlOperator = "IS NOT";
			Value = "Null";
		}
		DbType = DbType.Boolean;
	}

	/// <summary>
	/// Initialize a new WhereCriterium with a fieldname, a value and a SQL operator.
	/// </summary>
	/// <param name="fieldName">The fieldname or alias as in the SQL database.</param>
	/// <param name="sqlExpression">The fieldname as an SQL expression like [table].[fieldname]</param>
	/// <param name="sqlOperator">Any valid SQL operator like =, <>, >, <, >=, <=, LIKE, NOT LIKE, IN, NOT IN, IS, IS NOT, BETWEEN, NOT BETWEEN</param>
	/// <param name="Value">The value to filter as a string. Use the DbType property to set the type of the value.</param>
	/// <param name="dbType">The type of the value. Default is DbType.String.</param>
	public EZWhereCriterium(string fieldName, string sqlExpression, string sqlOperator, string? value, DbType dbType = DbType.String) : this()
	{
		FieldName = fieldName;
		_sqlExpression = sqlExpression;
		SqlOperator = sqlOperator;
		if (value != null) { 
			Value = value; 
		}
		else {
			if (sqlOperator == "=") SqlOperator = "IS";
			if (sqlOperator == "<>") SqlOperator = "IS NOT";
			Value = "Null";
		}
		DbType = dbType;
	}

	/// <summary>
	/// Initialize a new WhereCriterium with a fieldname, a value and a SQL operator.
	/// </summary>
	/// <param name="fieldName">The fieldname or alias as in the SQL database.</param>
	/// <param name="sqlExpression">The fieldname as an SQL expression like [table].[fieldname]</param>
	/// <param name="sqlOperator">Any valid SQL operator like =, <>, >, <, >=, <=, LIKE, NOT LIKE, IN, NOT IN, IS, IS NOT, BETWEEN, NOT BETWEEN</param>
	/// <param name="Value">The value to filter as an integer.</param>
	public EZWhereCriterium(string fieldName, string sqlExpression, string sqlOperator, int? value) : this()
	{
		FieldName = fieldName;
		_sqlExpression = sqlExpression;
		SqlOperator = sqlOperator;
		if (value is not null) { 
			Value = value.ToString(); 
		}
		else {
			if (sqlOperator == "=") SqlOperator = "IS";
			if (sqlOperator == "<>") SqlOperator = "IS NOT";
			Value = "Null";
		}
		DbType = DbType.Int32;
	}

	/// <summary>
	/// Initialize a new WhereCriterium with a fieldname, a value and a SQL operator.
	/// </summary>
	/// <param name="fieldName">The fieldname or alias as in the SQL database.</param>
	/// <param name="sqlExpression">The fieldname as an SQL expression like [table].[fieldname]</param>
	/// <param name="sqlOperator">Any valid SQL operator like =, <>, >, <, >=, <=, LIKE, NOT LIKE, IN, NOT IN, IS, IS NOT, BETWEEN, NOT BETWEEN</param>
	/// <param name="Value">The value to filter as a long.</param>
	public EZWhereCriterium(string fieldName, string sqlExpression, string sqlOperator, long? value) : this()
	{
		FieldName = fieldName;
		_sqlExpression = sqlExpression;
		SqlOperator = sqlOperator;
		if (value is not null) { 
			Value = value.ToString(); 
		}
		else {
			if (sqlOperator == "=") SqlOperator = "IS";
			if (sqlOperator == "<>") SqlOperator = "IS NOT";
			Value = "Null";
		}
		DbType = DbType.Int64;
	}

	/// <summary>
	/// Initialize a new WhereCriterium with a fieldname, a value and a SQL operator.
	/// </summary>
	/// <param name="fieldName">The fieldname or alias as in the SQL database.</param>
	/// <param name="sqlExpression">The fieldname as an SQL expression like [table].[fieldname]</param>
	/// <param name="sqlOperator">Any valid SQL operator like =, <>, >, <, >=, <=, LIKE, NOT LIKE, IN, NOT IN, IS, IS NOT, BETWEEN, NOT BETWEEN</param>
	/// <param name="Value">The value to filter as a double.</param>
	public EZWhereCriterium(string fieldName, string sqlExpression, string sqlOperator, double? value) : this()
	{
		FieldName = fieldName;
		_sqlExpression = sqlExpression;
		SqlOperator = sqlOperator;
		if (value is not null) { 
			Value = value.ToString(); 
		}
		else
		{
			if (sqlOperator == "=") SqlOperator = "IS";
			if (sqlOperator == "<>") SqlOperator = "IS NOT";
			Value = "Null";
		}
		DbType = DbType.Double;
	}

	/// <summary>
	/// Initialize a new WhereCriterium with a fieldname, a value and a SQL operator.
	/// </summary>
	/// <param name="fieldName">The fieldname or alias as in the SQL database.</param>
	/// <param name="sqlExpression">The fieldname as an SQL expression like [table].[fieldname]</param>
	/// <param name="sqlOperator">Any valid SQL operator like =, <>, >, <, >=, <=, LIKE, NOT LIKE, IN, NOT IN, IS, IS NOT, BETWEEN, NOT BETWEEN</param>
	/// <param name="Value">The value to filter as a decimal.</param>
	public EZWhereCriterium(string fieldName, string sqlExpression, string sqlOperator, decimal? value) : this()
	{
		FieldName = fieldName;
		_sqlExpression = sqlExpression;
		SqlOperator = sqlOperator;
		if (value is not null) { 
			Value = value.ToString(); 
		}
		else {
			if (sqlOperator == "=") SqlOperator = "IS";
			if (sqlOperator == "<>") SqlOperator = "IS NOT";
			Value = "Null";
		}
		DbType = DbType.Decimal;
	}

	/// <summary>
	/// Initialize a new WhereCriterium with a fieldname, a value and a SQL operator.
	/// </summary>
	/// <param name="fieldName">The fieldname or alias as in the SQL database.</param>
	/// <param name="sqlExpression">The fieldname as an SQL expression like [table].[fieldname]</param>
	/// <param name="sqlOperator">Any valid SQL operator like =, <>, >, <, >=, <=, LIKE, NOT LIKE, IN, NOT IN, IS, IS NOT, BETWEEN, NOT BETWEEN</param>
	/// <param name="Value">The value to filter as a DateTime.</param>
	public EZWhereCriterium(string fieldName, string sqlExpression, string sqlOperator, DateTime? value) : this()
	{
		FieldName = fieldName;
		_sqlExpression = sqlExpression;
		SqlOperator = sqlOperator;
		if (value is not null) { 
			Value = value.EzFormat("o", CultureInfo.InvariantCulture); 
		}
		else {
			if (sqlOperator == "=") SqlOperator = "IS";
			if (sqlOperator == "<>") SqlOperator = "IS NOT";
			Value = "Null";
		}
		DbType = DbType.DateTime;
	}

	/// <summary>
	/// Initialize a new WhereCriterium with a fieldname, a sqlExpression, a value and a SQL operator.
	/// </summary>
	/// <param name="fieldName">The fieldname or alias as in the SQL database.</param>
	/// <param name="sqlExpression">The fieldname as an SQL expression like [table].[fieldname]</param>
	/// <param name="sqlOperator">Any valid SQL operator like =, <>, >, <, >=, <=, LIKE, NOT LIKE, IN, NOT IN, IS, IS NOT, BETWEEN, NOT BETWEEN</param>
	/// <param name="Value">The value to filter as a boolean.</param>
	public EZWhereCriterium(string fieldName, string sqlExpression, string sqlOperator, bool? value) : this()
	{
		FieldName = fieldName;
		_sqlExpression = sqlExpression;
		SqlOperator = sqlOperator;
		if (value is not null) { 
			Value = value.ToString(); 
		}
		else
		{
			if (sqlOperator == "=") SqlOperator = "IS";
			if (sqlOperator == "<>") SqlOperator = "IS NOT";
			Value = "Null";
		}
		DbType = DbType.Boolean;
	}

	/// <summary>
	/// Initialize a new WhereCriterium with a fieldname, a sqlExpression, a value and a SQL operator.
	/// </summary>
	/// <param name="fieldName">The fieldname or alias as in the SQL database.</param>
	/// <param name="sqlExpression">The fieldname as an SQL expression like [table].[fieldname]</param>
	/// <param name="sqlOperator">Any valid SQL operator like =, <>, >, <, >=, <=, LIKE, NOT LIKE, IN, NOT IN, IS, IS NOT, BETWEEN, NOT BETWEEN</param>
	/// <param name="value">The value to filter as a Guid.</param>
	public EZWhereCriterium(string fieldName, string sqlExpression, string sqlOperator, Guid? value) : this()
	{
		FieldName = fieldName;
		_sqlExpression = sqlExpression;
		SqlOperator = sqlOperator;
		if (value is not null) { 
			Value = value.ToString(); 
		}
		else {
			if (sqlOperator == "=") SqlOperator = "IS";
			if (sqlOperator == "<>") SqlOperator = "IS NOT";
			Value = "Null";
		}
		DbType = DbType.Boolean;
	}

	/// <summary>
	/// Initialize a new WhereCriterium with a fieldname and an array of values.
	/// This is used for the IN, NOT IN, BETWEEN and NOT BETWEEN operators.
	/// </summary>
	/// <param name="fieldName">The fieldname or alias as in the SQL database.</param>
	/// <param name="sqlExpression">The fieldname as an SQL expression like [table].[fieldname]</param>
	/// <param name="sqlOperator">Use IN, NOT IN, BETWEEN or NOT BETWEEN</param>
	/// <param name="values">An array of values to filter on</param>
	/// <param name="dbType">The type of the values. Default is DbType.String.</param>
	public EZWhereCriterium(string fieldName, string sqlExpression, string sqlOperator, object[] values, DbType dbType = DbType.String) : this()
	{
		FieldName = fieldName;
		_sqlExpression = sqlExpression;
		SqlOperator = sqlOperator;
		Value = null; // Set to null, because we use the Values property
		Values = values;
		DbType = dbType;
	}

	#endregion Constructors

	/// <summary>
	/// When the list OrCriteria has items, they will be combined with an OR operator to this criterium.
	/// Any other property is ignored
	/// </summary>
	public List<EZWhereCriterium> OrCriteria { get; set; }

	/// <summary>
	/// When the list AndCriteria has items, they will be combined with an AND operator to this criterium.
	/// Any other property is ignored.
	/// </summary>
	public List<EZWhereCriterium> AndCriteria { get; set; }

	public string? TableName { get; set; }

	public string FieldName { get; set; } = string.Empty;

	public readonly string FieldSafeName => FieldName.Replace("[", "").Replace("]", "").Replace(".", "_");

	/// <summary>
	/// SQL expression to use for the filter. Use this like "[table].[field]". If empty, the FieldName will be used
	/// </summary>
	public readonly string SqlExpression => GetSqlExpression(null);

	/// <summary>
	/// Get the SQL expression to use for the filter. Use this like "[table].[field]". If empty, the FieldName will be used
	/// When a tableName is provided, it will be used to prefix the fieldname
	/// </summary>
	/// <param name="tableName"></param>
	/// <returns></returns>
	public readonly string GetSqlExpression(string? tableName)
	{
		// a predefined SQL expression always takes precedence
		if (!string.IsNullOrEmpty(_sqlExpression)) return _sqlExpression; 
		// a fieldname with square brackets is asumed to be an alias
		if (FieldName.StartsWith("[") && FieldName.EndsWith("]")) return FieldName;
		// if a TableName is set, use it to prefix the fieldname
		if (!string.IsNullOrEmpty(TableName)) return $"[{TableName}].[{FieldName}]";
		// if a tableName is provided, use it to prefix the fieldname
		if (!string.IsNullOrEmpty(tableName)) {
			if (tableName.StartsWith("[") && tableName.EndsWith("]")) {
				return $"{tableName}.[{FieldName}]";
			}
			return $"[{tableName}].[{FieldName}]";
		}
		// otherwise, return the fieldname as is
		return FieldName;
	}

	/// <summary>
	/// Operator to use for the filter. Use SQL operators like =, <>, >, <, >=, <=, LIKE, NOT LIKE, IN, NOT IN, IS, IS NOT, BETWEEN, NOT BETWEEN
	/// </summary>
	public string SqlOperator { get; } = "=";

	/// <summary>
	/// Value to filter on. Use this for =, <>, >, <, >=, <=, LIKE, NOT LIKE, IS, IS NOT operators
	/// </summary>
	public string? Value { get; }

	/// <summary>
	/// Array of values to filter on. Use this for IN, NOT IN, BETWEEN and NOT BETWEEN operators
	/// </summary>
	public object[] Values { get; } = [];

	/// <summary>
	/// Helper property to set the DbType of the Value property
	/// </summary>
	public DbType DbType { get; } = DbType.String;

}

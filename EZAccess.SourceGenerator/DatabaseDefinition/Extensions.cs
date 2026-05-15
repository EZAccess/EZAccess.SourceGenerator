namespace EZAccess.SourceGenerator.DatabaseDefinition;

public static class Extensions
{
	/// <summary>
	/// Get the DbType for the field in the notation of System.Data.DbType...
	/// </summary>
	/// <param name="fieldDef"></param>
	/// <returns></returns>
	public static string GetSystemDataDbType(this FieldDef fieldDef)
	{
		return fieldDef.DataType switch
		{
			"NVARCHAR" => "System.Data.DbType.String",
			"NVARCHAR(MAX)" => "System.Data.DbType.String",
			"BINARY" => "System.Data.DbType.Binary",
			"VARBINARY" => "System.Data.DbType.Binary",
			"VARBINARY(MAX)" => "System.Data.DbType.Binary",
			"BIGINT" => "System.Data.DbType.Int64",
			"INT" => "System.Data.DbType.Int32",
			"SMALLINT" => "System.Data.DbType.Int16",
			"TINYINT" => "System.Data.DbType.Byte",
			"BIT" => "System.Data.DbType.Boolean",
			"REAL" => "System.Data.DbType.Single",
			"FLOAT" => "System.Data.DbType.Double",
			"DECIMAL" => "System.Data.DbType.Decimal",
			"MONEY" => "System.Data.DbType.Currency",
			"SMALLMONEY" => "System.Data.DbType.Currency",
			"DATETIME" => "System.Data.DbType.DateTime",
			"DATETIME2" => "System.Data.DbType.DateTime2",
			"DATETIMEOFFSET" => "System.Data.DbType.DateTimeOffset",
			"SMALLDATETIME" => "System.Data.DbType.DateTime",
			"DATE" => "System.Data.DbType.Date",
			"TIME" => "System.Data.DbType.Time",
			"TIMESTAMP" => "System.Data.DbType.Binary",
			"UNIQUEIDENTIFIER" => "System.Data.DbType.Guid",
			_ => "System.Data.DbType.String",
		};
	}

	/// <summary>
	/// Get the SQL datatype for the field in the notation of SQL server: NVARCHAR(50)
	/// </summary>
	/// <returns>DataType of SQL server</returns>
	public static string GetSqlDataType(this IColumnDef columnDef)
	{
		return columnDef.DataType switch
		{
			"NVARCHAR" => $"NVARCHAR({columnDef.FieldLength ?? 50})",
			"BINARY" => $"BINARY({columnDef.FieldLength ?? 50})",
			"VARBINARY" => $"VARBINARY({columnDef.FieldLength ?? 50})",
			"DECIMAL" => $"DECIMAL({columnDef.FieldLength ?? 5},{columnDef.Scale ?? 2})",
			_ => columnDef.DataType ?? "missing",

		};
	}

	/// <summary>
	/// Get the datatype for the field in the notation of C#.
	/// </summary>
	/// <returns>DataType of C#</returns>
	public static string GetDotNetDataType(this IColumnDef columnDef)
	{
		return columnDef.DataType switch
		{
			"NVARCHAR" => "string",
			"NVARCHAR(MAX)" => "string",
			"BINARY" => "byte[]",
			"VARBINARY" => "byte[]",
			"VARBINARY(MAX)" => "byte[]",
			"BIGINT" => "long",
			"INT" => "int",
			"SMALLINT" => "short",
			"TINYINT" => "byte",
			"BIT" => "bool",
			"REAL" => "float",
			"FLOAT" => "double",
			"DECIMAL" => "decimal",
			"MONEY" => "decimal",
			"SMALLMONEY" => "decimal",
			"DATETIME" => "DateTime",
			"DATETIME2" => "DateTime",
			"DATETIMEOFFSET" => "DateTimeOffset",
			"SMALLDATETIME" => "DateTime",
			"DATE" => "DateOnly",
			"TIME" => "TimeOnly",
			"TIMESTAMP" => "byte[]",
			"UNIQUEIDENTIFIER" => "Guid",
			_ => "string",
		};
	}

	/// <summary>
	/// Get the DbType for the field in the notation of System.Data.DbType...
	/// </summary>
	/// <param name="fieldDef"></param>
	/// <returns></returns>
	public static string GetSystemDataDbType(this IColumnDef columnDef)
	{
		return columnDef.DataType switch
		{
			"NVARCHAR" => "System.Data.DbType.String",
			"NVARCHAR(MAX)" => "System.Data.DbType.String",
			"BINARY" => "System.Data.DbType.Binary",
			"VARBINARY" => "System.Data.DbType.Binary",
			"VARBINARY(MAX)" => "System.Data.DbType.Binary",
			"BIGINT" => "System.Data.DbType.Int64",
			"INT" => "System.Data.DbType.Int32",
			"SMALLINT" => "System.Data.DbType.Int16",
			"TINYINT" => "System.Data.DbType.Byte",
			"BIT" => "System.Data.DbType.Boolean",
			"REAL" => "System.Data.DbType.Single",
			"FLOAT" => "System.Data.DbType.Double",
			"DECIMAL" => "System.Data.DbType.Decimal",
			"MONEY" => "System.Data.DbType.Currency",
			"SMALLMONEY" => "System.Data.DbType.Currency",
			"DATETIME" => "System.Data.DbType.DateTime",
			"DATETIME2" => "System.Data.DbType.DateTime2",
			"DATETIMEOFFSET" => "System.Data.DbType.DateTimeOffset",
			"SMALLDATETIME" => "System.Data.DbType.DateTime",
			"DATE" => "System.Data.DbType.Date",
			"TIME" => "System.Data.DbType.Time",
			"TIMESTAMP" => "System.Data.DbType.Binary",
			"UNIQUEIDENTIFIER" => "System.Data.DbType.Guid",
			_ => "System.Data.DbType.String",
		};
	}

	public static string GetSaveVariableName(this IColumnDef columnDef, bool camelCase = false)
	{
		if (string.IsNullOrEmpty(columnDef.ColumnName))
		{
			return string.Empty;
		}
		// Replace invalid characters with underscores
		var saveColumnName = columnDef.ColumnName!.Replace('\\', '_');
		saveColumnName = saveColumnName.Replace('/', '_');
		saveColumnName = saveColumnName.Replace('?', '_');
		saveColumnName = saveColumnName.Replace('[', '_');
		saveColumnName = saveColumnName.Replace(']', '_');
		saveColumnName = saveColumnName.Replace('-', '_');
		saveColumnName = saveColumnName.Replace('\'', '_');
		saveColumnName = saveColumnName.Replace(' ', '_');
		saveColumnName = saveColumnName.Replace('(', '_');
		saveColumnName = saveColumnName.Replace(')', '_');
		saveColumnName = saveColumnName.Replace("__", "_");
		// return if the column name is just an underscore
		if (saveColumnName == "_")
		{
			return saveColumnName;
		}
		// remove trailing underscores
		while (saveColumnName.Length > 0 && saveColumnName[saveColumnName.Length - 1] == '_')
		{
			saveColumnName = saveColumnName.Remove(saveColumnName.Length - 1);
		}
		if (saveColumnName[0] >= '0' && saveColumnName[0] <= '9')
		{
			saveColumnName = "p" + saveColumnName;
		}
		if (camelCase)
		{
			// Convert to camelCase
			saveColumnName = char.ToLowerInvariant(saveColumnName[0]) + saveColumnName.Substring(1);
		}
		else
		{
			// Convert to PascalCase
			saveColumnName = char.ToUpperInvariant(saveColumnName[0]) + saveColumnName.Substring(1);
		}
		return saveColumnName;

	}
}

using System.Collections.Generic;

namespace EZAccess.SourceGenerator.Abstractions.HelperClasses;

/// <summary>
/// This class is used to return a list of data with pagination information.
/// </summary>
/// <typeparam name="ModelT"></typeparam>
public class EZDataList<ModelT>
{
	public IEnumerable<ModelT>? Data { get; set; }
	public int PageNumber { get; set; }
	public int PageSize { get; set; }
	public int TotalCount { get; set; }
}

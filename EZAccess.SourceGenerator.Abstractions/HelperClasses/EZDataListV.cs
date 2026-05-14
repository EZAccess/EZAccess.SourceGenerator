using System.Collections.Generic;

namespace EZAccess.SourceGenerator.Abstractions.HelperClasses;

/// <summary>
/// This class is used to return a list of data with pagination information. 
/// This version is used for virtual pagination, where the client can request a specific range of data (start index and count) instead of page number and page size. 
/// </summary>
/// <typeparam name="ModelT"></typeparam>
public class EZDataListV<ModelT>
{
	public ICollection<ModelT>? Data { get; set; }
	public int StartIndex { get; set; }
	public int Count { get; set; }
	public int TotalCount { get; set; }
}

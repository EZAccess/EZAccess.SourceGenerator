using EZAccess.SourceGenerator.Abstractions.HelperClasses;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace EZAccess.SourceGenerator.Abstractions.BaseClasses;

public interface IBFFServiceBase<ModelT, CoreT, RequestT, KeyT> where ModelT : class where CoreT : class where RequestT : class
{
	Task<EZRestResponse<EZDataList<ModelT>?>> GetAllWhereAsync(
				RequestT request,
				string? customQuery = null,
				int? pageNumber = null,
				int? pageSize = null,
				string? sortBy = null,
				string? sortDirection = null);

	Task<EZRestResponse<EZDataListV<ModelT>?>> GetAllVirtualAsync(
				RequestT request,
				int startIndex,
				int count,
				CancellationToken cancellationToken);

	Task<EZRestResponse<EZDataList<ModelT>?>> PostQueryAsync(
				IEnumerable<EZWhereCriterium> where,
				int? pageNumber = null,
				int? pageSize = null,
				string? sortBy = null,
				string? sortDirection = null);

	Task<EZRestResponse<int>> GetCountWhereAsync(RequestT request);

	Task<EZRestResponse<ModelT?>> GetAsync(KeyT key);

	Task<EZRestResponse<ModelT?>> CreateAsync(CoreT newModel);

	Task<EZRestResponse<ModelT?>> UpdateAsync(KeyT key, CoreT updated);

	Task<EZRestResponse<bool>> DeleteAsync(KeyT key);

	Task<EZRestResponse<Dictionary<KeyT, string>?>> LookupWhereAsync(
				KeyT key, 
				string? value, 
				string? likeCriteria, 
				bool OrderByKey = false, 
				RequestT? request = null, 
				int limit = 100);

	Task<string> LookupAsync(KeyT key, bool fromCache);

}

public abstract class BFFServiceBase<ModelT, CoreT, RequestT, KeyT> : EZHttpService<BFFServiceBase<ModelT, CoreT, RequestT, KeyT>>, IBFFServiceBase<ModelT, CoreT, RequestT, KeyT> where ModelT : class where CoreT : class where RequestT : class
{
	protected readonly IMemoryCache _cache;

	protected BFFServiceBase(
		HttpClient httpClient, 
		ILogger<BFFServiceBase<ModelT, CoreT, RequestT, KeyT>> logger,
		IMemoryCache cache) : base(httpClient, logger)
	{
		_cache = cache;
	}

	protected abstract string RequestUri { get; }
	protected abstract string EntityName { get; }
	protected abstract string GetQueryForModel(RequestT request);
	protected abstract string GetQueryForKey(KeyT key, string seperator = "/", bool includeFieldname = false);

	public virtual async Task<EZRestResponse<EZDataList<ModelT>?>> GetAllWhereAsync(RequestT request, string? customQuery = null, int? pageNumber = null, int? pageSize = null, string? sortBy = null, string? sortDirection = null)
	{
		var query = GetQueryForModel(request);
		var uri = RequestUri;
		if (customQuery is not null) {
			query += $"&{customQuery}";
			uri += "/customQuery";
		}
		if(pageNumber is not null && pageSize is not null) {
			query += $"&pageNumber={pageNumber}&pageSize={pageSize}";
		}
		if (sortBy is not null) {
			query += $"&sortBy={sortBy}";
		}
		if (!string.IsNullOrEmpty(sortDirection)) { 
			query += $"&sortDirection={sortDirection}";
		}

		return string.IsNullOrEmpty(query)
			? await HttpGetAsync<EZDataList<ModelT>?>(uri)
			: await HttpGetAsync<EZDataList<ModelT>?>($"{uri}?{query.Substring(1)}");
	}

	public virtual async Task<EZRestResponse<EZDataListV<ModelT>?>> GetAllVirtualAsync(RequestT request, int startIndex, int count, CancellationToken cancellationToken)
	{
		var query = GetQueryForModel(request);
		var uri = $"{RequestUri}/virtual";
		query += $"&startindex={startIndex}&count={count}";
		return await HttpGetAsync<EZDataListV<ModelT>?>($"{uri}?{query.Substring(1)}", cancellationToken);
	}

	public virtual async Task<EZRestResponse<EZDataList<ModelT>?>> PostQueryAsync(IEnumerable<EZWhereCriterium> where, int? pageNumber = null, int? pageSize = null, string? sortBy = null, string? sortDirection = null)
	{
		var query = string.Empty;
		var uri = $"{RequestUri}/query";
		if (pageNumber is not null && pageSize is not null) {
			query += $"&pagenumber={pageNumber}&pagesize={pageSize}";
		}
		if (sortBy is not null) {
			query += $"&sortby={sortBy}";
		}
		if (sortDirection is not null) {
			query += $"&sortdirection={sortDirection}";
		}

		return string.IsNullOrEmpty(query)
			? await HttpPostAsync<EZDataList<ModelT>?, IEnumerable<EZWhereCriterium>>(uri, where)
			: await HttpPostAsync<EZDataList<ModelT>?, IEnumerable<EZWhereCriterium>>($"{uri}?{query.Substring(1)}", where);
	}

	public virtual async Task<EZRestResponse<int>> GetCountWhereAsync(RequestT request)
	{
		var query = GetQueryForModel(request);
		query += "&getCountOnly=true";

		var result = await HttpGetAsync<EZDataList<ModelT>?>($"{RequestUri}?{query.Substring(1)}");
		if (result.IsSuccess) {
			return new EZRestResponse<int> {
				Content = result.Content?.TotalCount ?? 0
			};
		}
		else {
			return new EZRestResponse<int> {
				ErrorMessage = result.ErrorMessage,
				StatusCode = result.StatusCode
			};
		}
	}

	public virtual async Task<EZRestResponse<ModelT?>> GetAsync(KeyT key)
	{
		return await HttpGetAsync<ModelT?>($"{RequestUri}{GetQueryForKey(key)}");
	}

	public virtual async Task<EZRestResponse<ModelT?>> CreateAsync(CoreT newModel)
	{
		return await HttpPostAsync<ModelT?, CoreT?>(RequestUri, newModel);
	}

	public virtual async Task<EZRestResponse<ModelT?>> UpdateAsync(KeyT key, CoreT updated)
	{
		return await HttpPutAsync<ModelT?, CoreT?>($"{RequestUri}{GetQueryForKey(key)}", updated);
	}

	public virtual async Task<EZRestResponse<bool>> DeleteAsync(KeyT key)
	{
		return await HttpDeleteAsync<bool>($"{RequestUri}{GetQueryForKey(key)}");
	}

	public virtual async Task<EZRestResponse<Dictionary<KeyT, string>?>> LookupWhereAsync(KeyT key, string? value, string? likeCriteria, bool OrderByKey = false, RequestT? request = null, int limit = 100)
	{
		// Build a query from the properties that are not null to be send as query parameters in the URL
		string query = string.Empty;
		if (request is not null) {
			query = GetQueryForModel(request);
		}
		if (key is not null) {
			query += $"&key={GetQueryForKey(key, ";")}";
		}
		if (value is not null) {
			query += $"&value={value}";
		}
		if (likeCriteria is not null) {
			query += $"&likecriteria={likeCriteria}";
		}
		if (OrderByKey) {
			query += $"&orderbykey=true";
		}
		query += $"&limit={limit}";

		return string.IsNullOrEmpty(query)
			? await HttpGetAsync<Dictionary<KeyT, string>?>(RequestUri + "/lookup")
			: await HttpGetAsync<Dictionary<KeyT, string>?>($"{RequestUri}/lookup?{query.Substring(1)}");
	}

	public virtual async Task<string> LookupAsync(KeyT key, bool fromCache)
	{
		if (fromCache) {
			return await LookupFromCacheAsync(key);
		}

		var response =
			await HttpGetAsync<Dictionary<KeyT, string>?>($"{RequestUri}/lookup?{GetQueryForKey(key, "&", true).Substring(1)}");
		if (response.IsError) {
			return $"#HttpError: {response.StatusCode}#";
		}
		// The lookup is by id, thus there should only be one item returned
		return response.Content?.Values.FirstOrDefault() ?? $"#Error: {GetQueryForKey(key).Substring(1)} not found#";
	}

	/// <summary>
	/// When a list is small and constant it is useful to cache the lookup values in memory. 
	/// </summary>
	private async Task<string> LookupFromCacheAsync(KeyT? key)
	{
		if (key is null) return string.Empty;
		string cacheKey = $"cacheKeyLookup{EntityName}";
		if (_cache.TryGetValue(cacheKey, out Dictionary<KeyT, string>? lookupList)) {
			_logger.LogTrace("LookupFromCacheAsync: cache retrieved for {entityName}", EntityName);
		}
		else {
			var result = await HttpGetAsync<Dictionary<KeyT, string>?>($"{RequestUri}/lookup");
			if (result.IsSuccess) {
				lookupList = result.Content;

				var cacheEntryOptions = new MemoryCacheEntryOptions()
					.SetSlidingExpiration(TimeSpan.FromMinutes(10))
					.SetAbsoluteExpiration(TimeSpan.FromMinutes(30))
					.SetPriority(CacheItemPriority.Normal);
				_cache.Set(cacheKey, lookupList, cacheEntryOptions);
				_logger.LogTrace("LookupFromCacheAsync: cache created for {entityName}", EntityName);
			}
			else {
				_logger.LogError("LookupFromCacheAsync: {result.ErrorMessage}", result.ErrorMessage);
				return $"#HttpError: {result.StatusCode}#";
			}
		}
		if (lookupList is null)
			return "#Error: lookupList is null#";
		if (!lookupList.TryGetValue(key, out string? description))
			return $"#Error: {GetQueryForKey(key)} not found#";
		return description;
	}
}

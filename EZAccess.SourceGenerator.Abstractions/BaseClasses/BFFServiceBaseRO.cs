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

public interface IBFFServiceBaseRO<ModelT, CoreT, RequestT, KeyT> where ModelT : class where CoreT : class where RequestT : class
{
	Task<EZRestResponse2<EZDataList<ModelT>?>> GetAllWhereAsync(
				RequestT request,
				string? customQuery = null,
				int? pageNumber = null,
				int? pageSize = null,
				string? sortBy = null,
				string? sortDirection = null);

	Task<EZRestResponse2<EZDataListV<ModelT>?>> GetAllVirtualAsync(
				RequestT request,
				int startIndex,
				int count,
				CancellationToken cancellationToken);

	Task<EZRestResponse2<EZDataList<ModelT>?>> PostQueryAsync(
				IEnumerable<EZWhereCriterium> where,
				int? pageNumber = null,
				int? pageSize = null,
				string? sortBy = null,
				string? sortDirection = null);

	Task<EZRestResponse2<int>> GetCountWhereAsync(RequestT request);

	Task<EZRestResponse2<ModelT?>> GetAsync(KeyT key);

	Task<EZRestResponse2<Dictionary<KeyT, string>?>> LookupWhereAsync(
				KeyT key, 
				string? value, 
				string? likeCriteria, 
				bool OrderByKey = false, 
				RequestT? request = null, 
				int limit = 100);

	Task<string> LookupAsync(KeyT key, bool fromCache);

}

public abstract class BFFServiceBaseRO<ModelT, CoreT, RequestT, KeyT> : EZHttpService<BFFServiceBase<ModelT, CoreT, RequestT, KeyT>>, IBFFServiceBaseRO<ModelT, CoreT, RequestT, KeyT> where ModelT : class where CoreT : class where RequestT : class
{
	protected readonly IMemoryCache _cache;

	protected BFFServiceBaseRO(
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

	public virtual async Task<EZRestResponse2<EZDataList<ModelT>?>> GetAllWhereAsync(RequestT request, string? customQuery = null, int? pageNumber = null, int? pageSize = null, string? sortBy = null, string? sortDirection = null)
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

	public virtual async Task<EZRestResponse2<EZDataListV<ModelT>?>> GetAllVirtualAsync(RequestT request, int startIndex, int count, CancellationToken cancellationToken)
	{
		var query = GetQueryForModel(request);
		var uri = $"{RequestUri}/virtual";
		query += $"&startindex={startIndex}&count={count}";
		return await HttpGetAsync<EZDataListV<ModelT>?>($"{uri}?{query.Substring(1)}", cancellationToken);
	}

	public virtual async Task<EZRestResponse2<EZDataList<ModelT>?>> PostQueryAsync(IEnumerable<EZWhereCriterium> where, int? pageNumber = null, int? pageSize = null, string? sortBy = null, string? sortDirection = null)
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

	public virtual async Task<EZRestResponse2<int>> GetCountWhereAsync(RequestT request)
	{
		var query = GetQueryForModel(request);
		query += "&getCountOnly=true";

		var result = await HttpGetAsync<EZDataList<ModelT>?>($"{RequestUri}?{query.Substring(1)}");
		if (result.IsSuccess) {
			return new EZRestResponse2<int> {
				Content = result.Content?.TotalCount ?? 0
			};
		}
		else {
			return new EZRestResponse2<int> {
				ErrorMessage = result.ErrorMessage,
				StatusCode = result.StatusCode
			};
		}
	}

	public virtual async Task<EZRestResponse2<ModelT?>> GetAsync(KeyT key)
	{
		return await HttpGetAsync<ModelT?>($"{RequestUri}{GetQueryForKey(key)}");
	}

	public virtual async Task<EZRestResponse2<Dictionary<KeyT, string>?>> LookupWhereAsync(KeyT key, string? value, string? likeCriteria, bool OrderByKey = false, RequestT? request = null, int limit = 100)
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

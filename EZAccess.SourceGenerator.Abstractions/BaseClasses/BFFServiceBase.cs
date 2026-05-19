using EZAccess.SourceGenerator.Abstractions.HelperClasses;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;

namespace EZAccess.SourceGenerator.Abstractions.BaseClasses;

public interface IBFFServiceBase<ModelT, CoreT, RequestT, KeyT> : IBFFServiceBaseRO<ModelT, CoreT, RequestT, KeyT>
	where ModelT : class where CoreT : class where RequestT : class
{

	Task<EZRestResponse2<ModelT?>> CreateAsync(CoreT newModel);

	Task<EZRestResponse2<ModelT?>> UpdateAsync(KeyT key, CoreT updated);

	Task<EZRestResponse2<bool>> DeleteAsync(KeyT key);

}

public abstract class BFFServiceBase<ModelT, CoreT, RequestT, KeyT> : BFFServiceBaseRO<ModelT, CoreT, RequestT, KeyT>, IBFFServiceBase<ModelT, CoreT, RequestT, KeyT> where ModelT : class where CoreT : class where RequestT : class
{

	protected BFFServiceBase(
		HttpClient httpClient, 
		ILogger<BFFServiceBase<ModelT, CoreT, RequestT, KeyT>> logger,
		IMemoryCache cache) : base(httpClient, logger, cache)
	{
	}

	public virtual async Task<EZRestResponse2<ModelT?>> CreateAsync(CoreT newModel)
	{
		return await HttpPostAsync<ModelT?, CoreT?>(RequestUri, newModel);
	}

	public virtual async Task<EZRestResponse2<ModelT?>> UpdateAsync(KeyT key, CoreT updated)
	{
		return await HttpPutAsync<ModelT?, CoreT?>($"{RequestUri}{GetQueryForKey(key)}", updated);
	}

	public virtual async Task<EZRestResponse2<bool>> DeleteAsync(KeyT key)
	{
		return await HttpDeleteAsync<bool>($"{RequestUri}{GetQueryForKey(key)}");
	}

}

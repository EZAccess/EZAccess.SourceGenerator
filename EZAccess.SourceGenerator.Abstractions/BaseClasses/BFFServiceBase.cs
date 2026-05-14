using EZAccess.SourceGenerator.Abstractions.HelperClasses;
using System.Collections.Generic;
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

	ModelT? CloneAanvraag(ModelT? original);
}

public abstract class BFFServiceBase<ModelT, CoreT, RequestT, KeyT> : IBFFServiceBase<ModelT, CoreT, RequestT, KeyT> where ModelT : class where CoreT : class where RequestT : class
{
	public virtual ModelT? CloneAanvraag(ModelT? original)
	{
		throw new System.NotImplementedException();
	}

	public virtual Task<EZRestResponse<ModelT?>> CreateAsync(CoreT newModel)
	{
		throw new System.NotImplementedException();
	}

	public virtual Task<EZRestResponse<bool>> DeleteAsync(KeyT key)
	{
		throw new System.NotImplementedException();
	}

	public virtual Task<EZRestResponse<EZDataListV<ModelT>?>> GetAllVirtualAsync(RequestT request, int startIndex, int count, CancellationToken cancellationToken)
	{
		throw new System.NotImplementedException();
	}

	public virtual Task<EZRestResponse<EZDataList<ModelT>?>> GetAllWhereAsync(RequestT request, string? customQuery = null, int? pageNumber = null, int? pageSize = null, string? sortBy = null, string? sortDirection = null)
	{
		throw new System.NotImplementedException();
	}

	public virtual Task<EZRestResponse<ModelT?>> GetAsync(KeyT key)
	{
		throw new System.NotImplementedException();
	}

	public virtual Task<EZRestResponse<int>> GetCountWhereAsync(RequestT request)
	{
		throw new System.NotImplementedException();
	}

	public virtual Task<string> LookupAsync(KeyT key, bool fromCache)
	{
		throw new System.NotImplementedException();
	}

	public virtual Task<EZRestResponse<Dictionary<KeyT, string>?>> LookupWhereAsync(KeyT key, string? value, string? likeCriteria, bool OrderByKey = false, RequestT? request = null, int limit = 100)
	{
		throw new System.NotImplementedException();
	}

	public virtual Task<EZRestResponse<EZDataList<ModelT>?>> PostQueryAsync(IEnumerable<EZWhereCriterium> where, int? pageNumber = null, int? pageSize = null, string? sortBy = null, string? sortDirection = null)
	{
		throw new System.NotImplementedException();
	}

	public virtual Task<EZRestResponse<ModelT?>> UpdateAsync(KeyT key, CoreT updated)
	{
		throw new System.NotImplementedException();
	}
}

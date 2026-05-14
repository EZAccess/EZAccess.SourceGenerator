using System.Collections.Generic;
using System.Net;

namespace EZAccess.SourceGenerator.Abstractions.HelperClasses;

public class EZRestResponse<TModel> 
{
	private string? _errorMessage;
	private TModel _content;

	public TModel Content { 
		get { return _content; }
		set { 
			_content = value; 
			IsSuccess = true;
		}
	}

	public Dictionary<string, List<string>>? ValidationErrors { get; set; }
	public HttpStatusCode StatusCode { get; set; }
	public bool IsSuccess { get; set; } = true;
	public bool IsError => !IsSuccess;

	public string? ErrorMessage
	{
		get { return _errorMessage; }
		set { 
			IsSuccess = false;
			_errorMessage = value; 
		}
	}

	public EZRestResponse()
	{
		_content = default!;
		IsSuccess = false;
	}

	public EZRestResponse(TModel content)
	{
		_content = content;
		IsSuccess = true;
	}

}
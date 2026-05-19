using System.Collections.Generic;
using System.Net;

namespace EZAccess.SourceGenerator.Abstractions.HelperClasses;

public class EZRestResponse2<TModel> 
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

	public EZRestResponse2()
	{
		_content = default!;
		IsSuccess = false;
	}

	public EZRestResponse2(TModel content)
	{
		_content = content;
		IsSuccess = true;
	}

}
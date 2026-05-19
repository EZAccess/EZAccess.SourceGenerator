using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace EZAccess.SourceGenerator.Abstractions.HelperClasses;

public abstract class EZHttpService<Tclass> where Tclass : class
{
	readonly protected HttpClient httpClient;
	readonly protected ILogger<Tclass> _logger;

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "Primary constructors are not a replacement for DI!!! Microsoft stop this bullshit!")]
	public EZHttpService(HttpClient httpClient, ILogger<Tclass> logger)
	{
		this.httpClient = httpClient;
		_logger = logger;
	}

	async protected Task<EZRestResponse2<T>> HttpGetAsync<T>(string requestUri)
	{
		_logger.LogInformation("Requesting data from server: {requestUri}", requestUri);
		return await HttpRequestAsync<T>(httpClient.GetAsync(requestUri), requestUri, _logger);
	}

	async protected Task<EZRestResponse2<T>> HttpGetAsync<T>(string requestUri, CancellationToken cancellationToken)
	{
		_logger.LogInformation("Requesting data from server: {requestUri}", requestUri);
		return await HttpRequestAsync<T>(httpClient.GetAsync(requestUri, cancellationToken), requestUri, _logger);
	}

	async protected Task<EZRestResponse2<T>> HttpPostAsync<T>(string requestUri, T newT)
	{
		_logger.LogInformation("Posting data to server: {requestUri}", requestUri);
		return await HttpRequestAsync<T>(httpClient.PostAsJsonAsync(requestUri, newT), requestUri, _logger);
	}

	async protected Task<EZRestResponse2<TResponse>> HttpPostAsync<TResponse, TInput>(string requestUri, TInput newT)
	{
		_logger.LogInformation("Posting data to server: {requestUri}", requestUri);
		return await HttpRequestAsync<TResponse>(httpClient.PostAsJsonAsync(requestUri, newT), requestUri, _logger);
	}

	async protected Task<EZRestResponse2<T>> HttpPostFileAsync<T>(string requestUri, Stream fileStream, string fileName, string? contentType = null)
	{
		_logger.LogInformation("Posting file to server: {requestUri}", requestUri);
		try {
			var streamContent = new StreamContent(fileStream);

			if (!string.IsNullOrWhiteSpace(contentType)) {
				streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
			}
			else {
				// Fallback to a safe generic binary content type when not provided
				streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
			}

			var content = new MultipartFormDataContent {
				{ streamContent, "file", fileName }
			};

			return await HttpRequestAsync<T>(httpClient.PostAsync(requestUri, content), requestUri, _logger);
		}
		catch (Exception ex) {
			Debug.Print(ex.Message);
			throw;
		}
	}

	async protected Task<EZRestResponse2<Tresponse>> HttpActionAsync<Tresponse>(string requestUri, JsonElement request)
	{
		_logger.LogInformation("Posting data to server: {requestUri}", requestUri);
		return await HttpRequestAsync<Tresponse>(httpClient.PostAsJsonAsync(requestUri, request), requestUri, _logger);
	}

	async protected Task<EZRestResponse2<Tresponse>> HttpActionAsync<Tresponse>(string requestUri)
	{
		_logger.LogInformation("Posting data to server: {requestUri}", requestUri);
		return await HttpRequestAsync<Tresponse>(httpClient.PostAsJsonAsync(requestUri, string.Empty), requestUri, _logger);
	}

	async protected Task<EZRestResponse2<T>> HttpPutAsync<T>(string requestUri, T updatedT)
	{
		_logger.LogInformation("Updating data on server: {requestUri}", requestUri);
		return await HttpRequestAsync<T>(httpClient.PutAsJsonAsync(requestUri, updatedT), requestUri, _logger);
	}

	async protected Task<EZRestResponse2<TResponse>> HttpPutAsync<TResponse, TInput>(string requestUri, TInput updatedT)
	{
		_logger.LogInformation("Updating data on server: {requestUri}", requestUri);
		return await HttpRequestAsync<TResponse>(httpClient.PutAsJsonAsync(requestUri, updatedT), requestUri, _logger);
	}

	async protected Task<EZRestResponse2<TResponse>> HttpDeleteAsync<TResponse>(string requestUri)
	{
		_logger.LogInformation("Deleting data on server: {requestUri}", requestUri);
		return await HttpRequestAsync<TResponse>(httpClient.DeleteAsync(requestUri), requestUri, _logger);
	}

	private static async Task<EZRestResponse2<T>> HttpRequestAsync<T>(Task<HttpResponseMessage> httpRequest, string requestUri, ILogger logger)
	{
		var response = new EZRestResponse2<T>();
		try
		{
			var result = await httpRequest;
			try
			{
				response.StatusCode = result.StatusCode;
				result.EnsureSuccessStatusCode();
				if (typeof(T) == typeof(MemoryStream))
				{
					response.Content = (T)(object)await result.Content.ReadAsStreamAsync();
				}
				else if (typeof(T) == typeof(string))
				{
					response.Content = (T)(object)await result.Content.ReadAsStringAsync();
				}
				else
				{
					var resultObject = await result.Content.ReadFromJsonAsync<T>();
					response.Content = resultObject ?? default!;
				}
			}
			catch (HttpRequestException ex)
			{
				logger.LogWarning("An error returned on the {requestMethod} request to the server. \n\tRequest: {requestUri} \n\tMessage: {ex.Message}", 
									httpRequest.Result.RequestMessage?.Method.ToString(), 
									requestUri, 
									ex.Message);
				response.ErrorMessage = "An error occured while requesting data from the server. \nRequest: " + requestUri + "\n" + ex.Message;
				var responseBody = await result.Content.ReadAsStringAsync();
				var options = new JsonSerializerOptions
				{
					PropertyNameCaseInsensitive = true, // Makes the deserialization process case-insensitive
					PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // Use camelCase for matching JSON property names
				};
				//var errorResponse2 = JsonSerializer.Deserialize<ErrorResponse>(responseBody, options);

				try
				{
					//response.ValidationErrors = await result.Content.ReadFromJsonAsync<Dictionary<string, List<string>>>();
					response.ValidationErrors = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(responseBody, options);
				}
				catch (Exception )
				{
					try
					{
						//var errorResponse = await result.Content.ReadFromJsonAsync<ErrorResponse>();
						var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseBody, options);
						var errorMessage = "An error occured while requesting data from the server. \nRequest: " + requestUri + "\n" + ex.Message + "\nDetails:";
						if (errorResponse is not null && errorResponse.Errors is not null && errorResponse.Errors.Count > 0)
						{
							foreach (var error in errorResponse.Errors)
							{
								errorMessage += "\n" + error.Key + ": " + string.Join(", ", error.Value);
							}
						}
						response.ErrorMessage = errorMessage;
					}
					catch (Exception)
					{
						response.ErrorMessage = "An error occured while requesting data from the server. \nRequest: " + requestUri + "\n" + ex.Message + "\nDetails:\n" + responseBody;
					}
				}
			}
			catch (Exception ex)
			{
				response.ErrorMessage = "Unhandled exception: " + ex.Message;
			}
		}
		catch (OperationCanceledException) {
			// This is expected behavior when requests are cancelled (e.g., QuickGrid virtualization, user navigation)
			logger.LogDebug("Request was cancelled: {requestUri}. This is normal for virtualized grids or navigation changes.", requestUri);
			// Don't set ErrorMessage - this allows the caller to distinguish between cancellation and actual errors
			// The response will have IsSuccess = false by default, but no error message
			response.StatusCode = System.Net.HttpStatusCode.RequestTimeout; // Or leave as default (0)
		}
		catch (Exception ex)
		{
			response.ErrorMessage = "Unhandled exception: " + ex.Message;
		}
		return response;
	}


	private class ErrorResponse
	{
		public string? Type { get; set; }
		public string? Title { get; set; }
		public int Status { get; set; }
		public Dictionary<string, string[]>? Errors { get; set; }
		public string? TraceId { get; set; }
	}
}
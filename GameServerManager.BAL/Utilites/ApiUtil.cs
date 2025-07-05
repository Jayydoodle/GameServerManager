using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GameServerManager.BAL
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly JsonSerializerOptions _jsonOptions;

        /// <summary>
        /// Initializes a new instance of the ApiClient class.
        /// </summary>
        /// <param name="baseUrl">The base URL of the API.</param>
        /// <param name="timeout">Optional timeout in seconds (default: 30).</param>
        /// <param name="defaultHeaders">Optional default headers to include with every request.</param>
        public ApiClient(string baseUrl, int timeout = 30, Dictionary<string, string> defaultHeaders = null)
        {
            _baseUrl = baseUrl.TrimEnd('/');
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(timeout)
            };

            // Set default headers if provided
            if (defaultHeaders != null)
            {
                foreach (var header in defaultHeaders)
                {
                    _httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }

            // Configure JSON serializer options
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };
        }

        /// <summary>
        /// Tells the client to use the basic authentication scheme for the request
        /// </summary>
        /// <param name="username">The username for authenticationh</param>
        /// <param name="password">The password for authentication</param>
        public void UseBasicAuthentication(string username, string password)
        {
            var byteArray = Encoding.ASCII.GetBytes($"{username}:{password}");
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        }

        /// <summary>
        /// Performs a GET request to the specified endpoint.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the response to.</typeparam>
        /// <param name="endpoint">The API endpoint (will be appended to the base URL).</param>
        /// <param name="queryParams">Optional query parameters.</param>
        /// <returns>The deserialized response object.</returns>
        public async Task<T> GetAsync<T>(string endpoint, Dictionary<string, string> queryParams = null)
        {
            string url = BuildUrl(endpoint, queryParams);
            HttpResponseMessage response = await _httpClient.GetAsync(url);
            return await ProcessResponseAsync<T>(response);
        }

        /// <summary>
        /// Performs a POST request to the specified endpoint.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the response to.</typeparam>
        /// <param name="endpoint">The API endpoint (will be appended to the base URL).</param>
        /// <param name="data">The data to send in the request body.</param>
        /// <param name="queryParams">Optional query parameters.</param>
        /// <returns>The deserialized response object.</returns>
        public async Task<T> PostAsync<T>(string endpoint, object data, Dictionary<string, string> queryParams = null)
        {
            string url = BuildUrl(endpoint, queryParams);
            string json = JsonSerializer.Serialize(data, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(url, content);
            return await ProcessResponseAsync<T>(response);
        }

        /// <summary>
        /// Performs a PUT request to the specified endpoint.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the response to.</typeparam>
        /// <param name="endpoint">The API endpoint (will be appended to the base URL).</param>
        /// <param name="data">The data to send in the request body.</param>
        /// <param name="queryParams">Optional query parameters.</param>
        /// <returns>The deserialized response object.</returns>
        public async Task<T> PutAsync<T>(string endpoint, object data, Dictionary<string, string> queryParams = null)
        {
            string url = BuildUrl(endpoint, queryParams);
            string json = JsonSerializer.Serialize(data, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PutAsync(url, content);
            return await ProcessResponseAsync<T>(response);
        }

        /// <summary>
        /// Performs a PATCH request to the specified endpoint.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the response to.</typeparam>
        /// <param name="endpoint">The API endpoint (will be appended to the base URL).</param>
        /// <param name="data">The data to send in the request body.</param>
        /// <param name="queryParams">Optional query parameters.</param>
        /// <returns>The deserialized response object.</returns>
        public async Task<T> PatchAsync<T>(string endpoint, object data, Dictionary<string, string> queryParams = null)
        {
            string url = BuildUrl(endpoint, queryParams);
            string json = JsonSerializer.Serialize(data, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(new HttpMethod("PATCH"), url)
            {
                Content = content
            };

            HttpResponseMessage response = await _httpClient.SendAsync(request);
            return await ProcessResponseAsync<T>(response);
        }

        /// <summary>
        /// Performs a DELETE request to the specified endpoint.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the response to.</typeparam>
        /// <param name="endpoint">The API endpoint (will be appended to the base URL).</param>
        /// <param name="queryParams">Optional query parameters.</param>
        /// <returns>The deserialized response object.</returns>
        public async Task<T> DeleteAsync<T>(string endpoint, Dictionary<string, string> queryParams = null)
        {
            string url = BuildUrl(endpoint, queryParams);
            HttpResponseMessage response = await _httpClient.DeleteAsync(url);
            return await ProcessResponseAsync<T>(response);
        }

        /// <summary>
        /// Builds the full URL from the endpoint and query parameters.
        /// </summary>
        private string BuildUrl(string endpoint, Dictionary<string, string> queryParams)
        {
            endpoint = endpoint.TrimStart('/');
            string url = $"{_baseUrl}/{endpoint}";

            if (queryParams != null && queryParams.Count > 0)
            {
                var queryString = string.Join("&", queryParams.Select(p => $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value)}"));
                url = $"{url}?{queryString}";
            }

            return url;
        }

        /// <summary>
        /// Processes the HTTP response and deserializes it to the specified type.
        /// </summary>
        private async Task<T> ProcessResponseAsync<T>(HttpResponseMessage response)
        {
            string content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApiException(
                    $"API request failed with status code {(int)response.StatusCode}: {response.ReasonPhrase}",
                    (int)response.StatusCode,
                    content);
            }

            if (typeof(T) == typeof(string))
            {
                return (T)(object)content;
            }

            try
            {
                return JsonSerializer.Deserialize<T>(content, _jsonOptions);
            }
            catch (JsonException ex)
            {
                throw new ApiException("Failed to deserialize response", (int)response.StatusCode, content, ex);
            }
        }
    }

    /// <summary>
    /// Exception thrown when an API request fails.
    /// </summary>
    public class ApiException : Exception
    {
        public int StatusCode { get; }
        public string ResponseContent { get; }

        public ApiException(string message, int statusCode, string responseContent, Exception innerException = null)
            : base(message, innerException)
        {
            StatusCode = statusCode;
            ResponseContent = responseContent;
        }
    }
}

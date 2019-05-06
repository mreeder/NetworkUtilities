using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NetworkUtilitiesAPI.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace NetworkUtilitiesAPI.HttpClients
{
    public class GeoIPClient : IGeoIPClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<GeoIPClient> _logger;

        public GeoIPClient(HttpClient client, IConfiguration config, ILogger<GeoIPClient> logger)
        {
            _logger = logger;
            _client = client;
            _client.BaseAddress = new Uri(config["ApiEndpoints:GeoIp"]);    //TODO: Handle missing/misconfiguration exceptions
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.AcceptEncoding.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("gzip"));
        }

        public async Task<ApiResult> GetIPDetailsAsync(IPAddress ipAddress)
        {
            ApiResult apiResult;

            try
            {
                HttpResponseMessage response = await _client.GetAsync("ip/" + ipAddress.ToString());

                apiResult = await ParseResponse(response);

                return apiResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred calling API for '{ipAddress}'", ipAddress.ToString());

                apiResult = new ApiResult()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    ErrorMessage = "Exception encountered"
                };

                return apiResult;
            }

        }

        public async Task<ApiResult> GetIPDetailsAsync(string domain)
        {
            ApiResult apiResult;

            try
            {
                HttpResponseMessage response = await _client.GetAsync("domain/" + domain);

                apiResult = await ParseResponse(response);

                return apiResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred calling API for '{domain}'", domain);

                apiResult = new ApiResult()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    ErrorMessage = "Exception encountered"
                };

                return apiResult;
            }

        }

        private async Task<ApiResult> ParseResponse(HttpResponseMessage response)
        {
            ApiResult apiResult;

            if (response.IsSuccessStatusCode)
            {
                string resultContent = await response.Content.ReadAsStringAsync();

                apiResult = new ApiResult()
                {
                    StatusCode = response.StatusCode,
                    Result = JObject.Parse(resultContent)
                };

                return apiResult;
            }
            else
            {
                if (response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.BadRequest)
                {
                    apiResult = new ApiResult()
                    {
                        StatusCode = response.StatusCode,
                        ErrorMessage = response.ReasonPhrase
                    };
                }
                else
                {
                    apiResult = new ApiResult()
                    {
                        StatusCode = HttpStatusCode.InternalServerError,
                        ErrorMessage = "Error performing lookup"
                    };

                    _logger.LogWarning("Failed lookup for: {path}. StatusCode: {statusCode}; Reason: {reasonPhrase}", response.RequestMessage.RequestUri.PathAndQuery, response.StatusCode.ToString(), response.ReasonPhrase);
                }

                return apiResult;
            }
        }
    }
}

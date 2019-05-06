using GeoIPWorkerAPI.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace GeoIPWorkerAPI.HttpClients
{
    public class Client
    {
        private readonly ILogger _logger;
        private readonly HttpClient _client;
        private readonly string _apikey;

        public Client(ILogger<Client> logger, HttpClient client, IConfiguration config)
        {
            _logger = logger;
            _apikey = config["APIKeys:ipstack"];
            _client = client;
            _client.BaseAddress = new Uri("http://api.ipstack.com/");
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.AcceptEncoding.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("gzip"));
        }

        public async Task<ApiResult> GetIPDetailsAsync(IPAddress ipAddress)
        {
            string ipAddressString = ipAddress.ToString();
            return await CallApiAsync(ipAddressString);
        }

        internal async Task<ApiResult> GetIPDetailsAsync(string domain)
        {
            return await CallApiAsync(domain);
        }

        private async Task<ApiResult> CallApiAsync(string lookup)
        {
            ApiResult apiResult;

            string accessKey = $"?access_key={_apikey}";

            try
            {
                HttpResponseMessage response = await _client.GetAsync(lookup + accessKey + "&fields=main");

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
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        apiResult = new ApiResult()
                        {
                            StatusCode = HttpStatusCode.NotFound,
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

                        _logger.LogWarning("Failed lookup for: {lookup}. StatusCode: {statusCode}; Reason: {reasonPhrase}", lookup, response.StatusCode.ToString(), response.ReasonPhrase);
                    }
                    
                    return apiResult;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred calling external API for '{lookup}'", lookup);

                apiResult = new ApiResult()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    ErrorMessage = "Exception encountered"
                };

                return apiResult;
            }
        }
    }
}

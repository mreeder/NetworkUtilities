using RDAPWorkerAPI.Models;
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

namespace RDAPWorkerAPI.HttpClients
{
    public class Client
    {
        private readonly ILogger _logger;
        private readonly HttpClient _client;

        public Client(ILogger<Client> logger, HttpClient client)
        {
            _logger = logger;
            _client = client;
            _client.BaseAddress = new Uri("https://www.rdap.net/");
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.AcceptEncoding.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("gzip"));
        }

        public async Task<ApiResult> GetIPDetailsAsync(IPAddress ipAddress)
        {
            string ipAddressString = ipAddress.ToString();
            string endpoint = "ip/";
            return await CallApiAsync(ipAddressString, endpoint);
        }

        internal async Task<ApiResult> GetIPDetailsAsync(string domain)
        {
            string endpont = "domain/";
            return await CallApiAsync(domain, endpont);
        }

        private async Task<ApiResult> CallApiAsync(string lookup, string endpoint)
        {
            ApiResult apiResult;

            try
            {
                HttpResponseMessage response = await _client.GetAsync(endpoint + lookup);

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

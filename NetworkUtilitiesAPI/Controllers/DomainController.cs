using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetworkUtilitiesAPI.HttpClients;
using NetworkUtilitiesAPI.Models;
using Newtonsoft.Json.Linq;

namespace NetworkUtilitiesAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class DomainController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IGeoIPClient _geoIPClient;
        private readonly IRDAPClient _rdapClient;

        public DomainController(ILogger<DomainController> logger, IGeoIPClient geoIPClient, IRDAPClient rdapClient)
        {
            _logger = logger;
            _geoIPClient = geoIPClient;
            _rdapClient = rdapClient;
        }

        //GET api/domain/google.com
        [HttpGet("{domain}")]
        public async Task<ActionResult<string>> GetAsync(string domain, [FromQuery] string serviceFilter)
        {
            //TODO: BETTER DOMAIN NAME VALIDATION
            if (Uri.CheckHostName(domain) == UriHostNameType.Dns)
            {
                JObject report = new JObject();

                if (string.IsNullOrWhiteSpace(serviceFilter) || serviceFilter.ToLowerInvariant().Contains("geoip"))
                {
                    ApiResult geoIpResults = await _geoIPClient.GetIPDetailsAsync(domain);

                    if (geoIpResults.IsSuccessStatusCode)
                    {
                        JObject result = new JObject();
                        result.Add("statusCode", (int)geoIpResults.StatusCode);
                        result.Add("results", geoIpResults.Result);
                        report.Add("geoIp", result);
                    }
                    else
                    {
                        JObject error = new JObject();
                        error.Add("statusCode", (int)geoIpResults.StatusCode);
                        error.Add("errorMessage", geoIpResults.ErrorMessage);
                        report.Add("geoIp", error);
                    }
                }

                if (String.IsNullOrWhiteSpace(serviceFilter) || serviceFilter.ToLowerInvariant().Contains("rdap"))
                {
                    ApiResult rdapResults = await _rdapClient.GetIpDetailsAsync(domain);

                    if (rdapResults.IsSuccessStatusCode)
                    {
                        JObject result = new JObject();
                        result.Add("statusCode", (int)rdapResults.StatusCode);
                        result.Add("results", rdapResults.Result);
                        report.Add("rdap", result);
                    }
                    else
                    {
                        JObject error = new JObject();
                        error.Add("statusCode", (int)rdapResults.StatusCode);
                        error.Add("errorMessage", rdapResults.ErrorMessage);
                        report.Add("rdap", error);
                    }
                }

                return Ok(report);
            }
            else
            {
                return BadRequest(new { message = "Invalid domain" });
            }

        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RDAPWorkerAPI.HttpClients;
using RDAPWorkerAPI.Models;

namespace RDAPWorkerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IPController : ControllerBase
    {
        private readonly ILogger<IPController> _logger;
        private readonly Client _client;

        public IPController(ILogger<IPController> logger, Client client)
        {
            _logger = logger;
            _client = client;
        }

        //GET api/ip/192.192.1.1
        [HttpGet("{ipaddressstring}")]
        public async Task<ActionResult<string>> GetAsync(string ipAddressString)
        {
            IPAddress ipAddress;

            if (IPAddress.TryParse(ipAddressString, out ipAddress))
            {
                ApiResult ipDetails = await _client.GetIPDetailsAsync(ipAddress);

                if (ipDetails.IsSuccessStatusCode)
                {
                    return Ok(ipDetails.Result);
                }
                else if (ipDetails.StatusCode == HttpStatusCode.NotFound)
                {
                    return NotFound(new { message = ipDetails.ErrorMessage });
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, new { message = ipDetails.ErrorMessage });
                }
            }
            else
            {
                return BadRequest(new { message = "Error parsing IP Addess" });
            }

        }
    }
}
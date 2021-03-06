﻿using System;
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
    public class DomainController : ControllerBase
    {
        private readonly ILogger<DomainController> _logger;
        private readonly Client _client;

        public DomainController(ILogger<DomainController> logger, Client client)
        {
            _logger = logger;
            _client = client;
        }

        //GET api/domain/google.com
        [HttpGet("{domain}")]
        public async Task<ActionResult<string>> GetAsync(string domain)
        {
            //TODO: BETTER DOMAIN NAME VALIDATION
            if (Uri.CheckHostName(domain) == UriHostNameType.Dns)
            {
                //TODO: Make this smarter or enforce on input
                if (domain.StartsWith("www"))
                {
                    domain = domain.Remove(domain.IndexOf("www"), 4);
                }

                ApiResult ipDetails = await _client.GetIPDetailsAsync(domain);

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
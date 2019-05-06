using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace RDAPWorkerAPI.Models
{
    public class ApiResult
    {
        public bool IsSuccessStatusCode
        {
            get
            {
                if ((int)StatusCode >= 200 && (int)StatusCode < 300)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public HttpStatusCode StatusCode { get; set; }
        public string ErrorMessage { get; set; }
        public JObject Result { get; set; }
    }
}

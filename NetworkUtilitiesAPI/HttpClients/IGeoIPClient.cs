using System.Net;
using System.Threading.Tasks;
using NetworkUtilitiesAPI.Models;

namespace NetworkUtilitiesAPI.HttpClients
{
    public interface IGeoIPClient
    {
        Task<ApiResult> GetIPDetailsAsync(IPAddress ipAddress);
        Task<ApiResult> GetIPDetailsAsync(string domain);
    }
}
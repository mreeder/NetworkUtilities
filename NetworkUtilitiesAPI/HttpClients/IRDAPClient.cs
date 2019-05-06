using System.Net;
using System.Threading.Tasks;
using NetworkUtilitiesAPI.Models;

namespace NetworkUtilitiesAPI.HttpClients
{
    public interface IRDAPClient
    {
        Task<ApiResult> GetIpDetailsAsync(IPAddress ipAddress);
        Task<ApiResult> GetIpDetailsAsync(string domain);
    }
}
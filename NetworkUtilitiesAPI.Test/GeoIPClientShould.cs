
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using NetworkUtilitiesAPI.HttpClients;
using NetworkUtilitiesAPI.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace NetworkUtilitiesAPI.Test
{
    public class GeoIPClientShould
    {
        private const string JSON_RESULT = @"{""ip"":""8.8.8.8"",""type"":""ipv4""}";

        [Fact]
        public async Task ReturnSuccessResultsWithValidIP()
        {
            var mockHandler = new Mock<HttpMessageHandler>();
            mockHandler.Protected().Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
                ).ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JSON_RESULT)
                });

            var mockConfig = new Mock<IConfigurationRoot>();
            mockConfig.SetupGet(x => x[It.IsAny<string>()]).Returns("https://localhost:44385/api/");

            var mockLogger = new Mock<ILogger<GeoIPClient>>();
            var httpClient = new HttpClient(mockHandler.Object);

            var sut = new GeoIPClient(httpClient, mockConfig.Object, mockLogger.Object);
            IPAddress ipAddress = IPAddress.Parse("8.8.8.8");

            var result = await sut.GetIPDetailsAsync(ipAddress);

            Assert.IsType<ApiResult>(result);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(JObject.Parse(JSON_RESULT), result.Result);
        }


        
    }
}

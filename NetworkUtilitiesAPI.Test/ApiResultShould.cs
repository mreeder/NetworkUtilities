using NetworkUtilitiesAPI.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Xunit;

namespace NetworkUtilitiesAPI.Test
{
    public class ApiResultShould
    {
        [Theory]
        [InlineData(HttpStatusCode.OK)]
        [InlineData(HttpStatusCode.NoContent)]
        [InlineData(HttpStatusCode.Created)]
        public void ReturnSuccessWithSuccessStatusCodes(HttpStatusCode code)
        {
            ApiResult result = new ApiResult()
            {
                StatusCode = code
            };

            Assert.True(result.IsSuccessStatusCode);
        }

        [Theory]
        [InlineData(HttpStatusCode.NotFound)]
        [InlineData(HttpStatusCode.InternalServerError)]
        [InlineData(HttpStatusCode.BadRequest)]
        public void NotReturnSuccessWithFailureStatusCodes(HttpStatusCode code)
        {
            ApiResult result = new ApiResult()
            {
                StatusCode = code
            };

            Assert.False(result.IsSuccessStatusCode);
        }
    }
}

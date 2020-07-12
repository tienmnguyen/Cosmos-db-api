using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CosmosDBAPI.Models;
using Xunit;
using System.Collections.Generic;

namespace CosmosDBAPI.IntegrationTests
{
    public class UsersControllerTests : IClassFixture<TestFixture<Startup>>
    {
        private HttpClient _client;

        public UsersControllerTests(TestFixture<Startup> fixture)
        {
            _client = fixture.Client;
        }

        [Fact]
        public async Task GetUserByEmailAsync_DataFound()
        {
            // Arrange
            var request = "/api/Users?email=matt@awesomedomain.com";

            // Act
            var response = await _client.GetAsync(request);

            // Assert
            response.EnsureSuccessStatusCode();
            var strResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var userGet = JsonConvert.DeserializeObject<UserGet>(strResponse);
            Assert.NotNull(userGet.UserId);
        }

        [Fact]
        public async Task GetUserByEmailAsync_DataNotFound()
        {
            // Arrange
            var request = "/api/Users?email=matt1@awesomedomain.com";

            // Act
            var response = await _client.GetAsync(request);

            // Assert
            var strResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var userGet = JsonConvert.DeserializeObject<UserGet>(strResponse);
            Assert.Null(userGet.UserId);
        }

        [Fact]
        public async Task CreateAsync_OK()
        {
            // Arrange
            // check email does not exist
            var response = await _client.GetAsync("/api/Users?email=matt@awesomedomain.com").ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
            var strResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var userGet = JsonConvert.DeserializeObject<UserGet>(strResponse);
            if (userGet.ErrorMessages.Count > 0)
                throw new Exception($"Error getting by email: {userGet.ErrorMessages[0]}");
            if (userGet.UserId != null)
            {
                // delete if email exists, assume email is unique
                response = await _client.DeleteAsync($"/api/Users/{userGet.UserId}").ConfigureAwait(false);

                response.EnsureSuccessStatusCode();
                strResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var errList = JsonConvert.DeserializeObject<List<string>>(strResponse);
                if (errList.Count > 0)
                    throw new Exception($"Error deleting: {errList[0]}");
            }
            var request = new User
            {
                FirstName = "Matthew",
                MiddleName = "Decker",
                LastName = "Lund",
                PhoneNumber = "555-555-5555",
                EmailAddress = "matt@awesomedomain.com"
            };
            // Act
            response = await _client.PostAsync("/api/Users", ContentHelper.GetStringContent(request)).ConfigureAwait(false);

            // Assert
            response.EnsureSuccessStatusCode();
            strResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var userNew = JsonConvert.DeserializeObject<UserReturn>(strResponse);
            if (userNew.ErrorMessages.Count > 0)
                throw new Exception($"Error adding user: {userNew.ErrorMessages[0]}");
            // get back and compare
            response = await _client.GetAsync("/api/Users?email=matt@awesomedomain.com").ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
            strResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            userGet = JsonConvert.DeserializeObject<UserGet>(strResponse);
            Assert.Equal(userGet.UserId, userNew.Id.ToString());
        }
    }
}
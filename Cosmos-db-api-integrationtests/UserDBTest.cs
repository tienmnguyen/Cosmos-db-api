using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Cosmos_db_api.Models;
using Xunit;

namespace Cosmos_db_api.IntegrationTests
{
    public class UsersControllerTests : IClassFixture<TestFixture<Startup>>
    {
        private HttpClient _client;

        public UsersControllerTests(TestFixture<Startup> fixture)
        {
            _client = fixture.Client;
        }

        [Fact]
        public async Task GetUserbyEmailAsync_DataFound()
        {
            // Arrange
            var request = "/api/Users?email=matt@awesomedomain.com";

            // Act
            var response = await _client.GetAsync(request);

            // Assert
            response.EnsureSuccessStatusCode();
            var strResponse = await response.Content.ReadAsStringAsync();
            var userGet = JsonConvert.DeserializeObject<UserGet>(strResponse);
            Assert.NotNull(userGet.UserId);
        }

        [Fact]
        public async Task GetUserbyEmailAsync_DataNotFound()
        {
            // Arrange
            var request = "/api/Users?email=matt1@awesomedomain.com";

            // Act
            var response = await _client.GetAsync(request);

            // Assert
            var strResponse = await response.Content.ReadAsStringAsync();
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

            if (userGet.UserId != null)
            {
                // dele
                response = await _client.DeleteAsync($"/api/Users/{userGet.UserId}").ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
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
            // get back and compare
            response = await _client.GetAsync("/api/Users?email=matt@awesomedomain.com").ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            strResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            userGet = JsonConvert.DeserializeObject<UserGet>(strResponse);
            Assert.Equal(userGet.UserId, userNew.Id.ToString());
        }
    }
}
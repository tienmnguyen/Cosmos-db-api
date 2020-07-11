using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Cosmos_db_api.Interfaces;
using Cosmos_db_api.Models;

namespace Cosmos_db_api.DataProviders
{
    public class UserData : IUser
    {
        private Container _container;

        public UserData(
            CosmosClient dbClient,
            string databaseName,
            string containerName)
        {
            this._container = dbClient.GetContainer(databaseName, containerName);
        }

        public async Task<List<string>> CreateAsync(Cosmos_db_api.Models.User user)
        {
            var errMsgs = new List<string>();
            try
            {
                user.id = Guid.NewGuid();
                var ret = await this._container.CreateItemAsync<Models.User>(user);
            }
            catch (CosmosException ex)
            {
                errMsgs.Add($"Error creating user: {ex.Message}");
            }
            return errMsgs;
        }
        public async Task<UserGet> GetByEmailAsync(string email)
        {
            UserGet userGet = new UserGet();
            try
            {
                QueryDefinition queryDefinition = new QueryDefinition("select * from User u where u.EmailAddress = @email")
                     .WithParameter("@email", email);
                FeedIterator<Cosmos_db_api.Models.User> feedIterator = this._container.GetItemQueryIterator<Cosmos_db_api.Models.User>(
                    queryDefinition);

                while (feedIterator.HasMoreResults)
                {
                    foreach (var user in await feedIterator.ReadNextAsync())
                    {
                        userGet.UserId = user.id.ToString();
                        userGet.Name = user.FirstName + (string.IsNullOrEmpty(user.MiddleName) ? String.Empty : " " + user.MiddleName) +
                            " " + user.LastName;
                        userGet.PhoneNumber = user.PhoneNumber;
                        userGet.EmailAddress = user.EmailAddress;
                        break;
                    }
                }
            }
            catch (CosmosException ex)
            {
                userGet.ErrorMessages.Add($"Error getting user by email: " + ex.Message);
            }
            return userGet;
        }
    }
}

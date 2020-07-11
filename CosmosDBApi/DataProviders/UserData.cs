using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using CosmosDBAPI.Interfaces;
using CosmosDBAPI.Models;

namespace CosmosDBAPI.DataProviders
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

        public async Task<UserReturn> CreateAsync(CosmosDBAPI.Models.User user)
        {
            var userRet = new UserReturn();
            try
            {
                user.id = Guid.NewGuid();
                var userResp = await this._container.CreateItemAsync<Models.User>(user, new PartitionKey(user.id.ToString()));
                userRet.Id = ((Models.User)userResp.Resource).id;
            }
            catch (CosmosException ex)
            {
                userRet.ErrorMessages.Add($"Error creating user: {ex.Message}");
            }
            return userRet;
        }
        public async Task<UserGet> GetByEmailAsync(string email)
        {
            UserGet userGet = new UserGet();
            try
            {
                QueryDefinition queryDefinition = new QueryDefinition("select * from User u where u.EmailAddress = @email")
                     .WithParameter("@email", email);
                FeedIterator<CosmosDBAPI.Models.User> feedIterator = this._container.GetItemQueryIterator<CosmosDBAPI.Models.User>(
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
        public async Task<List<string>> DeleteAsync(Guid id)
        {
            var errMsgs = new List<string>();
            try
            {
                var ret = await this._container.DeleteItemAsync<Models.User>(id.ToString(), new PartitionKey(id.ToString()));
            }
            catch (CosmosException ex)
            {
                errMsgs.Add($"Error deleting user: " + ex.Message);
            }
            return errMsgs;
        }
    }
}

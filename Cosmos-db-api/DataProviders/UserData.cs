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
        //TODO add call to data provider
        public async Task<List<string>> CreateAsync(Cosmos_db_api.Models.User user)
        {
            return null;
        }
        public async Task<UserGet> GetByEmailAsync(string email)
        {
            return null;
        }
    }
}

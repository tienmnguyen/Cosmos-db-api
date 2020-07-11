using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CosmosDBAPI.Models;

namespace CosmosDBAPI.Interfaces
{
    public interface IUser
    {
        // return list of error messages
        Task<UserReturn> CreateAsync(User user);
        // return user data with list of error messages
        Task<UserGet> GetByEmailAsync(string email);
        // delete user
        Task<List<string>> DeleteAsync(Guid id);
    }
}

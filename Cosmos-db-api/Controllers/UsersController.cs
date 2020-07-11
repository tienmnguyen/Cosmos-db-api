using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Cosmos_db_api.Models;
using Cosmos_db_api.Interfaces;

namespace Cosmos_db_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IUser _userService;

        public UsersController(IUser userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<List<string>> CreateAsync(User user)
        {
            var ret = new List<string>();
            if (String.IsNullOrEmpty(user?.FirstName))
            {
                ret.Add("The User First Name is required");
            }
            if (String.IsNullOrEmpty(user?.LastName))
            {
                ret.Add("The User Last Name is required");
            }
            if (String.IsNullOrEmpty(user?.EmailAddress))
            {
                ret.Add("The User email address is required");
            }
            ret = await _userService.CreateAsync(user);
            return ret;
        }

        [HttpGet]
        public async Task<UserGet> GetByEmailAsync(string email)
        {
            var ret = new UserGet();
            if (String.IsNullOrEmpty(email))
            {
                ret.ErrorMessages.Add("The User email address is required");
            }
            // TODO add email validation
            ret = await _userService.GetByEmailAsync(email);
            return ret;

        }
    }
}
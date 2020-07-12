using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CosmosDBAPI.Models;
using CosmosDBAPI.Interfaces;

namespace CosmosDBAPI.Controllers
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
        public async Task<UserReturn> CreateAsync(User user)
        {
            var userRet = new UserReturn();
            if (String.IsNullOrEmpty(user?.FirstName))
            {
                userRet.ErrorMessages.Add("The User First Name is required");
            }
            if (String.IsNullOrEmpty(user?.LastName))
            {
                userRet.ErrorMessages.Add("The User Last Name is required");
            }
            if (String.IsNullOrEmpty(user?.EmailAddress))
            {
                userRet.ErrorMessages.Add("The User email address is required");
            }
            userRet = await _userService.CreateAsync(user);
            return userRet;
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

        [HttpDelete ("{id}")]
        public async Task<List<string>> DeleteAsync([FromRoute] Guid id)
        {
            var errMsgs = await _userService.DeleteAsync(id);
            return errMsgs;
        }

        [HttpPost("{id}")]
        public async Task<List<string>> UpdateAsync([FromRoute] Guid id, User user)
        {
            var errMsgs = await _userService.UpdateAsync(id, user);
            return errMsgs;
        }
    }
}
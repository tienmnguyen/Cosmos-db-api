﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Cosmos_db_api.Models
{
    public class User
    {
        [JsonProperty(PropertyName = "id")]
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
    }
    public class UserGet
    {
        public List<string> ErrorMessages { get; set; }
        public User User { get; set; }

        public UserGet()
        {
            ErrorMessages = new List<string>();
        }
    }
}
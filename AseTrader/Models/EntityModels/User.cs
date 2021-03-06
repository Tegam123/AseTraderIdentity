﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AseTrader.Models.EntityModels;
using Microsoft.AspNetCore.Identity;

namespace AseTrader.Models
{
    public class User : IdentityUser
    {
        //[Required]
        [Display(Name = "First name")]
        public string FirstName { get; set; }
        //[Required]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        public List<Follow> Following { get; set; }

        public List<Follow> Followers { get; set; }

        public string secret_accesstoken { get; set; }

    }
}

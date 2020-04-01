using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AseTrader.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required, MaxLength(254)]
        public string Email { get; set; }

        [Required, MaxLength(56)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}

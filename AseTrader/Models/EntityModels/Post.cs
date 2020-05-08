using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AseTrader.Models.EntityModels
{
    public class Post
    {
        public long PostId { get; set; }

        [MaxLength(450)]
        public string ApplicationUserId { get; set; }

        [DisplayName("User")]
        public User ApplicationUser { get; set; }
 
        [DisplayName("Post")]
        [MaxLength(2000)]
        public string Comment { get; set; }

        public DateTime Date { get; set; }
    }
}

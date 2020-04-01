using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AseTrader.Models.EntityModels
{
    public class Post
    {
        [Key] public int PostId { get; set; }
        public string body { get; set; }
        public string added_by { get; set; }
        public string User_posted_to { get; set; }
    }
}

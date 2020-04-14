using AseTrader.Models.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AseTrader.Models.ViewModels
{
    public class PostsViewModel
    {
        public Post CurrentPost { get; set; }
        public List<Post> Posts { get; set; }
    }
}

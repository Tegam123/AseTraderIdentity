using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AseTrader.Models.EntityModels
{
    public class Follow
    {
        [Key]
        public int FollowId { get; set; }

        [MaxLength(450)]
        public string followersId { get; set; }
        public User Followers { get; set; }

        [MaxLength(450)]
        public string followingId { get; set; }
        public User Following { get; set; }
    }
}

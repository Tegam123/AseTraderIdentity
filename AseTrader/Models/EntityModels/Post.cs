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
        /// <summary>
        /// Primary key
        /// </summary>
        public long PostId { get; set; }
        /// <summary>
        /// ApplicationUserId - foreign key.
        /// </summary>
        /// 
        [MaxLength(450)]
        public string ApplicationUserId { get; set; }
        /// <summary>
        /// ApplicationUser.
        /// </summary>
        /// 
        [DisplayName("User")]
        [ForeignKey(nameof(ApplicationUserId))]
        public User ApplicationUser { get; set; }
        /// <summary>
        /// The review content.
        /// </summary>
        [DisplayName("Post")]
        [MaxLength(2000)]
        public string Comment { get; set; }
    }
}

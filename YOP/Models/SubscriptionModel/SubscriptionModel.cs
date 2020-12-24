using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace YOP.Models.SubscriptionModel
{
    public class SubscriptionModel
    {
        [Required]
        public Guid AuthorId { get; set; }
    }
}

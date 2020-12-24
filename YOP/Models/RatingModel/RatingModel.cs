using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace YOP.Models.RatingModel
{
    public class RatingModel
    {
        [Required]
        public Guid PodcastId { get; set; }
    }
}

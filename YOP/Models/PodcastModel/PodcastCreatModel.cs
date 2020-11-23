using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace YOP.Models.PodcastModel
{
    public class PodcastCreatModel
    {
        [Required]
        public string Name { get; set; }
        public string Genre { get; set; }
        public string Description { get; set; }

    }
}

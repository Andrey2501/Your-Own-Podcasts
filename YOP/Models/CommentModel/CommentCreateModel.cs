using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace YOP.Models.CommentModel
{
    public class CommentCreateModel
    {
        [Required]
        public Guid PodcastId { get; set; }
        [Required]
        public string Text { get; set; }
    }
}

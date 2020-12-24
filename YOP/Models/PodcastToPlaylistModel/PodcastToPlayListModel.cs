using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace YOP.Models.PodcastToPlaylistModel
{
    public class PodcastToPlayListModel
    {
        [Required]
        public Guid PodcastId { get; set; }
        [Required]
        public Guid PlaylistId { get; set; }
        [Required]
        public Guid UserId { get; set; }
    }
}

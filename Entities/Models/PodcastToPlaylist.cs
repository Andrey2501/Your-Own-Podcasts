using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.Models
{
    [Table("PodcastToPlaylist")]
    public class PodcastToPlaylist
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid PodcastId { get; set; }
        public Guid PlaylistId { get; set; }
        public DateTime PublicationDate { get; set; }
    }
}

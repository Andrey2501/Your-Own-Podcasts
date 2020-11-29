using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Entities.QueryModels
{
    public class PodcastToPlaylistParameters: QueryStringParameters
    {
        public string Name { get; set; }
        public string Genre { get; set; }
        public string AuthorName { get; set; }
        public string OrderBy { get; set; }
        [Required]
        public Guid PlaylistId { get; set; }
    }
}

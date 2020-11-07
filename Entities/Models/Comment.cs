using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("comment")]
    public class Comment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public Guid PodcastId { get; set; }
        public DateTime PublicationDate { get; set; }
        public string Text { get; set; }
    }
}

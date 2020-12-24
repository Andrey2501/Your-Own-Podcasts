using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.QueryModels
{
    public class CommentParameters: QueryStringParameters
    {
        public Guid PodcastId { get; set; }
    }
}

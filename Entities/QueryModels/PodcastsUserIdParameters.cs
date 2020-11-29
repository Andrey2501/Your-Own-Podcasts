using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.QueryModels
{
    public class PodcastsUserIdParameters: QueryStringParameters
    {
         public Guid UserId { get; set; }
    }
}

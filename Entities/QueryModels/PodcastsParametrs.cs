using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.QueryModels
{
    public class PodcastsParametrs : QueryStringParameters
    {
        public string Name { get; set; }
        public string Genre { get; set; }
        public string AuthorName { get; set; }
        public string OrderBy { get; set; }
    }
}

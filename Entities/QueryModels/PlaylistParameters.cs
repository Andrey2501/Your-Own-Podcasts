using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.QueryModels
{
    public class PlaylistParameters: QueryStringParameters
    {
        public string Name { get; set; }
        public string OrderBy { get; set; }
    }
}

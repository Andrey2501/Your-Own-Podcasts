using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YOP.Models.PodcastModel
{
    public class PodcastGetModel : Podcast
    {
        public bool IsLike { get; set; }
        public int CountLikes { get; set; } = 0;
    }
}

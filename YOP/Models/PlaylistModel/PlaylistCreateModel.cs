using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace YOP.Models.PlaylistModel
{
    public class PlaylistCreateModel
    {
        [Required]
        public string Name { get; set; }
    }
}

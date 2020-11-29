using Contracts;
using Entities;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repository
{
    class PodcastToPlaylistRepository: RepositoryBase<PodcastToPlaylist>, IPodcastToPlaylistRepository
    {
        public PodcastToPlaylistRepository(RepositoryContext repositoryContext)
            : base(repositoryContext)
        { 
        }
    }
}

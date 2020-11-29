using Entities;
using Entities.Models;
using Entities.QueryModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts
{
    public interface IPodcastToPlaylistRepository: IRepositoryBase<PodcastToPlaylist>
    {
        PagedList<Podcast> FindByPlaylistId(PodcastToPlaylistParameters parameters);
    }
}

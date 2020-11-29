using Entities;
using Entities.Models;
using Entities.QueryModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts
{
    public interface IPodcastRepository : IRepositoryBase<Podcast>
    {
        PagedList<Podcast> FindAll(PodcastsParametrs parameters);
        PagedList<Podcast> FindByUserId(PodcastsUserIdParameters parameters);

    }
}

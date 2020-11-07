using Contracts;
using Entities;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repository
{
    class PodcastRepository : RepositoryBase<Podcast>, IPodcastRepository
    {
        public PodcastRepository(RepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }
    }
}

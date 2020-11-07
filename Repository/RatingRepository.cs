using Contracts;
using Entities;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repository
{
    class RatingRepository : RepositoryBase<Rating>, IRatingRepository
    {
        public RatingRepository(RepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }
    }
}

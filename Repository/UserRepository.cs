using Contracts;
using Entities;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repository
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        public UserRepository(RepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }
        public PagedList<User> FindAll(QueryStringParameters parameters)
        {
            return PagedList<User>.ToPagedList(FindAll(),
                parameters.PageNumber,
                parameters.PageSize);
        }
    }
}

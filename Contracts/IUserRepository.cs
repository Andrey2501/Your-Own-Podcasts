using Entities;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts
{
    public interface IUserRepository : IRepositoryBase<User>
    {
        PagedList<User> FindAll(QueryStringParameters parameters);
    }
}

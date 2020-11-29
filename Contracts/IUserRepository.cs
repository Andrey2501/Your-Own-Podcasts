using Entities;
using Entities.Models;
using Entities.QueryModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts
{
    public interface IUserRepository : IRepositoryBase<User>
    {
        PagedList<User> FindAll(UserParameters parameters);
    }
}

using Entities;
using Entities.Models;
using Entities.QueryModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts
{
    public interface ISubscriptionRepository : IRepositoryBase<Subscription>
    {
        PagedList<Subscription> FindByUser(SubscriptionParameters parameters);
        PagedList<Subscription> FindByAuthor(SubscriptionParameters parameters);
    }
}

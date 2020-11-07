using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts
{
    public interface IRepositoryWrapper
    {
        IUserRepository User { get; }
        IPodcastRepository Podcast { get; }
        IRatingRepository Rating { get; }
        ICommentRepository Comment { get; }
        ISubscriptionRepository Subscription { get; }
        void Save();
    }
}

using Entities;
using Entities.Models;
using Entities.QueryModels;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Contracts
{
    public interface ICommentRepository : IRepositoryBase<Comment>
    {
        PagedList<Comment> FindByPodcast(CommentParameters parameters);
    }
}

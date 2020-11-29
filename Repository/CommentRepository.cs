using Contracts;
using Entities;
using Entities.Models;
using Entities.QueryModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Repository
{
    class CommentRepository : RepositoryBase<Comment>, ICommentRepository
    {
        public CommentRepository(RepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public PagedList<Comment> FindByPodcast(CommentParameters parameters)
        {
            var comments = FindByCondition(c => c.PodcastId == parameters.PodcastId);

            return PagedList<Comment>.ToPagedList(comments.OrderBy(c => c.PublicationDate),
                parameters.PageNumber,
                parameters.PageSize);
        }
    }
}

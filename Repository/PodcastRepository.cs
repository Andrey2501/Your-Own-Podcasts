using Contracts;
using Entities;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Linq.Dynamic.Core;
using Entities.QueryModels;

namespace Repository
{
    class PodcastRepository : RepositoryBase<Podcast>, IPodcastRepository
    {
        public PodcastRepository(RepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }
        public PagedList<Podcast> FindByUserId(PodcastsUserIdParameters parameters)
        {
            IQueryable<Podcast> podcasts = FindByCondition(p => p.UserId == parameters.UserId);

            return PagedList<Podcast>.ToPagedList(podcasts, parameters.PageNumber, parameters.PageSize);
        }
        public PagedList<Podcast> FindAll(PodcastsParametrs parameters)
        {
            var podcasts = FindAll();

            SearchByName(ref podcasts, parameters.Name);
            SearchByGenre(ref podcasts, parameters.Genre);
            SearchByAuthorName(ref podcasts, parameters.AuthorName);
            ApplySort(ref podcasts, parameters.OrderBy);

            return PagedList<Podcast>.ToPagedList(podcasts,
                parameters.PageNumber,
                parameters.PageSize);
        }
        private void SearchByName(ref IQueryable<Podcast> podcasts, string name)
        {
            if (!podcasts.Any() || string.IsNullOrWhiteSpace(name))
                return;
            podcasts = podcasts.Where(p => p.Name.ToLower().Contains(name.Trim().ToLower()));
        }
        private void SearchByGenre(ref IQueryable<Podcast> podcasts, string genre)
        {
            if (!podcasts.Any() || string.IsNullOrWhiteSpace(genre))
                return;
            podcasts = podcasts.Where(p => p.Genre.ToLower().Contains(genre.Trim().ToLower()));
        }
        private void SearchByAuthorName(ref IQueryable<Podcast> podcasts, string authorName)
        {
            if (!podcasts.Any() || string.IsNullOrWhiteSpace(authorName))
                return;

            List<Guid> usersId = RepositoryContext.Users
                .Where(u => (u.FirstName + " " + u.LastName).ToLower().Contains(authorName.Trim().ToLower()))
                .Select(u => u.Id)
                .ToList();
            podcasts = podcasts.Where(p => usersId.Contains(p.UserId));
        }
        private void ApplySort(ref IQueryable<Podcast> podcasts, string orderByQueryString)
        {
            if (!podcasts.Any())
                return;
            if (string.IsNullOrWhiteSpace(orderByQueryString))
            {
                podcasts = podcasts.OrderBy(x => x.Name);
                return;
            }
            var orderParams = orderByQueryString.Trim().Split(',');
            var propertyInfos = typeof(Podcast).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var orderQueryBuilder = new StringBuilder();
            foreach (var param in orderParams)
            {
                if (string.IsNullOrWhiteSpace(param))
                    continue;
                var propertyFromQueryName = param.Split(" ")[0];
                var objectProperty = propertyInfos.FirstOrDefault(pi =>
                    pi.Name.Equals(propertyFromQueryName, StringComparison.InvariantCultureIgnoreCase));
                if (objectProperty == null)
                    continue;
                var sortingOrder = param.EndsWith(" desc") ? "descending" : "ascending";
                orderQueryBuilder.Append($"{objectProperty.Name} {sortingOrder}, ");
            }
            var orderQuery = orderQueryBuilder.ToString().TrimEnd(',', ' ');
            if (string.IsNullOrWhiteSpace(orderQuery))
            {
                podcasts = podcasts.OrderBy(x => x.Name);
                return;
            }
            podcasts = podcasts.OrderBy(orderQuery);
        }
    }
}

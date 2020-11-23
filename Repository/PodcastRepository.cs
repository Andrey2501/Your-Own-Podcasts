using Contracts;
using Entities;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Linq.Dynamic.Core;

namespace Repository
{
    class PodcastRepository : RepositoryBase<Podcast>, IPodcastRepository
    {
        public PodcastRepository(RepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }
        public PagedList<Podcast> FindAll(QueryStringParameters parameters)
        {
            var podcasts = FindAll();
            SearchByField(ref podcasts, parameters.Name);
            SearchByField(ref podcasts, parameters.Genre);
            ApplySort(ref podcasts, parameters.OrderBy);

            return PagedList<Podcast>.ToPagedList(podcasts.OrderBy(p => p.Name),
                parameters.PageNumber,
                parameters.PageSize);
        }
        private void SearchByField(ref IQueryable<Podcast> podcasts, string name)
        {
            if (!podcasts.Any() || string.IsNullOrWhiteSpace(name))
                return;
            podcasts = podcasts.Where(p => p.Name.ToLower().Contains(name.Trim().ToLower()));
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
            var propertyInfos = typeof(User).GetProperties(BindingFlags.Public | BindingFlags.Instance);
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

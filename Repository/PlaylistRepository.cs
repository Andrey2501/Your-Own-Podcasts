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
using System.Linq.Expressions;

namespace Repository
{
    class PlaylistRepository: RepositoryBase<Playlist>, IPlaylistRepository
    {
          public PlaylistRepository(RepositoryContext repositoryContext)
            : base(repositoryContext)
          {
          }

        public PagedList<Playlist> FindByCondition(Expression<Func<Playlist, bool>> expression, PlaylistParameters parameters)
        {
            var playlists = FindByCondition(expression);

            SearchByName(ref playlists, parameters.Name);
            ApplySort(ref playlists, parameters.OrderBy);

            return PagedList<Playlist>.ToPagedList(playlists,
                parameters.PageNumber,
                parameters.PageSize);
        }
        private void SearchByName(ref IQueryable<Playlist> playlists, string name)
        {
            if (!playlists.Any() || string.IsNullOrWhiteSpace(name))
                return;
            playlists = playlists.Where(p => p.Name.ToLower().Contains(name.Trim().ToLower()));
        }
        private void ApplySort(ref IQueryable<Playlist> playlists, string orderByQueryString)
        {
            if (!playlists.Any())
                return;
            if (string.IsNullOrWhiteSpace(orderByQueryString))
            {
                playlists = playlists.OrderBy(x => x.Name);
                return;
            }
            var orderParams = orderByQueryString.Trim().Split(',');
            var propertyInfos = typeof(Playlist).GetProperties(BindingFlags.Public | BindingFlags.Instance);
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
                playlists = playlists.OrderBy(x => x.Name);
                return;
            }
            playlists = playlists.OrderBy(orderQuery);
        }
    }
}

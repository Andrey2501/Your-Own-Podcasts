using Contracts;
using Entities;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repository
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        private RepositoryContext _repositoryContext;
        public UserRepository(RepositoryContext repositoryContext)
            : base(repositoryContext)
        {
            _repositoryContext = repositoryContext;
        }
        public PagedList<User> FindAll(QueryStringParameters parameters)
        {
            var users = FindAll();
            SearchByName(ref users, parameters.Name);

            return PagedList<User>.ToPagedList(users.OrderBy(u => u.FirstName),
                parameters.PageNumber,
                parameters.PageSize);
        }
        private void SearchByName(ref IQueryable<User> users, string userName)
        {
            if (!users.Any() || string.IsNullOrWhiteSpace(userName))
                return;
            users = users.Where(u => u.FirstName.ToLower().Contains(userName.Trim().ToLower()) ||
                u.LastName.ToLower().Contains(userName.Trim().ToLower()) ||
                $"{u.FirstName} {u.LastName}".ToLower().Contains(userName.Trim().ToLower()));
        }
    }
}

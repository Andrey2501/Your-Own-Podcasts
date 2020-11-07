using Entities;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Contracts
{
    public interface ICommentRepository : IRepositoryBase<Comment>
    {
    }
}

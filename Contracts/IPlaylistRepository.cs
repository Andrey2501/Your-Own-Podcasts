using Entities;
using Entities.Models;
using Entities.QueryModels;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Contracts
{
    public interface IPlaylistRepository: IRepositoryBase<Playlist>
    {
        PagedList<Playlist> FindByCondition(Expression<Func<Playlist, bool>> expression, PlaylistParameters parameters);
    }
}

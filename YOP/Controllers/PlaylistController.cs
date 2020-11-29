using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Contracts;
using Entities;
using Entities.Models;
using Entities.QueryModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using YOP.Models.PlaylistModel;

namespace YOP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PlaylistController : ControllerBase
    {
        private readonly IRepositoryWrapper _repoWrapper;
        public PlaylistController(IRepositoryWrapper repoWrapper)
        {
            _repoWrapper = repoWrapper;
        }
        [HttpPost, Authorize]
        public IActionResult CreatePlaylist([FromBody] PlaylistCreateModel playlistModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            User user = _repoWrapper.User.FindByCondition(u => u.Id.ToString() == userId).FirstOrDefault();
            if (user == null)
            {
                return BadRequest("Token is incorrect");
            }

            IQueryable<Playlist> playlists = _repoWrapper.Playlist
                .FindByCondition(p => p.UserId == user.Id && p.Name == playlistModel.Name);
            if (playlists.Count() != 0)
            {
                return BadRequest("Name alredy exists");
            }

            Playlist playlist = new Playlist() 
            {
                Name = playlistModel.Name,
                DateCreated = DateTime.Now,
                UserId = user.Id
            };
            
            _repoWrapper.Playlist.Create(playlist);
            _repoWrapper.Save();

            return Ok(playlist);
        }

        [HttpGet("{id}")]
        public IActionResult GetPlaylistById(Guid id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string role = User.FindFirstValue(ClaimTypes.Role);

            Playlist playlist = _repoWrapper.Playlist
                .FindByCondition(p => p.Id == id && (p.UserId.ToString() == userId || role == "Admin"))
                .FirstOrDefault();

            if (playlist == null)
            {
                return BadRequest("PlaylistId is incorrect");
            }

            return Ok(playlist);
        }
        [HttpGet, Route("list")]
        public IActionResult GetListPlaylist([FromQuery] PlaylistParameters parameters)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Token is incorrect");
            }

            PagedList<Playlist> playlists = _repoWrapper.Playlist
                .FindByCondition(p => p.UserId.ToString() == userId, parameters);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(playlists.MetaData));

            return Ok(playlists);
        }
        [HttpDelete("{id}"), Authorize]
        public IActionResult DeleteById(Guid id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string role = User.FindFirstValue(ClaimTypes.Role);
            Playlist playlist = _repoWrapper.Playlist
                .FindByCondition(p => p.Id == id && (p.UserId.ToString() == userId || role == "Admin"))
                .FirstOrDefault();

            if (playlist == null)
            {
                return NotFound();
            }

            _repoWrapper.Playlist.Delete(playlist);
            _repoWrapper.Save();

            return Ok(new { Success = true });

        }
    }
}

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
using YOP.Models.PodcastModel;
using YOP.Models.PodcastToPlaylistModel;
using YOP.Services;

namespace YOP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PodcastToPlaylistController : ControllerBase
    {
        private readonly IRepositoryWrapper _repoWrapper;
        private StatisticService _statisticService;
        public PodcastToPlaylistController(IRepositoryWrapper repoWrapper)
        {
            _repoWrapper = repoWrapper;
            _statisticService = new StatisticService(_repoWrapper);
        }

        [HttpPost]
        public IActionResult CreatePodcastToPlaylist([FromBody] PodcastToPlayListModel podcastToPlayListModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string role = User.FindFirstValue(ClaimTypes.Role);
            User user = _repoWrapper.User.FindByCondition(u => u.Id.ToString() == userId).FirstOrDefault();
            if (user == null)
            {
                return BadRequest("Token is incorrect");
            }
            if (user.Id != podcastToPlayListModel.UserId && role != "Admin")
            {
                return BadRequest("UserId is incorrect");
            }

            Playlist playlist = _repoWrapper.Playlist
                .FindByCondition(p => p.Id == podcastToPlayListModel.PlaylistId)
                .FirstOrDefault();
            if (playlist == null)
            {
                return NotFound("PlaylistId is incorrect");
            }

            Podcast podcast = _repoWrapper.Podcast
                .FindByCondition(p => p.Id == podcastToPlayListModel.PodcastId)
                .FirstOrDefault();
            if (podcast == null)
            {
                return NotFound("PodcastId is incorrect");
            }

            PodcastToPlaylist podcastToPlaylistCheck = _repoWrapper.PodcastToPlaylist
                .FindByCondition(p => 
                    p.PodcastId == podcastToPlayListModel.PodcastId && 
                    p.PlaylistId == podcastToPlayListModel.PlaylistId &&
                    p.UserId == podcastToPlayListModel.UserId)
                .FirstOrDefault();
            if (podcastToPlaylistCheck != null)
            {
                return BadRequest("The podcast already exists in the playlist");
            }

            PodcastToPlaylist podcastToPlaylist = new PodcastToPlaylist()
            {
                PodcastId = podcastToPlayListModel.PodcastId,
                PlaylistId = podcastToPlayListModel.PlaylistId,
                PublicationDate = DateTime.Now,
                UserId = podcastToPlayListModel.UserId
            };
            _repoWrapper.PodcastToPlaylist.Create(podcastToPlaylist);
            _repoWrapper.Save();

            return Ok(podcastToPlaylist);
        }

        [HttpGet, Route("list")]
        public IActionResult GetListPodcasts([FromQuery] PodcastToPlaylistParameters parameters)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string strUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string role = User.FindFirstValue(ClaimTypes.Role);
            User user = _repoWrapper.User.FindByCondition(u => u.Id.ToString() == strUserId).FirstOrDefault();
            if (user == null)
            {
                return BadRequest("Token is incorrect");
            }

            PagedList<Podcast> podcasts = _repoWrapper.PodcastToPlaylist.FindByPlaylistId(parameters);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(podcasts.MetaData));

            List<PodcastGetModel> podcastModel = _statisticService.TransformPodcast(podcasts, user.Id);

            return Ok(podcastModel);
        }

        [HttpDelete]
        public IActionResult DeleteById([FromBody] PodcastToPlayListModel podcastToPlayListModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string role = User.FindFirstValue(ClaimTypes.Role);

            if (podcastToPlayListModel.UserId.ToString() != userId && role != "Admin")
            {
                return BadRequest("UserId is incorrect");
            }

            User user = _repoWrapper.User
                .FindByCondition(u => u.Id == podcastToPlayListModel.UserId)
                .FirstOrDefault();

            if (user == null)
            {
                return BadRequest("Token is incorrect");
            }

            PodcastToPlaylist podcastToPlaylist = _repoWrapper.PodcastToPlaylist
                .FindByCondition(p =>
                    p.PlaylistId == podcastToPlayListModel.PlaylistId &&
                    p.PodcastId == podcastToPlayListModel.PodcastId &&
                    p.UserId == podcastToPlayListModel.UserId)
                .FirstOrDefault();

            if (podcastToPlaylist == null)
            {
                return BadRequest("The podcast not exists in the playlist");
            }

            _repoWrapper.PodcastToPlaylist.Delete(podcastToPlaylist);
            _repoWrapper.Save();

            return Ok(new { Success = true });

        }
    }
}

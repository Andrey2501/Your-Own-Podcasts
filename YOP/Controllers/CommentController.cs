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
using YOP.Models.CommentModel;

namespace YOP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly IRepositoryWrapper _repoWrapper;
        public CommentController(IRepositoryWrapper repoWrapper)
        {
            _repoWrapper = repoWrapper;
        }

        [HttpPost, Authorize]
        public IActionResult CreateComment([FromBody] CommentCreateModel commentModel)
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

            Podcast podcast = _repoWrapper.Podcast.FindByCondition(p => p.Id == commentModel.PodcastId).FirstOrDefault();
            if (podcast == null)
            {
                return NotFound("PodcastId is incorrect");
            }

            Comment comment = new Comment()
            {
                Text = commentModel.Text,
                PodcastId = commentModel.PodcastId,
                UserId = user.Id,
                PublicationDate = DateTime.Now
            };

            _repoWrapper.Comment.Create(comment);
            _repoWrapper.Save();

            return Ok(comment);
        }

        [HttpGet, Route("list")]
        public IActionResult GetListComment([FromQuery] CommentParameters parameters)
        {

            PagedList<Comment> comments = _repoWrapper.Comment.FindByPodcast(parameters);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(comments.MetaData));

            return Ok(comments);
        }
        [HttpDelete("{id}"), Authorize]
        public IActionResult DeleteById(Guid id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string role = User.FindFirstValue(ClaimTypes.Role);
            Comment comment = _repoWrapper.Comment
                .FindByCondition(p => p.Id == id && (p.UserId.ToString() == userId || role == "Admin"))
                .FirstOrDefault();

            if (comment == null)
            {
                return NotFound();
            }

            _repoWrapper.Comment.Delete(comment);
            _repoWrapper.Save();

            return Ok(new { Success = true });

        }
    }
}

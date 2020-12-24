using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Contracts;
using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YOP.Models.RatingModel;

namespace YOP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingController : ControllerBase
    {
        private readonly IRepositoryWrapper _repoWrapper;
        public RatingController(IRepositoryWrapper repoWrapper)
        {
            _repoWrapper = repoWrapper;
        }

        [HttpDelete, Authorize]
        public IActionResult RemoveLike([FromBody] RatingModel ratingModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            User user = _repoWrapper.User.FindByCondition(u => u.Id.ToString() == userId).FirstOrDefault();
            if (user == null)
            {
                return Unauthorized("Token is incorrect");
            }

            Rating ratingCheck = _repoWrapper.Rating
                .FindByCondition(r => r.UserId == user.Id && r.PodcastId == ratingModel.PodcastId)
                .FirstOrDefault();
            if (ratingCheck == null)
            {
                return BadRequest("Like not exist");
            }

            _repoWrapper.Rating.Delete(ratingCheck);
            _repoWrapper.Save();

            return Ok(new { Success = true });

        }

        [HttpPost, Authorize]
        public IActionResult AddLike([FromBody] RatingModel ratingModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            User user = _repoWrapper.User.FindByCondition(u => u.Id.ToString() == userId).FirstOrDefault();
            if (user == null)
            {
                return Unauthorized("Token is incorrect");
            }

            Rating ratingCheck = _repoWrapper.Rating
                .FindByCondition(r => r.UserId == user.Id && r.PodcastId == ratingModel.PodcastId)
                .FirstOrDefault();
            if (ratingCheck != null)
            {
                return BadRequest("Has already been liked");
            }

            Rating rating = new Rating()
            {
                PodcastId = ratingModel.PodcastId,
                isLike = true,
                UserId = user.Id
            };

            _repoWrapper.Rating.Create(rating);
            _repoWrapper.Save();

            return Ok(new { Success = true });

        }
    }
}

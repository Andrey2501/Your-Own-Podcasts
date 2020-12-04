using Contracts;
using Entities;
using Entities.Models;
using Entities.QueryModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using YOP.Models.SubscriptionModel;

namespace YOP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private readonly IRepositoryWrapper _repoWrapper;
        public SubscriptionController(IRepositoryWrapper repoWrapper)
        {
            _repoWrapper = repoWrapper;
        }
        [HttpPost, Authorize]
        public IActionResult Subscribe([FromBody]SubscriptionModel subscriptionModel)
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
            if (user.Id == subscriptionModel.AuthorId)
            {
                return BadRequest("UserId cannot equal AuthorId");
            }
            User author = _repoWrapper.User.FindByCondition(u => u.Id == subscriptionModel.AuthorId).FirstOrDefault();
            if (author == null)
            {
                return NotFound("AuthorId does not exist");
            }
            Subscription subscriptionCheck = _repoWrapper.Subscription
                .FindByCondition(s => s.UserId == user.Id && s.AuthorId == subscriptionModel.AuthorId)
                .FirstOrDefault();

            if (subscriptionCheck != null)
            {
                return BadRequest("Subscription already exists");
            }

            Subscription subscription = new Subscription()
            {
                UserId = user.Id,
                AuthorId = subscriptionModel.AuthorId,
                SubcriptionDate = DateTime.Now
            };

            _repoWrapper.Subscription.Create(subscription);
            _repoWrapper.Save();

            return Ok(new { Success = true });
        }

        [HttpGet, Route("list")]
        public IActionResult GetListSubscription([FromQuery] SubscriptionParameters parameters)
        {

            PagedList<Subscription> subscriptions = _repoWrapper.Subscription.FindByUser(parameters);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(subscriptions.MetaData));

            return Ok(subscriptions);
        }
        [HttpGet, Route("subscribers/list")]
        public IActionResult GetListSubscribers([FromQuery] SubscriptionParameters parameters)
        {

            PagedList<Subscription> subscriptions = _repoWrapper.Subscription.FindByAuthor(parameters);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(subscriptions.MetaData));

            return Ok(subscriptions);
        }
        [HttpDelete, Authorize]
        public IActionResult Delete([FromBody] SubscriptionModel subscriptionModel)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string role = User.FindFirstValue(ClaimTypes.Role);
            Subscription subscription = _repoWrapper.Subscription
                .FindByCondition(p => 
                    p.AuthorId == subscriptionModel.AuthorId && p.UserId.ToString() == userId)
                .FirstOrDefault();

            if (subscription == null)
            {
                return NotFound();
            }

            _repoWrapper.Subscription.Delete(subscription);
            _repoWrapper.Save();

            return Ok(new { Success = true });

        }
    }
}

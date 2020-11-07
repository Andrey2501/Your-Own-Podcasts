using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Contracts;
using Entities;
using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PetControlBackend.Models.UserModel;
using Repository;

namespace PetControlBackend.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IRepositoryWrapper _repoWrapper;

        public UserController(IRepositoryWrapper repoWrapper)
        {
            _repoWrapper = repoWrapper;
        }

        [HttpGet]
        public IActionResult GetUserById()
        {
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (id != null && Guid.TryParse(id, out Guid userID))
            {
                User user = _repoWrapper.User
                    .FindByCondition(u => u.Id == userID)
                    .FirstOrDefault();

                if (user == null)
                {
                    return NotFound("User with id does not exist");
                }

                var config = new MapperConfiguration(cfg => 
                    cfg.CreateMap<User, UserViewModel>());
                var mapper = new Mapper(config);
                UserViewModel userView = mapper.Map<User, UserViewModel>(user);

                return Ok(userView);
            }

            return BadRequest("UserId is incorrect");
        }

        [HttpGet, Route("list"), Authorize(Roles = "Admin")]
        public IActionResult GetUserList([FromQuery] QueryStringParameters parameters)
        {
            PagedList<User> users = _repoWrapper.User.FindAll(parameters);
            var metadata = users.MetaData;
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

            var mapper = new Mapper(new MapperConfiguration(cfg =>
                   cfg.CreateMap<User, UserViewModel>()));
            IEnumerable<UserViewModel> userView = mapper
                .Map<IEnumerable<User>, IEnumerable<UserViewModel>>(users);

            return Ok(userView);
        }

        [HttpPut]
        public IActionResult Edit([FromBody] EditViewModel userEdit)
        {
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string role = User.FindFirstValue(ClaimTypes.Role);

            if (userEdit == null || (userEdit.Id.ToString() != id && role != "Admin"))
            {
                return BadRequest("Editing data is incorrect");
            }

            User user = _repoWrapper.User
                .FindByCondition(u => u.Id == userEdit.Id)
                .FirstOrDefault();

            if (user == null)
            {
                return NotFound();
            }

            var mapper = new Mapper(new MapperConfiguration(cfg =>
                  cfg.CreateMap<EditViewModel, User>()));
            user = mapper.Map<EditViewModel, User>(userEdit);

            _repoWrapper.User.Update(user);

            return Ok(new { Success = true });
        }
    }
}

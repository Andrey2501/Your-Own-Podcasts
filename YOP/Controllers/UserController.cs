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
using YOP.Models.UserModel;
using Repository;
using YOP.Services;
using Entities.QueryModels;

namespace PetControlBackend.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IRepositoryWrapper _repoWrapper;
        private readonly AuthService _authService;

        public UserController(IRepositoryWrapper repoWrapper, AuthService authService)
        {
            _repoWrapper = repoWrapper;
            _authService = authService;
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
        public IActionResult GetUserList([FromQuery] UserParameters parameters)
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
        public IActionResult Edit([FromBody] EditViewModel userViewEdit)
        {
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string role = User.FindFirstValue(ClaimTypes.Role);

            if (userViewEdit == null || (userViewEdit.Id.ToString() != id && role != "Admin"))
            {
                return BadRequest("Editing data is incorrect");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IEnumerable<User> users = _repoWrapper.User
                .FindByCondition(u => u.Id == userViewEdit.Id || u.Email == userViewEdit.Email)
                .ToList();

            if (users == null)
            {
                return NotFound();
            }
            if (users.Count() != 1) 
            {
                return BadRequest("Email alredy exist!");
            }
            User user = users.First();

            var mapper = new Mapper(new MapperConfiguration(cfg =>
                  cfg.CreateMap<EditViewModel, User>()));
            User userEdit = mapper.Map<EditViewModel, User>(userViewEdit);
            userEdit.Password = user.Password;
            userEdit.Role = user.Role;

            _repoWrapper.User.Update(userEdit);
            _repoWrapper.Save();

            return Ok(new { Success = true });
        }
        [HttpPatch, Route("password")]
        public IActionResult ChangePassword([FromBody] EditPasswordUser editPassword)
        {
            if (editPassword == null || editPassword.NewPassword == null) 
            {
                return BadRequest("New password is incorrect");
            }
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (id == null)
            {
                return Unauthorized("Token is incorrect");
            }

            User user = _repoWrapper.User.FindByConditionTrack(u => u.Id.ToString() == id).FirstOrDefault();
            if (user == null)
            {
                return NotFound();
            }
            string hashPassword = _authService.HashPassword(editPassword.NewPassword);
            user.Password = hashPassword;
            _repoWrapper.Save();
            return Ok(new { success = true });
        }
    }
}

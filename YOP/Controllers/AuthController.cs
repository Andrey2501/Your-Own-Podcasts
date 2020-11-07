using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Contracts;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using YOP.Models.AuthModel;
using YOP.Services;

namespace YOP.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IRepositoryWrapper _repoWrapper;
        private readonly IConfiguration _configuration;
        private readonly AuthService _authService;
        public AuthController(IRepositoryWrapper repoWrapper, IConfiguration configuration, AuthService authService)
        {
            _repoWrapper = repoWrapper;
            _configuration = configuration;
            _authService = authService;
        }

        [HttpPost, Route("login")]
        public IActionResult Login([FromBody] LoginModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            User user = _repoWrapper.User
                .FindByCondition(u => u.Email == loginModel.Email)
                .FirstOrDefault();

            if (user == null)
            {
                return Unauthorized("Email is incorrect");
            }

            if (!_authService.VerifyPassword(user.Password, loginModel.Password))
            {
                return Unauthorized("Password is incorrect");
            }

            return Ok(new { token = _authService.GenerateToken(user, _configuration) });
        }

        [HttpPost, Route("register")]
        public IActionResult Register([FromBody]RegisterModel registerModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            User user = _repoWrapper.User
                .FindByCondition(u => u.Email == registerModel.Email)
                .FirstOrDefault();

            if (user != null)
            {
                return Conflict("Email already exists");
            }

            var config = new MapperConfiguration(cfg => cfg.CreateMap<RegisterModel, User>()
                .ForMember("Password", opt => opt.MapFrom(src => _authService.HashPassword(src.Password))));
            var mapper = new Mapper(config);
            User newUser = mapper.Map<RegisterModel, User>(registerModel);
            newUser.Role = "User";
            _repoWrapper.User.Create(newUser);
            _repoWrapper.Save();

            return Ok(new { token = _authService.GenerateToken(newUser, _configuration) });
        }
    }
}

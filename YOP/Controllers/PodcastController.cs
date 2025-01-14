﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Contracts;
using Entities;
using Entities.Models;
using Entities.QueryModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using YOP.Models.PodcastModel;
using YOP.Services;

namespace PetControlBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PodcastController : ControllerBase
    {
        private readonly IRepositoryWrapper _repoWrapper;
        private IWebHostEnvironment _appEnvironment;
        private StatisticService _statisticService;
        public PodcastController(
            IRepositoryWrapper repoWrapper,
            IWebHostEnvironment appEnvironmente)
        {
            _repoWrapper = repoWrapper;
            _appEnvironment = appEnvironmente;
            _statisticService = new StatisticService(_repoWrapper);
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> CreatePodcast(IFormFile uploadedFile, [FromQuery] PodcastCreatModel podcastCreatModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            User user = _repoWrapper.User.FindByCondition(u => u.Id.ToString() == userId).FirstOrDefault();
            if (user == null) 
            {
                return BadRequest("UserId is incorrect");
            }

            Podcast podcast = new Podcast()
            {
                UserId = user.Id,
                Name = podcastCreatModel.Name,
                Genre = podcastCreatModel.Genre,
                PublicationDate = DateTime.Now,
                Views = 0,
                Description = podcastCreatModel.Description
            };
            if (uploadedFile != null)
            {
                string extension = uploadedFile.FileName.Substring(uploadedFile.FileName.LastIndexOf('.'));
                podcast.ContentType = uploadedFile.ContentType;
                podcast.FileName = $"{podcast.Id + extension}";
                string path = Path.Combine(_appEnvironment.ContentRootPath, "Storage", $"{podcast.Id + extension}");
                string subPath = Path.Combine(_appEnvironment.ContentRootPath, "Storage");

                bool exists = Directory.Exists(subPath);

                if (!exists) 
                {
                    Directory.CreateDirectory(subPath);
                }
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }
            }
            else
            {
                return BadRequest("File is incorrect");
            }
            _repoWrapper.Podcast.Create(podcast);
            _repoWrapper.Save();

            var mapper = new Mapper(new MapperConfiguration(cfg =>
                   cfg.CreateMap<Podcast, PodcastGetModel>()));
            PodcastGetModel podcastGetModel = mapper
                .Map<Podcast, PodcastGetModel>(podcast);

            return Ok(podcastGetModel);
        }

        [HttpGet, Route("list")]
        public IActionResult GetListPodcasts([FromQuery] PodcastsParametrs parameters)
        {
            string strUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid userId = string.IsNullOrEmpty(strUserId) ? Guid.NewGuid() : new Guid(strUserId);

            PagedList <Podcast> podcasts = _repoWrapper.Podcast.FindAll(parameters);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(podcasts.MetaData));

            List<PodcastGetModel> podcastModel = _statisticService.TransformPodcast(podcasts, userId);

            return Ok(podcastModel);
        }
        [HttpGet, Route("user/list")]
        public IActionResult GetPodcastsOfUser([FromQuery] PodcastsUserIdParameters parameters)
        {
            User user = _repoWrapper.User
                .FindByCondition(u => u.Id == parameters.UserId)
                .FirstOrDefault();

            if (user == null)
            {
                return NotFound("UserId is incorrect");
            }
            PagedList<Podcast> podcasts = _repoWrapper.Podcast.FindByUserId(parameters);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(podcasts.MetaData));

            List<PodcastGetModel> podcastModel = _statisticService.TransformPodcast(podcasts, user.Id);

            return Ok(podcastModel);
        }

        [HttpGet("{id}")]
        public IActionResult GetListPocasts(string id)
        {
            Podcast podcast = _repoWrapper.Podcast.FindByCondition(p => p.Id.ToString() == id).FirstOrDefault();
            if (podcast == null)
            {
                return BadRequest("PodcastId is incorrect");
            }


            return Ok(podcast);
        }
        [HttpGet("file/{fileName}")]
        public IActionResult GetFilePodcast(string fileName)
        {
            string filePath = Path.Combine(_appEnvironment.ContentRootPath, "Storage", fileName);
            string podcastId = fileName.Substring(0, fileName.LastIndexOf('.'));
            Podcast podcast = _repoWrapper.Podcast
                .FindByCondition(p => p.Id.ToString() == podcastId)
                .FirstOrDefault();

            if (podcast == null)
            {
                return NotFound();
            }

            return PhysicalFile(filePath, podcast.ContentType);
        }

        [HttpDelete("{id}"), Authorize]
        public IActionResult DeleteById(Guid id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string role = User.FindFirstValue(ClaimTypes.Role);
            Podcast podcast = _repoWrapper.Podcast
                .FindByCondition(p => p.Id == id && (p.UserId.ToString() == userId || role == "Admin"))
                .FirstOrDefault();

            if (podcast == null)
            {
                return NotFound();
            }

            _repoWrapper.Podcast.Delete(podcast);
            _repoWrapper.Save();

            return Ok(new { Success = true });

        }
    }
}

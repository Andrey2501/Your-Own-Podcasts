using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Contracts;
using Entities;
using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using YOP.Models.PodcastModel;

namespace PetControlBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PodcastController : ControllerBase
    {
        private readonly IRepositoryWrapper _repoWrapper;
        private IWebHostEnvironment _appEnvironment;
        public PodcastController(IRepositoryWrapper repoWrapper, IWebHostEnvironment appEnvironment)
        {
            _repoWrapper = repoWrapper;
            _appEnvironment = appEnvironment;
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

            return Ok(new { PodcastId = podcast.Id });
        }

        [HttpGet, Route("list")]
        public IActionResult GetListPocasts([FromQuery] QueryStringParameters parameters)
        {
            PagedList<Podcast> podcasts = _repoWrapper.Podcast.FindAll(parameters);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(podcasts.MetaData));

            return Ok(podcasts);
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
    }
}

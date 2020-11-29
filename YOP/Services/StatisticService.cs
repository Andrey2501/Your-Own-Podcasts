using AutoMapper;
using Contracts;
using Entities;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YOP.Models.PodcastModel;

namespace YOP.Services
{
    public class StatisticService
    {
        private readonly IRepositoryWrapper _repoWrapper;
        public StatisticService(IRepositoryWrapper repoWrapper)
        {
            _repoWrapper = repoWrapper;
        }
        public int GetCountLikesByPodcastId(Guid podcastId)
        {
            return _repoWrapper.Rating.FindByCondition(r => r.PodcastId == podcastId).Count();
        }
        public bool CheckLike(Guid podcastId, Guid userId)
        {
            Rating rating = _repoWrapper.Rating
                .FindByCondition(r => r.PodcastId == podcastId && r.UserId == userId)
                .FirstOrDefault();

            return rating != null;
        }
        public List<PodcastGetModel> TransformPodcast(PagedList<Podcast> podcasts, Guid userId)
        {
            var mapper = new Mapper(new MapperConfiguration(cfg =>
                  cfg.CreateMap<Podcast, PodcastGetModel>()
                    .ForMember("CountLikes", opt => opt.MapFrom(podcast => GetCountLikesByPodcastId(podcast.Id)))
                    .ForMember("IsLike", opt => opt.MapFrom(podcast => CheckLike(podcast.Id, userId)))
            ));

            return mapper.Map<List<Podcast>, List<PodcastGetModel>>(podcasts);
        }
    }
}

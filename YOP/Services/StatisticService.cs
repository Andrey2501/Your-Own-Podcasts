using AutoMapper;
using Contracts;
using Entities;
using Entities.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YOP.Models.PodcastModel;
using YOP.Models.SubscriptionModel;

namespace YOP.Services
{
    public class StatisticService
    {
        private readonly IRepositoryWrapper _repoWrapper;
        public StatisticService(IRepositoryWrapper repoWrapper)
        {
            _repoWrapper = repoWrapper;
        }
        public int GetCountLikesByUserId(Guid userId)
        {
            string connectionString = @"Data Source=yop.database.windows.net;Initial Catalog=yop;Persist Security Info=True;User ID=Andrii;Password=Andrey645";
            string sqlExpression = "Select COUNT(*) From [dbo].[Rating] as rating Left join[dbo].[Podcast] as podcast on rating.PodcastId = podcast.id WHERE podcast.UserId = '" + userId.ToString() + "'";
            object countLikes;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sqlExpression, connection);
                    countLikes = command.ExecuteScalar();
                }
            }
            catch
            {
                countLikes = 0;
            }
            return (int)countLikes;
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
        public string GetAuthorName(Guid userId)
        {
            User user = _repoWrapper.User
                .FindByCondition(u => u.Id == userId)
                .FirstOrDefault();

            string name = "Unknown";
            if (user != null)
            {
                name = user.LastName != null ? $"{user.FirstName} {user.LastName}" : user.FirstName;
            }
            return name;
        }
        public List<PodcastGetModel> TransformPodcast(PagedList<Podcast> podcasts, Guid userId)
        {
            var mapper = new Mapper(new MapperConfiguration(cfg =>
                  cfg.CreateMap<Podcast, PodcastGetModel>()
                    .ForMember("CountLikes", opt => opt.MapFrom(podcast => GetCountLikesByPodcastId(podcast.Id)))
                    .ForMember("IsLike", opt => opt.MapFrom(podcast => CheckLike(podcast.Id, userId)))
                    .ForMember("AuthorName", opt => opt.MapFrom(podcast => GetAuthorName(podcast.UserId)))
            ));

            return mapper.Map<List<Podcast>, List<PodcastGetModel>>(podcasts);
        }

        public List<SubscriptionGetModel> TransformSubscription(PagedList<Subscription> subscriptions)
        {
            var mapper = new Mapper(new MapperConfiguration(cfg =>
                  cfg.CreateMap<Subscription, SubscriptionGetModel>()
                    .ForMember("CountLikes", opt => opt.MapFrom(subscription => GetCountLikesByUserId(subscription.AuthorId)))
                    .ForMember("AuthorName", opt => opt.MapFrom(subscription => GetAuthorName(subscription.AuthorId)))
            ));

            return mapper.Map<List<Subscription>, List<SubscriptionGetModel>>(subscriptions);
        }
    }
}

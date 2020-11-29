using Contracts;
using Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repository
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private readonly RepositoryContext _repoContext;
        private IUserRepository _user;
        private IPodcastRepository _podcast;
        private ICommentRepository _comment;
        private IRatingRepository _rating;
        private ISubscriptionRepository _subscription;
        private IPlaylistRepository _playlist;
        private IPodcastToPlaylistRepository _podcastToPlaylist;
        public IUserRepository User
        {
            get
            {
                if (_user == null)
                {
                    _user = new UserRepository(_repoContext);
                }
                return _user;
            }
        }
        public IPodcastRepository Podcast
        {
            get
            {
                if (_podcast == null)
                {
                    _podcast = new PodcastRepository(_repoContext);
                }
                return _podcast;
            }
        }
        public ISubscriptionRepository Subscription
        {
            get
            {
                if (_subscription == null)
                {
                    _subscription = new SubscriptionRepository(_repoContext);
                }
                return _subscription;
            }
        }
        public ICommentRepository Comment
        {
            get
            {
                if (_comment == null)
                {
                    _comment = new CommentRepository(_repoContext);
                }
                return _comment;
            }
        }
        public IRatingRepository Rating
        {
            get
            {
                if (_rating == null)
                {
                    _rating = new RatingRepository(_repoContext);
                }
                return _rating;
            }
        }
        public IPlaylistRepository Playlist
        {
            get
            {
                if (_playlist == null)
                {
                    _playlist = new PlaylistRepository(_repoContext);
                }
                return _playlist;
            }
        }
        public IPodcastToPlaylistRepository PodcastToPlaylist
        {
            get
            {
                if (_podcastToPlaylist == null)
                {
                    _podcastToPlaylist = new PodcastToPlaylistRepository(_repoContext);
                }
                return _podcastToPlaylist;
            }
        }
        public RepositoryWrapper(RepositoryContext repositoryContext)
        {
            _repoContext = repositoryContext;
        }
        public void Save()
        {
            _repoContext.SaveChanges();
        }
    }
}

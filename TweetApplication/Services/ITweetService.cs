using com.tweetapp.Models;
using com.tweetapp.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace com.tweetapp.Services
{
    public interface ITweetService
    {
        //public void Register(User user);

        //public void Login(string username, string password);

        //public void ForgotPassword(string username);

        public Task<IEnumerable<Tweet>> GetAllTweets();

        //public List<User> GetAllUsers();

        //public User SearchUser(string username);

        public Task<IEnumerable<Tweet>> GetUserTweets(string username);

        public Task<Tweet> AddNewTweet(PostNewTweetDto tweet, string username);

        public Task<Tweet> UpdateTweet(string username, string id, string message);

        public Task<bool> DeleteTweet(string username, string id);

        public Task<int> LikeUnlikeTweet(string username, string id);

        public Task<bool> Reply(string username, string id, string message);
    }
}

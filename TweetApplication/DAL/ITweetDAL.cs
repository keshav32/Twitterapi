using com.tweetapp.Models;
using com.tweetapp.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace com.tweetapp.DAL
{
    public interface ITweetDAL
    {
        //public void Register(User user);

        //public void Login(string username, string password);

        //public void ForgotPassword(string username);

        public Task<IEnumerable<Tweet>> GetAllTweets();

        //public List<User> GetAllUsers();

        //public User SearchUser(string username);

        public Task<IEnumerable<Tweet>> GetUserTweets(string username);

        public Task<Tweet> AddNewTweet(Tweet tweet);

        public Task<Tweet> UpdateTweet(string username, string id, string newMessage);

        public Task<bool> DeleteTweet(string username, string id);

        public Task<int> LikeTweet(string username, string id);

        public Task<bool> Reply(string id, ReplyTweet replyTweet);
    }
}

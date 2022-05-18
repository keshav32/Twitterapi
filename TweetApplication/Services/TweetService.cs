using AutoMapper;
using com.tweetapp.DAL;
using com.tweetapp.Models;
using com.tweetapp.Models.Dtos;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace com.tweetapp.Services
{
    public class TweetService: ITweetService
    {
        private readonly ITweetDAL tweetDAL;

        private readonly IMapper mapper;

        private readonly IUserDAL userDAL;

        public TweetService(ITweetDAL _tweetDAL, IMapper _mapper, IUserDAL _userDAL)
        {
            tweetDAL = _tweetDAL;
            mapper = _mapper;
            userDAL = _userDAL;
        }

        public async Task<IEnumerable<Tweet>> GetAllTweets()
        {
            try
            {
                var tweets = await tweetDAL.GetAllTweets();
                if (tweets != null)
                {
                    return tweets.OrderByDescending(m => m.DateAndTimeofTweet);
                }
                return tweets;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<IEnumerable<Tweet>> GetUserTweets(string username)
        {
            try
            {
                User loggedUser = await userDAL.GetLoggedInUser();
                if (loggedUser.EmailId == username)
                {
                    var tweets = await tweetDAL.GetUserTweets(username);
                    if (tweets.Count() > 0)
                    {
                        return tweets.OrderByDescending(m => m.DateAndTimeofTweet);
                    }
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<Tweet> AddNewTweet(PostNewTweetDto tweet, string username)
        {
            try
            {
                User loggedUser = await userDAL.GetLoggedInUser();
                if (loggedUser.EmailId == username)
                {
                    var newTweet = mapper.Map<Tweet>(tweet);
                    //var userDetail = await userDAL.SearchUser(username);
                    newTweet.Likes = 0;
                    newTweet.Username = username;
                    newTweet.Replies = new List<ReplyTweet>();
                    newTweet.DateAndTimeofTweet = DateTime.Now;
                    newTweet.LikedBy = new string[] { };
                    return await tweetDAL.AddNewTweet(newTweet);
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<Tweet> UpdateTweet(string username, string id, string newMessage)
        {
            try
            {
                User loggedUser = await userDAL.GetLoggedInUser();
                if (loggedUser.EmailId == username)
                {
                    return await tweetDAL.UpdateTweet(username, id, newMessage);
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> DeleteTweet(string username, string id)
        {
            try
            {
                User loggedUser = await userDAL.GetLoggedInUser();
                if (loggedUser.EmailId == username)
                {
                    return await tweetDAL.DeleteTweet(username, id);
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<int> LikeUnlikeTweet(string username, string id)
        {
            try
            {
                User loggedUser = await userDAL.GetLoggedInUser();
                if (loggedUser.EmailId == username)
                {
                    return await tweetDAL.LikeTweet(username, id);
                }
                return -1;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public async Task<bool> Reply(string username, string id, string message)
        {
            try
            {
                User loggedUser = await userDAL.GetLoggedInUser();
                if (loggedUser.EmailId == username)
                {
                    ReplyTweet replyTweet = new ReplyTweet
                    {
                        ReplyTweetText = message,
                        Username = username,
                        DateAndTimeOfReply = DateTime.Now
                    };

                    return await tweetDAL.Reply(id, replyTweet);
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /*public void ForgotPassword(string username)
        {
            throw new NotImplementedException();
        }*/


        /*public List<User> GetAllUsers()
        {
            return tweetDAL.GetAllUsers();
        }*/

        /*public Tweet LikeTweet(string username, int id)
        {
            throw new NotImplementedException();
        }

        public void Login(string username, string password)
        {
            throw new NotImplementedException();
        }

        public void Register(User user)
        {
            tweetDAL.Register(user);
        }

        public User SearchUser(string username)
        {
            throw new NotImplementedException();
        }*/
    }
}

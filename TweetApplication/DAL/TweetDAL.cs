using com.tweetapp.Models;
using com.tweetapp.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace com.tweetapp.DAL
{
    public class TweetDAL : ITweetDAL
    {
        private readonly IConfiguration configuration;

        public TweetDAL(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        /*public void Register(User user)
        {
            MongoClient dbClient = new MongoClient(configuration.GetConnectionString("TweetAppCon"));

            dbClient.GetDatabase("TweetAppDb").GetCollection<User>("User").InsertOne(user);
        }

        public void Login(string username, string password)
        {
            MongoClient dbClient = new MongoClient(configuration.GetConnectionString("TweetAppCon"));

            dbClient.GetDatabase("TweetAppDb").GetCollection<User>("User").Find($"{{ Username : username }},{{ Password : password}}");
        }

        public void ForgotPassword(string username)
        {
            throw new NotImplementedException();
        }*/

        public async Task<IEnumerable<Tweet>> GetAllTweets()
        {
            MongoClient dbClient = new MongoClient(configuration.GetConnectionString("TweetAppCon"));

            var dbList = await dbClient.GetDatabase("TweetAppDb").GetCollection<Tweet>("Tweet").Find(t => true).ToListAsync();// .AsQueryable();

            return dbList;
        }

        /*public List<User> GetAllUsers()
        {
            MongoClient dbClient = new MongoClient(configuration.GetConnectionString("TweetAppCon"));

            var dbList = dbClient.GetDatabase("TweetAppDb").GetCollection<User>("User").AsQueryable();

            return dbList.ToList();
        }

        public User SearchUser(string username)
        {
            MongoClient dbClient = new MongoClient(configuration.GetConnectionString("TweetAppCon"));

            var filter = Builders<User>.Filter.Eq("Username", username);
            var user = dbClient.GetDatabase("TweetAppDb").GetCollection<User>("User").Find(filter).FirstOrDefault();

            return user;
        }*/

        public async Task<IEnumerable<Tweet>> GetUserTweets(string username)
        {
            MongoClient dbClient = new MongoClient(configuration.GetConnectionString("TweetAppCon"));

            var filter = Builders<Tweet>.Filter.Eq("username", username);

            var dbList = await dbClient.GetDatabase("TweetAppDb").GetCollection<Tweet>("Tweet").Find(filter).ToListAsync();

            return dbList;
        }

        public async Task<Tweet> AddNewTweet(Tweet tweet)
        {
            MongoClient dbClient = new MongoClient(configuration.GetConnectionString("TweetAppCon"));

            await dbClient.GetDatabase("TweetAppDb").GetCollection<Tweet>("Tweet").InsertOneAsync(tweet);

            return tweet;
        }

        public async Task<Tweet> UpdateTweet(string username, string id, string newMessage)
        {
            MongoClient dbClient = new MongoClient(configuration.GetConnectionString("TweetAppCon"));

            var filter = Builders<Tweet>.Filter.Eq(t => t.Id, id);

            var update = Builders<Tweet>.Update.Set("tweetMessage" , newMessage);
            
            await dbClient.GetDatabase("TweetAppDb").GetCollection<Tweet>("Tweet").FindOneAndUpdateAsync(filter, update);

            return dbClient.GetDatabase("TweetAppDb").GetCollection<Tweet>("Tweet").Find(filter).FirstOrDefault();
        }

        public async Task<bool> DeleteTweet(string username, string id)
        {
            MongoClient dbClient = new MongoClient(configuration.GetConnectionString("TweetAppCon"));

            var filter = Builders<Tweet>.Filter.Eq(t => t.Id, id) & Builders<Tweet>.Filter.Eq(t => t.Username, username);

            var res = await dbClient.GetDatabase("TweetAppDb").GetCollection<Tweet>("Tweet").DeleteOneAsync(filter);

            return res.DeletedCount > 0;
        }

        public async Task<int> LikeTweet(string username, string id)
        {
            MongoClient dbClient = new MongoClient(configuration.GetConnectionString("TweetAppCon"));

            var tweetDetail = await dbClient.GetDatabase("TweetAppDb").GetCollection<Tweet>("Tweet").Find(t => t.Id == id).FirstOrDefaultAsync();
            int likes = tweetDetail.Likes;
            bool isAlreadyLiked = tweetDetail.LikedBy.Contains(username); ;
            if (isAlreadyLiked)
            {
                likes = likes - 1;
                var pushElement = Builders<Tweet>.Update.Combine(
                   Builders<Tweet>.Update.Pull(x => x.LikedBy, username),
                   Builders<Tweet>.Update.Set(x => x.Likes, likes)
                );
                
                await dbClient.GetDatabase("TweetAppDb").GetCollection<Tweet>("Tweet").UpdateOneAsync(s => s.Id == id, pushElement);
            }
            else
            {
                likes = likes + 1;
                var pushElement = Builders<Tweet>.Update.Combine(
                    Builders<Tweet>.Update.Push(x => x.LikedBy, username),
                    Builders<Tweet>.Update.Set(x => x.Likes, likes)
                    );
                pushElement.Set(t => t.Likes, likes);
                await dbClient.GetDatabase("TweetAppDb").GetCollection<Tweet>("Tweet").UpdateOneAsync(s => s.Id == id, pushElement);
            }
            return likes;
        }

        public async Task<bool> Reply(string id, ReplyTweet replyTweet)
        {
            MongoClient dbClient = new MongoClient(configuration.GetConnectionString("TweetAppCon"));

            var filter = Builders<Tweet>.Filter.Eq(t => t.Id, id);

            var pushElement = Builders<Tweet>.Update.Push(t => t.Replies, replyTweet);
            
            var result = await dbClient.GetDatabase("TweetAppDb").GetCollection<Tweet>("Tweet").UpdateOneAsync(filter, pushElement);
            
            return result.ModifiedCount > 0;
        }
    }
}

using com.tweetapp.Models;
using com.tweetapp.Models.Dtos;
using com.tweetapp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace com.tweetapp.Controllers
{
    [Route("api/v1.0/tweets")]
    [ApiController]
    [Authorize]
    public class TweetController : ControllerBase
    {
        private readonly ITweetService tweetService;

        public TweetController(ITweetService _tweetService)
        {
            tweetService = _tweetService;
        }

        //POST /api/v1.0/tweets/register Register as new user
        /*[HttpPost]
        [Route("register")]
        public JsonResult Register(User user)
        {
            tweetService.Register(user);

            return new JsonResult("Registered Successfully!");
        }

        //GET /api/v1.0/tweets/login Login
        [HttpGet]
        [Route("login")]
        public JsonResult Login(string username, string password)
        {
            tweetService.Login(username, password);

            return new JsonResult("Logged in successfully!");
        }

        //GET /api/v1.0/tweets/<username>/forgot Forgot password
        [HttpGet]
        [Route("{username}/forgot")]
        public JsonResult ForgotPassword(string username)
        {
            tweetService.ForgotPassword(username);

            return new JsonResult("Password changed successfully!");
        }*/

        //GET /api/v1.0/tweets/all Get all tweets
        [HttpGet]
        [Route("all")]
        public async Task<IActionResult> GetAllTweets()
        {
            var tweetList = await tweetService.GetAllTweets();

            if(tweetList != null)
            {
                return Ok(tweetList);
            }

            return NoContent();
        }

        /*//GET /api/v1.0/tweets/users/all Get all users
        [HttpGet]
        [Route("users/all")]
        public JsonResult GetAllUsers()
        {
            var userList = tweetService.GetAllUsers();

            return new JsonResult(userList);
        }

        //GET /api/v/1.0/tweets/user/search/username* Search by username
        [HttpGet]
        public JsonResult SearchUser(string username)
        {
            User user = tweetService.SearchUser(username);

            return new JsonResult(user);
        }*/

        //GET /api/v1.0/tweets/username Get all tweets of user
        [HttpGet]
        [Route("{username}")]
        public async Task<IActionResult> GetUserTweets(string username)
        {
            var userTweetList = await tweetService.GetUserTweets(username);

            if(userTweetList != null)
            {
                return Ok(userTweetList);
            }
            return NoContent();
        }

        //POST /api/v1.0/tweets/<username>/add Post new tweet
        [HttpPost]
        [Route("{username}/add")]
        public async Task<IActionResult> AddNewTweet(PostNewTweetDto tweet, string username)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var res = await tweetService.AddNewTweet(tweet, username);

            if(res != null)
            {
                return Ok("New tweet posted!");
            }

            return BadRequest(); 
        }

        //PUT /api/v1.0/tweets/<username>/update/<id> Update tweet
        [HttpPut]
        [Route("{username}/update/{id}")]
        public async Task<IActionResult> UpdateTweet(string username, string id, [FromBody] string newMessage)
        {
            Tweet updatedTweet = await tweetService.UpdateTweet(username, id, newMessage);

            if(updatedTweet != null)
            {
                return Ok(updatedTweet);
            }

            return BadRequest();
        }

        //DELETE /api/v1.0/tweets/<username>/delete/<id> Delete tweet
        [HttpDelete]
        [Route("{username}/delete/{id}")]
        public async Task<IActionResult> DeleteTweet(string username, string id)
        {
            var res = await tweetService.DeleteTweet(username, id);

            if(res == true)
            {
                return Ok("Deleted Tweet!");
            }

            return BadRequest();
        }

        //PUT /api/v1.0/tweets/<username>/like/<id> Like tweet
        [HttpPut]
        [Route("{username}/like/{id}")]
        public async Task<IActionResult> LikeUnlikeTweet(string username, string id)
        {
            var res = await tweetService.LikeUnlikeTweet(username, id);
            
            if(res >= 0)
            {
                return Ok(res);
            }

            return BadRequest();
        }

        //POST /api/v1.0/tweets/<username>/reply/<id> Reply to tweet
        [HttpPost]
        [Route("{username}/reply/{id}")]
        public async Task<IActionResult> Reply(string username, string id, [FromBody] string message)
        {
            if(message == null)
            {
                return BadRequest();
            }
            
            var res = await tweetService.Reply(username, id, message);
            if (res == true)
                return Ok(res);
            else
                return BadRequest("Not able to reply to tweet!");
        }
    }
}

using com.tweetapp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace com.tweetapp.DAL
{
    public class UserDAL : IUserDAL
    {
        private readonly IConfiguration configuration;
        private readonly IHttpContextAccessor httpContextAccessor;

        public readonly string key;

        public UserDAL(IConfiguration _configuration, IHttpContextAccessor _httpContextAccessor)
        {
            configuration = _configuration;
            httpContextAccessor = _httpContextAccessor;
            key = _configuration.GetSection("ApplicationSettings:JWT_Secret").Value;
        }

        public async Task<bool> Register(User user)
        {
            
            MongoClient dbClient = new MongoClient(configuration.GetConnectionString("TweetAppCon"));

            await dbClient.GetDatabase("TweetAppDb").GetCollection<User>("User").InsertOneAsync(user);

            return true;
        }

        public async Task<string> Login(string username, string password)
        {
            MongoClient dbClient = new MongoClient(configuration.GetConnectionString("TweetAppCon"));

            //User user = dbClient.GetDatabase("TweetAppDb").GetCollection<User>("User").Find(u => u.Email == username && u.Password == password).FirstOrDefault();
            
            //var filter = Builders<User>.Filter.Eq("Email", username);
            //var user = dbClient.GetDatabase("TweetAppDb").GetCollection<User>("User").Find(filter).FirstOrDefault();
            
            //var user = dbClient.GetDatabase("TweetAppDb").GetCollection<User>("User").Find($"{{ Email : '{username}' }}").FirstOrDefault();//Single();

            User user = await dbClient.GetDatabase("TweetAppDb").GetCollection<User>("User").Find($"{{ emailId : '{username}' , password : '{password}' }}").FirstOrDefaultAsync();

            if (user == null)
                return null;

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim("Username", user.EmailId)
                    }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(securityToken);

            return token;
        }

        
        public async Task<User> GetLoggedInUser()
        {
            //First get user claims    
            var claims = httpContextAccessor.HttpContext.User.Identities.First().Claims.ToList();

            //Filter specific claim    
            var username = claims?.FirstOrDefault(x => x.Type.Equals("Username"))?.Value;

            MongoClient dbClient = new MongoClient(configuration.GetConnectionString("TweetAppCon"));

            User user = await dbClient.GetDatabase("TweetAppDb").GetCollection<User>("User").Find(u => u.EmailId == username).FirstOrDefaultAsync();

            return user;
        }

        public async Task<bool> ForgotPassword(string username, string oldPassword, string newPassword)
        {
            MongoClient dbClient = new MongoClient(configuration.GetConnectionString("TweetAppCon"));

            var user = await dbClient.GetDatabase("TweetAppDb").GetCollection<User>("User").Find($"{{ emailId : '{username}' }}").FirstOrDefaultAsync();

            if(user.Password == oldPassword)
            {
                var filter = Builders<User>.Filter.Eq("EmailId", username);
                var update = Builders<User>.Update.Set("Password", newPassword);

                var res = await dbClient.GetDatabase("TweetAppDb").GetCollection<User>("User").UpdateOneAsync(filter, update);

                return res.ModifiedCount > 0;
            }

            return false;
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            MongoClient dbClient = new MongoClient(configuration.GetConnectionString("TweetAppCon"));

            var dbList = await dbClient.GetDatabase("TweetAppDb").GetCollection<User>("User").Find(u => true).ToListAsync();

            return dbList;
        }

        public async Task<User> SearchUser(string username)
        {
            MongoClient dbClient = new MongoClient(configuration.GetConnectionString("TweetAppCon"));

            var filter = Builders<User>.Filter.Eq("emailId", username);
            var user = await dbClient.GetDatabase("TweetAppDb").GetCollection<User>("User").Find(filter).FirstOrDefaultAsync();

            return user;
        }

        public async Task<bool?> IsEmailIdAlreadyTaken(string username)
        {
            MongoClient dbClient = new MongoClient(configuration.GetConnectionString("TweetAppCon"));

            var filter = Builders<User>.Filter.Eq("EmailId", username);
            var user = await dbClient.GetDatabase("TweetAppDb").GetCollection<User>("User").Find(filter).FirstOrDefaultAsync();

            if (user != null)
                return true;

            return false;
        }
    }

    public static class ClaimsPrincipalExtensions
    {
        public static T GetLoggedInUserId<T>(this ClaimsPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            var loggedInUserId = principal.FindFirstValue(ClaimTypes.NameIdentifier);

            if (typeof(T) == typeof(string))
            {
                return (T)Convert.ChangeType(loggedInUserId, typeof(T));
            }
            else if (typeof(T) == typeof(int) || typeof(T) == typeof(long))
            {
                return loggedInUserId != null ? (T)Convert.ChangeType(loggedInUserId, typeof(T)) : (T)Convert.ChangeType(0, typeof(T));
            }
            else
            {
                throw new Exception("Invalid type provided");
            }
        }

        public static string GetLoggedInUserName(this ClaimsPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            return principal.FindFirstValue(ClaimTypes.Name);
        }

        public static string GetLoggedInUserEmail(this ClaimsPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            return principal.FindFirstValue(ClaimTypes.Email);
        }
    }
}

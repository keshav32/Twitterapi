using com.tweetapp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace com.tweetapp.DAL
{
    public interface IUserDAL
    {
        public Task<bool> Register(User user);

        public Task<string> Login(string username, string password);

        public Task<User> GetLoggedInUser();

        public Task<bool> ForgotPassword(string username, string oldPassword, string newPassword);

        public Task<IEnumerable<User>> GetAllUsers();

        public Task<User> SearchUser(string username);

        public Task<bool?> IsEmailIdAlreadyTaken(string emailId);
    }
}

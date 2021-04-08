using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using AaaaperoBack.Data;
using AaaaperoBack.Helpers;
using AaaaperoBack.Models;

namespace AaaaperoBack.Services
{
    public interface IUserService
    {
        User Authenticate(string username, string password);
        IEnumerable<User> GetAll();
        User GetById(int id);
        User Create(User user, string password);
        void Update(User user, string currentPassword, string password, string confirmPassword);
        string ForgotPassword(string username);
        User ResetPassword(string username, string currentPassword, string password, string confirmPassword);
        void Delete(int id);
    }

    public class UserService : IUserService
    {
        private Context _context;
        private readonly IEmailService _emailService;

        public UserService(Context context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }
        
        /// <summary>
        /// Authenticate the user, if it exists
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public User Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return null;
            }

            var user = _context.User.FirstOrDefault(x => x.Username == username) ?? null;

            // check if username exists
            if (user == null)
            {
                return null;
            }

            // Granting access if the hashed password in the database matches with the password(hashed in computeHash method) entered by user.
            if(computeHash(password) != user.PasswordHash)
            {
                return null;
            }
            return user;        
        }
        
        /// <summary>
        /// Show all the users' data
        /// </summary>
        /// <returns></returns>
        public IEnumerable<User> GetAll()
        {
            return _context.User;
        }
        
        /// <summary>
        /// Show the selected user's data
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public User GetById(int id)
        {
            return _context.User.Find(id);
        }
        
        /// <summary>
        /// Create a new User
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        /// <exception cref="AppException"></exception>
        public User Create(User user, string password)
        {
            // validation
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new AppException("Password is required");
            }

            if (_context.User.Any(x => x.Username == user.Username))
            {
                throw new AppException("Username \"" + user.Username + "\" is already taken");
            }

            //Saving hashed password into Database table
            user.PasswordHash = computeHash(password);  

            _context.User.Add(user);
            _context.SaveChanges();

            return user;
        }
        
        /// <summary>
        /// Update the user's informations
        /// </summary>
        /// <param name="userParam"></param>
        /// <param name="currentPassword"></param>
        /// <param name="password"></param>
        /// <param name="confirmPassword"></param>
        /// <exception cref="AppException"></exception>
        public void Update(User userParam, string currentPassword = null, string password = null, string confirmPassword = null)
        {
            //Find the user by Id
            var user = _context.User.Find(userParam.Id);

            if (user == null) 
            {
                throw new AppException("User not found");
            }
            // update user properties if provided
            if (!string.IsNullOrWhiteSpace(userParam.Username) && userParam.Username != user.Username)
            {
                // throw error if the new username is already taken
                if (_context.User.Any(x => x.Username == userParam.Username))
                {
                    throw new AppException("Username " + userParam.Username + " is already taken");
                }
                else
                {
                    user.Username = userParam.Username;
                }
            }
            if (!string.IsNullOrWhiteSpace(userParam.FirstName))
            {
                user.FirstName = userParam.FirstName;
            }
            if (!string.IsNullOrWhiteSpace(userParam.LastName))
            {
                user.LastName = userParam.LastName;
            }
            if (!string.IsNullOrWhiteSpace(currentPassword))
            {   
                if(computeHash(currentPassword) != user.PasswordHash)
                {
                    throw new AppException("Invalid Current password!");
                }

                if(currentPassword == password)
                {
                    throw new AppException("Please choose another password!");
                }

                if(password != confirmPassword)
                {
                    throw new AppException("Password doesn't match!");
                }
    
                //Updating hashed password into Database table
                user.PasswordHash = computeHash(password); 
            }
            
            _context.User.Update(user);
            _context.SaveChanges();
        }
        
        /// <summary>
        /// Delete the selected User
        /// </summary>
        /// <param name="id"></param>
        public void Delete(int id)
        {
            var user = _context.User.Find(id);
            if (user != null)
            {
                _context.User.Remove(user);
                _context.SaveChanges();
            }
        }
        
        /// <summary>
        /// Create a password hash
        /// </summary>
        /// <param name="Password"></param>
        /// <returns></returns>
        private static string computeHash(string Password)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            var input = md5.ComputeHash(Encoding.UTF8.GetBytes(Password));
            var hashstring = "";
            foreach(var hashbyte in input)
            {
                hashstring += hashbyte.ToString("x2"); 
            } 
            return hashstring;
        }
        
        /// <summary>
        /// Allow the user, if it exists, to receive an email to reset its password.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        /// <exception cref="AppException"></exception>
        public string ForgotPassword(string username)
        {
            if(string.IsNullOrEmpty(username))
            {
                throw new AppException("Valid Username is requred");
            }
            else
            {
                var user = _context.User.SingleOrDefault(x => x.Username == username);
                if(user != null)
                {
                    string token = GenerateRandomCryptographicKey(25);
                    user.Token = token;
                    _context.SaveChanges();
                    //USER.EMAIL IT WAS A TRAP !!!
                    var emailAddress = new List<string>(){user.Email};
                    var emailSubject = "Password Recovery";
                    var messageBody = "Token : " + token;

                    var response = _emailService.SendEmailAsync(emailAddress,emailSubject,messageBody);
                    System.Console.WriteLine(response.Result.StatusCode);

                    if(response.IsCompletedSuccessfully)
                    {
                        System.Console.WriteLine(emailAddress);
                        return new string("If your account exists, your new password will be emailed to you shortly");
                    }
                }
                return new string("If your account exists, your new password will be emailed to you shortly");
            }
        }
        
        /// <summary>
        /// Allow reset password.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="currentPassword"></param>
        /// <param name="password"></param>
        /// <param name="confirmPassword"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public User ResetPassword(string username, string currentPassword, string password, string confirmPassword)
        {
            throw new NotImplementedException();
        }

        // helper method to generate random password
        private static string GenerateRandomCryptographicKey(int keyLength)
        {
            RNGCryptoServiceProvider rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            byte[] randomBytes = new byte[keyLength];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            string hashstring = "";
            foreach(var hashbyte in randomBytes)
            {
                hashstring += hashbyte.ToString("x2"); 
            }
            return hashstring;
        }
        
    }
}
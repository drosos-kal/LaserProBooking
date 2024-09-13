using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotVVM.Framework.ViewModel;
using DotVVM.Framework.Hosting;
using BeautySalonBookingSystem.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using BeautySalonBookingSystem.Models.DTO;
using BeautySalonBookingSystem.Schedulers;
using BeautySalonBookingSystem.Models.Entities;

namespace BeautySalonBookingSystem.ViewModels
{
    public class LoginViewModel : MasterPageViewModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string ErrorMessage { get; set; }

        /*[FromQuery("returnUrl")]
        public string ReturnUrl { get; set; }*/

        private readonly UserService _userService;
        private readonly MongoBackupTask _mongoBackupTask;
        public LoginViewModel(UserService userService, MongoBackupTask mongoBackupTask)
        {
            _userService = userService;
            _mongoBackupTask = mongoBackupTask;
        }

        public override Task Init()
        {
            return base.Init();
        }

        public async Task SignIn()
        {
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                ErrorMessage = "Invalid Username or password!";
                return;
            }
            var identity = await CreateIdentity(Username, Password);
            if (identity == null)
            {
                ErrorMessage = "Invalid Username or password!";
            }
            else
            {
                var properties = new AuthenticationProperties()
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddMonths(1),
                };
                await Context.GetAuthentication().SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), properties);
                Context.RedirectToRoute("Calendar");
            }
        }

        private async Task<UserDTO> VerifyCredentials(string username, string password)
        {
            var user = await _userService.GetUserAsync(username);
            if (user != null)
            {
                var authentication = BCrypt.Net.BCrypt.Verify(password, user.Password);
                if (authentication)
                {
                    return user;
                }
            }
            return null;
        }

        private async Task<ClaimsIdentity> CreateIdentity(string username, string password)
        {
            var user = await VerifyCredentials(username, password);
            if (user == null) { return null; }

            // build user identity
            var claimsIdentity = new ClaimsIdentity(
            new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
                
            },
            CookieAuthenticationDefaults.AuthenticationScheme);
            return claimsIdentity;
        }



       
    }


}


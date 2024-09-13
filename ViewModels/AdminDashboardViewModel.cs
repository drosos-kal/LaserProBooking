using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotVVM.Framework.ViewModel;
using DotVVM.Framework.Hosting;
using BeautySalonBookingSystem.Models.DTO;
using BeautySalonBookingSystem.Schedulers;
using BeautySalonBookingSystem.Services;

namespace BeautySalonBookingSystem.ViewModels
{
    public class AdminDashboardViewModel : MasterPageViewModel
    {
        public string Username { get; set; }
        public string Password { get; set; }

        private readonly MongoBackupTask _mongoBackupTask;
        private readonly UserService _userService;
        public AdminDashboardViewModel(MongoBackupTask mongoBackupTask, UserService userService)
        {
            _mongoBackupTask = mongoBackupTask;
            _userService = userService;
        }

        public async override Task Init()
        {
            await Context.Authorize(roles: ["admin"]);
            await base.Init();
        }

        public async Task BackupNow()
        {
            await _mongoBackupTask.Invoke();
        }

        public async Task Register()
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(Password);

            var newUser = new UserDTO
            {
                Username = Username,
                Password = hashedPassword,
                Role = "user"
            };
            await _userService.CreateUserAsync(newUser);
        }
    }
}


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BeautySalonBookingSystem.Models.Entities;
using DotVVM.Framework.Hosting;
using DotVVM.Framework.Runtime.Filters;
using DotVVM.Framework.ViewModel;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace BeautySalonBookingSystem.ViewModels
{

    public class MasterPageViewModel : DotvvmViewModelBase
    {
        public string SignedInUser { get; set; } = "";
        public User CurrentUser { get; set; }

        public override async Task Init()
        {
            //await Context.Authorize();
            /*if (Context == null || Context.HttpContext == null || Context.HttpContext.User == null || Context.HttpContext.User.Identity == null)
            {
                Context.RedirectToRoute("Login", allowSpaRedirect: false);
                await base.Init();
            }*/

            SignedInUser = GetSignedInUsername();
            await base.Init();
        }

        public void SignOut()
        {
            Context.GetAuthentication().SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            Context.RedirectToRoute("Login", allowSpaRedirect: false);
        }

        public string GetSignedInUsername()
        {
            var signedInUsername = Context.GetAuthentication()?.Context?.User?.Identity?.Name;
            return signedInUsername;
        }

    }
}

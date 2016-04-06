using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;
using TwitchBotAdmin.Models;
using Owin.Security.Providers.Twitch;
using TwitchBot;
using System.Security.Claims;

namespace TwitchBotAdmin
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Home/Index"),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });            
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            ////Simple Twitch Sign-in
            //app.UseTwitchAuthentication(Config.ClientId, Config.ClientSecret);

            ////More complex Twitch Sign-in
            var opt = new TwitchAuthenticationOptions()
            {
                ClientId = Config.ClientId,
                ClientSecret = Config.ClientSecret,
                Provider = new TwitchAuthenticationProvider()
                {

                    OnAuthenticated = async z =>
                    {
                        //            Getting the twitch users picture
                        z.Identity.AddClaim(new Claim("Picture", z.User.GetValue("logo").ToString()));
                    }
                    //    You should be able to access these claims with  HttpContext.GetOwinContext().Authentication.GetExternalLoginInfoAsync().Claims in your Account Controller
                    //    Commonly used in the ExternalLoginCallback() in AccountController.cs
                    /*

                       if (user != null)
                            {
                                var claim = (await AuthenticationManager.GetExternalLoginInfoAsync()).ExternalIdentity.Claims.First(
                                a => a.Type == "Picture");
                                user.Claims.Add(new IdentityUserClaim() { ClaimType = claim.Type, ClaimValue = claim.Value });
                                await SignInAsync(user, isPersistent: false);
                                return RedirectToLocal(returnUrl);
                            }
                     */
                }
            };
            app.UseTwitchAuthentication(opt);
        }
    }
}
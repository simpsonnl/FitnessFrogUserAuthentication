using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Treehouse.FitnessFrog.Shared.Models;
using Treehouse.FitnessFrog.Shared.Security;
using Treehouse.FitnessFrog.ViewModels;

namespace Treehouse.FitnessFrog.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationUserManager _userManager;
        private readonly ApplicationSignInManager _signInManager;
        private readonly IAuthenticationManager _authenticationManager;

        public AccountController(
            ApplicationUserManager userManager,
            ApplicationSignInManager signInManager,
            IAuthenticationManager authenticationManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _authenticationManager = authenticationManager;
        }
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Register(AccountRegisterViewModel viewModel)
        {
            // If the ModelState is valid...
            if (ModelState.IsValid)
            {
                // Validate if the provided email address is already in use.
                var existingUser = await _userManager.FindByEmailAsync(viewModel.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", $"The provided email address '{viewModel.Email}' has already been used to register an account. Please sign-in using your existing account.");
                }

                // Instantiate a User object
                var user = new User { UserName = viewModel.Email, Email = viewModel.Email };

                // Create the user
                var result = await _userManager.CreateAsync(user, viewModel.Password);

                // If the user was successfully created...
                if (result.Succeeded)
                {
                    // Sign-in the user
                    await _signInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // Redirect them to the web app's "Home" page        
                    return RedirectToAction("Index", "Entries");
                }

                // If there were errors...
                // Add model errors
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }
            return View(viewModel);
        }

        public ActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> SignIn(AccountSignInViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            // Sign-in the user
            var result = await _signInManager.PasswordSignInAsync(
                viewModel.Email, viewModel.Password, viewModel.RememberMe, shouldLockout: false);

            // Check the result
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToAction("Index", "Entries");
                case SignInStatus.Failure:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(viewModel);
                case SignInStatus.LockedOut:
                case SignInStatus.RequiresVerification:
                    throw new NotImplementedException("Identity feature not implemented.");
                default:
                    throw new Exception("Unexpected Microsoft.AspNet.Identity.Owin.SignInStatus enum value: " + result);
            }
        }

        [HttpPost]
        public ActionResult SignOut()
        {
            _authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

            return RedirectToAction("Index", "Entries");
        }
    }
}
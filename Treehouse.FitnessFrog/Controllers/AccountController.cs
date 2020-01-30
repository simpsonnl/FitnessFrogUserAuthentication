using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Treehouse.FitnessFrog.ViewModels;

namespace Treehouse.FitnessFrog.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(AccountRegisterViewModel viewModel)
        {
            return View(viewModel);
        }
    }
}
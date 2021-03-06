using System;
using System.ComponentModel.DataAnnotations;
using System.EnterpriseServices;
using System.Web;
using System.Web.Mvc;
using ADAuthenication.Models;
using Microsoft.Owin.Security;


namespace ADAuthenication.Controllers
{

    public class LoginController : Controller
    {
        [AllowAnonymous]
        public virtual ActionResult Index(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Index(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // usually this will be injected via DI. but creating this manually now for brevity
            IAuthenticationManager authenticationManager = HttpContext.GetOwinContext().Authentication;
            var authService = new AdAuthenticationService(authenticationManager);



        var authenticationResult = authService.SignIn(model.Username, model.Password);
            if (authenticationResult.IsSuccess())
            {
                return RedirectToLocal("/Home/Index");
            }

            ModelState.AddModelError("", authenticationResult.ErrorMessage);
            return View(model);
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }


        [ValidateAntiForgeryToken]
        public virtual ActionResult Logoff()
        {
            IAuthenticationManager authenticationManager = HttpContext.GetOwinContext().Authentication;
            authenticationManager.SignOut(MyAuthentication.ApplicationCookie);

            return RedirectToAction("Index");
        }
    }

    public class LoginViewModel
    {
        [Required, AllowHtml]
        public string Username{ get; set; }
        
        [Required]
        [AllowHtml]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}


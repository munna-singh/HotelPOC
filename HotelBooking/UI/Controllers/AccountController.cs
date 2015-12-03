using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace UI.Controllers
{
    public class AccountController : Controller
    {
        //
        // GET: /Account/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login(FormCollection collection)
        {
            string userName = collection["username"];
            string password = collection["password"];
            if (userName != null && password != null)
            {
                if (userName.ToUpper() == "TRAVELEDGE" && password == "Pass1234")
                {
                    FormsAuthentication.SetAuthCookie(userName, false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.Error = "Login failed. Please verify userName/Password.";
                }
            }
            return View();
        }

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }
    }
}

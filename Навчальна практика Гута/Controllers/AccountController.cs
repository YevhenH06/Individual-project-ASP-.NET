using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using Навчальна_практика_Гута.Models;

public class AccountController : Controller
{
    private ConfectionerShopDBEntities db = new ConfectionerShopDBEntities();

    // GET: Account/Login
    public ActionResult Login()
    {
        return View();
    }

    // POST: Account/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Login(string login, string password)
    {
        var user = db.User.FirstOrDefault(u => u.Login == login && u.Password == password);
        if (user != null)
        {
            Session["UserId"] = user.Id;
            Session["UserRole"] = DetermineUserRole(user.Login);

            return RedirectToAction("Index", "Home");
        }
        ViewBag.ErrorMessage = "Невірний логін або пароль";
        return View();
    }

    // GET: Account/Register
    public ActionResult Register()
    {
        return View();
    }

    // POST: Account/Register
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Register(User model)
    {
        if (ModelState.IsValid)
        {
            db.User.Add(model);
            db.SaveChanges();
            return RedirectToAction("Login");
        }
        return View(model);
    }

    public ActionResult Logout()
    {
        Session["UserId"] = null;
        Session["UserRole"] = null;
        return RedirectToAction("Index", "Home");
    }

    private string DetermineUserRole(string login)
    {
        if (login.Contains("Admin") || login.Contains("admin")) return "Admin";
        return "Buyer";
    }
}
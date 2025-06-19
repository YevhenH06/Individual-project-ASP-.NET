using System;
using System.Web.Mvc;
using Навчальна_практика_Гута.Models;

public class BuyerController : Controller
{
    private ConfectionerShopDBEntities db = new ConfectionerShopDBEntities();

    public ActionResult Create()
    {
        var userId = Session["UserId"] as int?;
        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create([Bind(Include = "FirstName,LastName,PhoneNumber,Email")] Buyer buyer)
    {
        if (ModelState.IsValid)
        {
            var currentUserId = Session["UserId"] as int?;

            if (currentUserId.HasValue)
            {
                buyer.UserId = currentUserId.Value;
                db.Buyer.Add(buyer);
                db.SaveChanges();

                TempData["SuccessMessage"] = "Профіль покупця успішно створено!";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Не вдалося отримати користувача.");
            }
        }

        return View(buyer);
    }
}
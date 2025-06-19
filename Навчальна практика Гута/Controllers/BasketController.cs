using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using Навчальна_практика_Гута.Models;

public class BasketController : Controller
{
    private ConfectionerShopDBEntities db = new ConfectionerShopDBEntities();

    public ActionResult Index()
    {
        if (Session["UserId"] == null) return RedirectToAction("Login", "Account");

        var userId = (int)Session["UserId"];
        var basketItems = db.Basket
            .Where(b => b.BuyerId == userId)
            .ToList();

        return View(basketItems);
    }

    public ActionResult Remove(int id)
    {
        var item = db.Basket.Find(id);
        if (item != null)
        {
            db.Basket.Remove(item);
            db.SaveChanges();
            TempData["SuccessMessage"] = "Товар видалено з кошика";
        }
        return RedirectToAction("Index");
    }
}
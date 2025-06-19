using System;
using System.Linq;
using System.Data.Entity;
using System.Web.Mvc;
using Навчальна_практика_Гута.Models;

public class HomeController : Controller
{
    private ConfectionerShopDBEntities db = new ConfectionerShopDBEntities();

    public ActionResult Index()
    {
        var products = db.Production
            .Include(p => p.Discount)
            .Where(p => p.Quantity > 0)
            .ToList();

        return View(products);
    }

    [HttpPost]
    public ActionResult AddToCart(int productId, int quantity)
    {
        if (Session["UserId"] == null) return RedirectToAction("Login", "Account");

        var userId = (int)Session["UserId"];
        var buyer = db.Buyer.FirstOrDefault(b => b.UserId == userId);
        if (buyer == null) return RedirectToAction("Create", "Buyer");

        var product = db.Production.Find(productId);
        if (product == null || product.Quantity < quantity)
        {
            TempData["ErrorMessage"] = "Недостатня кількість товару";
            return RedirectToAction("Index");
        }

        // Отримуємо першу активну знижку для продукту
        var discount = db.Discount.FirstOrDefault(d => d.ProductionId == productId);

        // Створюємо запис у кошику
        var basketItem = new Basket
        {
            ProductName = product.ProductName,
            DiscountedPrice = discount != null ? discount.DiscountedPrice : product.Price,
            BuyerFirstName = buyer.FirstName,
            BuyerLastName = buyer.LastName,
            BuyerId = buyer.Id,
            DiscountId = discount?.Id
        };

        db.Basket.Add(basketItem);
        product.Quantity -= quantity;
        db.SaveChanges();

        TempData["SuccessMessage"] = product.ProductName + " додано до кошика";
        return RedirectToAction("Index", "Basket");
    }
}
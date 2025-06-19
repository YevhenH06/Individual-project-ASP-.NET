using System;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using Навчальна_практика_Гута.Models;

public class AdminController : Controller
{
    private ConfectionerShopDBEntities db = new ConfectionerShopDBEntities();

    [AuthorizeAdmin]
    public ActionResult Products()
    {
        var products = db.Production
            .Include(p => p.Discount)
            .ToList();

        return View(products);
    }

    [AuthorizeAdmin]
    public ActionResult CreateProduct()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [AuthorizeAdmin]
    public ActionResult CreateProduct(Production product)
    {
        if (ModelState.IsValid)
        {
            db.Production.Add(product);
            db.SaveChanges();
            TempData["SuccessMessage"] = "Продукцію успішно додано";
            return RedirectToAction("Products");
        }
        return View(product);
    }

    [AuthorizeAdmin]
    public ActionResult EditProduct(int id)
    {
        var product = db.Production.Find(id);
        if (product == null)
        {
            return HttpNotFound();
        }
        return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [AuthorizeAdmin]
    public ActionResult EditProduct(Production product)
    {
        if (ModelState.IsValid)
        {
            db.Entry(product).State = EntityState.Modified;
            db.SaveChanges();
            TempData["SuccessMessage"] = "Продукцію успішно оновлено";
            return RedirectToAction("Products");
        }
        return View(product);
    }

    [AuthorizeAdmin]
    public ActionResult DeleteProduct(int id)
    {
        // Починаємо транзакцію
        using (var transaction = db.Database.BeginTransaction())
        {
            try
            {
                // 1. Знаходимо продукт
                var product = db.Production.Find(id);
                if (product == null)
                {
                    return HttpNotFound();
                }

                // 2. Знаходимо знижку, пов'язану з продуктом
                var discount = db.Discount.FirstOrDefault(d => d.ProductionId == product.Id);

                if (discount != null)
                {
                    // 3. Знаходимо всі записи кошика, пов'язані зі знижкою
                    var basketItems = db.Basket.Where(b => b.DiscountId == discount.Id).ToList();

                    // 4. Видаляємо записи кошика
                    db.Basket.RemoveRange(basketItems);

                    // 5. Видаляємо знижку
                    db.Discount.Remove(discount);
                }

                // 6. Видаляємо сам продукт
                db.Production.Remove(product);

                // 7. Зберігаємо зміни
                db.SaveChanges();

                // 8. Підтверджуємо транзакцію
                transaction.Commit();

                TempData["SuccessMessage"] = "Продукцію та всі пов'язані дані успішно видалено";
            }
            catch (Exception ex)
            {
                // Відкатуємо транзакцію у разі помилки
                transaction.Rollback();
                TempData["ErrorMessage"] = $"Помилка при видаленні: {ex.Message}";

                if (ex.InnerException != null)
                {
                    TempData["ErrorMessage"] += $" | {ex.InnerException.Message}";
                }
            }
        }

        return RedirectToAction("Products");
    }

    [HttpPost]
    [AuthorizeAdmin]
    public JsonResult AddDiscount(int productId, string discountName, decimal discountValue)
    {
        try
        {
            // Логування для відладки
            System.Diagnostics.Debug.WriteLine($"AddDiscount called with: productId={productId}, discountName={discountName}, discountValue={discountValue}");

            var product = db.Production.Find(productId);
            if (product == null)
            {
                return Json(new { success = false, message = "Продукт не знайдено" });
            }

            // Перевірка чи знижка вже існує
            if (db.Discount.Any(d => d.ProductionId == productId))
            {
                return Json(new { success = false, message = "Для цього продукту вже є знижка" });
            }

            // Валідація розміру знижки
            if (discountValue <= 0 || discountValue > 100)
            {
                return Json(new { success = false, message = "Розмір знижки повинен бути від 0.01 до 100%" });
            }

            // Розрахунок ціни зі знижкою
            var discountValueDecimal = discountValue / 100;
            var discountedPrice = product.Price * (1 - discountValueDecimal);

            var discount = new Discount
            {
                DiscountName = discountName,
                DiscountValue = discountValueDecimal,
                DiscountedPrice = discountedPrice,
                ProductionId = productId
            };

            db.Discount.Add(discount);
            db.SaveChanges();

            System.Diagnostics.Debug.WriteLine("Discount added successfully");

            return Json(new { success = true, message = "Знижку успішно додано" });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in AddDiscount: {ex.Message}");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [AuthorizeAdmin]
    public JsonResult EditDiscount(int discountId, string discountName, decimal discountValue)
    {
        try
        {
            var discount = db.Discount.Find(discountId);
            if (discount == null)
            {
                return Json(new { success = false, message = "Знижка не знайдена" });
            }

            var product = db.Production.Find(discount.ProductionId);
            if (product == null)
            {
                return Json(new { success = false, message = "Продукт не знайдено" });
            }

            // Валідація розміру знижки
            if (discountValue <= 0 || discountValue > 100)
            {
                return Json(new { success = false, message = "Розмір знижки повинен бути від 0.01 до 100%" });
            }

            // Оновлення знижки
            discount.DiscountName = discountName;
            discount.DiscountValue = discountValue / 100;
            discount.DiscountedPrice = product.Price * (1 - discountValue / 100);

            db.Entry(discount).State = EntityState.Modified;
            db.SaveChanges();

            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [AuthorizeAdmin]
    public ActionResult RemoveDiscount(int id)
    {
        try
        {
            var discount = db.Discount.Find(id);
            if (discount != null)
            {
                db.Discount.Remove(discount);
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Знижка не знайдена" });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [AuthorizeAdmin]
    public ActionResult Users(string search = null) 
    {
        var users = db.User.AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            users = users.Where(u => u.Login.Contains(search));
        }

        return View(users.ToList());
    }

    [AuthorizeAdmin]
    public ActionResult Buyers(string search = null)
    {
        var buyers = db.Buyer.Include(b => b.User).AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            buyers = buyers.Where(b =>
                b.FirstName.Contains(search) ||
                b.LastName.Contains(search));
        }

        ViewBag.Search = search;
        return View(buyers.ToList());
    }

    [AuthorizeAdmin]
    public ActionResult BuyerDetails(int id)
    {
        var buyer = db.Buyer
            .Include(b => b.User)
            .Include(b => b.Basket)
            .FirstOrDefault(b => b.Id == id);

        if (buyer == null)
        {
            return HttpNotFound();
        }

        return View(buyer);
    }

    [AuthorizeAdmin]
    public ActionResult EditUser(int id)
    {
        var user = db.User.Find(id);
        if (user == null)
        {
            return HttpNotFound();
        }
        return View(user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [AuthorizeAdmin]
    public ActionResult EditUser(User user)
    {
        if (ModelState.IsValid)
        {
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();
            TempData["SuccessMessage"] = "Дані користувача успішно оновлено";
            return RedirectToAction("Users");
        }
        return View(user);
    }

    [AuthorizeAdmin]
    public ActionResult MakeAdmin(int id)
    {
        var user = db.User.Find(id);
        if (user == null)
        {
            return HttpNotFound();
        }

        if (!user.Login.Contains("Admin"))
        {
            user.Login = "Admin_" + user.Login;
            db.SaveChanges();
        }

        return RedirectToAction("Users");
    }

    [AuthorizeAdmin]
    public ActionResult EditBuyer(int id)
    {
        var buyer = db.Buyer.Find(id);
        if (buyer == null)
        {
            return HttpNotFound();
        }
        return View(buyer);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [AuthorizeAdmin]
    public ActionResult EditBuyer(Buyer buyer)
    {
        if (ModelState.IsValid)
        {
            db.Entry(buyer).State = EntityState.Modified;
            db.SaveChanges();
            TempData["SuccessMessage"] = "Дані покупця успішно оновлено";
            return RedirectToAction("Buyers");
        }
        return View(buyer);
    }

    [AuthorizeAdmin]
    public ActionResult DeleteUser(int id)
    {
        var user = db.User.Find(id);
        if (user == null)
        {
            return HttpNotFound();
        }

        // Перевірка наявності пов'язаних записів
        var hasRelatedBuyers = db.Buyer.Any(b => b.UserId == id);
        if (hasRelatedBuyers)
        {
            TempData["ErrorMessage"] = "Неможливо видалити користувача, оскільки є пов'язані записи покупців";
            return RedirectToAction("Users");
        }

        try
        {
            db.User.Remove(user);
            db.SaveChanges();
            TempData["SuccessMessage"] = "Користувача успішно видалено";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Помилка при видаленні користувача: " + ex.Message;
        }

        return RedirectToAction("Users");
    }

    [AuthorizeAdmin]
    public ActionResult DeleteBuyer(int id)
    {
        var buyer = db.Buyer.Find(id);
        if (buyer == null)
        {
            return HttpNotFound();
        }

        try
        {
            // Видалення пов'язаних записів кошика
            var basketItems = db.Basket.Where(b => b.BuyerId == id).ToList();
            db.Basket.RemoveRange(basketItems);

            db.Buyer.Remove(buyer);
            db.SaveChanges();
            TempData["SuccessMessage"] = "Покупця успішно видалено";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Помилка при видаленні покупця: " + ex.Message;
        }

        return RedirectToAction("Buyers");
    }
}

public class AuthorizeAdminAttribute : AuthorizeAttribute
{
    protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
    {
        var userRole = httpContext.Session["UserRole"] as string;
        return userRole == "Admin";
    }
}
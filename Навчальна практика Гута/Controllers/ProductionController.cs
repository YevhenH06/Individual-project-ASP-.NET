using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Навчальна_практика_Гута.Models;

namespace Навчальна_практика_Гута.Controllers
{
    public class ProductionController : Controller
    {
        private ConfectionerShopDBEntities db = new ConfectionerShopDBEntities();

        // GET: Production
        public ActionResult Index(string productName, DateTime? displayTime,
                                DateTime? expiryTime, decimal? price, int? quantity)
        {
            var productions = db.Production.AsQueryable();

            if (!string.IsNullOrEmpty(productName))
            {
                productions = productions.Where(p => p.ProductName.Contains(productName));
            }

            if (displayTime.HasValue)
            {
                productions = productions.Where(p => DbFunctions.TruncateTime(p.DisplayTime)
                                                    == displayTime.Value.Date);
            }

            if (expiryTime.HasValue)
            {
                productions = productions.Where(p => DbFunctions.TruncateTime(p.ExpiryTime)
                                                    == expiryTime.Value.Date);
            }

            if (price.HasValue)
            {
                productions = productions.Where(p => p.Price == price.Value);
            }

            if (quantity.HasValue)
            {
                productions = productions.Where(p => p.Quantity == quantity.Value);
            }

            return View(productions.ToList());
        }

        // GET: Production/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductName,DisplayTime,ExpiryTime,Price,Quantity")] Production production)
        {
            if (ModelState.IsValid)
            {
                db.Production.Add(production);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(production);
        }

        // GET: Production/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Production production = db.Production.Find(id);

            if (production == null)
            {
                return HttpNotFound();
            }

            return View(production);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ProductName,DisplayTime,ExpiryTime,Price,Quantity")] Production production)
        {
            if (ModelState.IsValid)
            {
                db.Entry(production).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(production);
        }

        // GET: Production/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Production production = db.Production.Find(id);

            if (production == null)
            {
                return HttpNotFound();
            }

            return View(production);
        }

        // POST: Production/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Production production = db.Production
                .Include(p => p.Discount) // Завантажуємо пов'язані знижки
                .FirstOrDefault(p => p.Id == id);

            if (production != null)
            {
                // Видаляємо всі пов'язані знижки
                foreach (var discount in production.Discount.ToList())
                {
                    db.Discount.Remove(discount);
                }

                db.Production.Remove(production);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
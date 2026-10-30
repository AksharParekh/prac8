using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using prac8.Models;

namespace prac8.Controllers
{
    public class InventoryController : Controller
    {
        private readonly InventoryDAL _dal = new InventoryDAL();

        // GET: Inventory / Inventory/Index
        public ActionResult Index(string searchTerm, string categoryFilter)
        {
            var items = _dal.GetAllItems(searchTerm, categoryFilter);

            var categories = _dal.GetCategories();
            categories.Insert(0, "All");
            ViewBag.Categories = new SelectList(categories, categoryFilter ?? "All");
            ViewBag.CurrentSearch = searchTerm;
            ViewBag.CurrentCategory = categoryFilter ?? "All";

            // Dashboard Metrics
            ViewBag.TotalItems = items.Count;
            ViewBag.TotalQuantity = items.Sum(i => i.Quantity);
            ViewBag.TotalValuation = items.Sum(i => i.Quantity * i.UnitPrice);
            ViewBag.LowStockCount = items.Count(i => i.Quantity < 10);

            return View(items);
        }

        // GET: Inventory/Details/5
        public ActionResult Details(int id)
        {
            var item = _dal.GetItemById(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        // GET: Inventory/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Inventory/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(InventoryItem item)
        {
            if (ModelState.IsValid)
            {
                bool success = _dal.InsertItem(item);
                if (success)
                {
                    TempData["SuccessMessage"] = $"Item '{item.ItemName}' added to inventory successfully!";
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("", "Database error: unable to insert inventory record.");
            }
            return View(item);
        }

        // GET: Inventory/Edit/5
        public ActionResult Edit(int id)
        {
            var item = _dal.GetItemById(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        // POST: Inventory/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(InventoryItem item)
        {
            if (ModelState.IsValid)
            {
                bool success = _dal.UpdateItem(item);
                if (success)
                {
                    TempData["SuccessMessage"] = $"Item #{item.ItemId} ('{item.ItemName}') updated successfully!";
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("", "Database error: unable to update inventory record.");
            }
            return View(item);
        }

        // GET: Inventory/Delete/5
        public ActionResult Delete(int id)
        {
            var item = _dal.GetItemById(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        // POST: Inventory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var item = _dal.GetItemById(id);
            string name = item != null ? item.ItemName : id.ToString();
            bool success = _dal.DeleteItem(id);
            if (success)
            {
                TempData["SuccessMessage"] = $"Item '{name}' deleted from inventory successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete the selected item.";
            }
            return RedirectToAction("Index");
        }
    }
}

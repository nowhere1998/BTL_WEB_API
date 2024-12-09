using BTL_CALLWEB_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Security.Policy;
using System.Xml.Linq;

namespace BTL_CALLWEB_API.Areas.Admin.Controllers
{
    [Area("admin")]
    [Authorize]
    public class OrdersController : Controller
    {
        string url = "https://localhost:44316/api/";
        HttpClient client = new HttpClient();

        public OrdersController()
        {
            client.BaseAddress = new Uri(url);
        }
        public async Task<IActionResult> Index(string name = "", int currentPage = 1)
        {
            client.BaseAddress = new Uri(url);
            int pageSize = 6;
            var allOrders = JsonConvert.DeserializeObject<List<Order>>(await client.GetStringAsync("orders/getAll"));
            var orders = JsonConvert.DeserializeObject<List<Order>>(await client.GetStringAsync("orders?currentPage=" + currentPage));
            if (!name.Equals(""))
            {
                orders = JsonConvert.DeserializeObject<List<Order>>(await client.GetStringAsync("orders?name=" + name));
                allOrders = JsonConvert.DeserializeObject<List<Order>>(await client.GetStringAsync("orders?name=" + name));
            }
            else
            {
                orders = JsonConvert.DeserializeObject<List<Order>>(await client.GetStringAsync("orders?currentPage=" + currentPage));
            }
            ViewBag.TotalPage = allOrders.Count() % pageSize == 0 ? allOrders.Count() / pageSize : allOrders.Count() / pageSize + 1;
            ViewBag.CurrentPage = currentPage;
            ViewBag.Name = name;
            ViewBag.Accounts = JsonConvert.DeserializeObject<List<Account>>(await client.GetStringAsync("accounts/getAll"));
            return View(orders);
        }

        // GET: Admin/Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderDetail = JsonConvert.DeserializeObject<List<OrderDetail>>(await client.GetStringAsync("orders/detail/" + id));
            ViewBag.Dishes = JsonConvert.DeserializeObject<List<Dish>>(await client.GetStringAsync("dishes/getAll"));
            if (orderDetail == null)
            {
                return NotFound();
            }

            return View(orderDetail);
        }

        // GET: Admin/Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = JsonConvert.DeserializeObject<Order>(await client.GetStringAsync("orders/" + id));
            ViewBag.Accounts = JsonConvert.DeserializeObject<List<Account>>(await client.GetStringAsync("accounts/getAll"));
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST: Admin/Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,AccountId,Name,Phone,Address,TotalPrice,status,note,CreatedAt")] Order order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }

            var response = await client.PutAsJsonAsync("orders/" + id, order);
            TempData["msg"] = response.Content.ReadAsStringAsync().Result;
            return RedirectToAction("Index");
        }

        //GET: Admin/Orders/Delete/5
        public async Task<IActionResult> Cancel(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = JsonConvert.DeserializeObject<Order>(await client.GetStringAsync("orders/" + id));
            if (order == null)
            {
                return NotFound();
            }
            order.status = 3;
            var response = await client.PutAsJsonAsync("orders/" + id, order);
            TempData["msg"] = response.Content.ReadAsStringAsync().Result;
            return RedirectToAction(nameof(Index));
        }

        //// POST: Admin/Orders/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var order = await _context.Orders.FindAsync(id);
        //    if (order != null)
        //    {
        //        _context.Orders.Remove(order);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}
    }
}

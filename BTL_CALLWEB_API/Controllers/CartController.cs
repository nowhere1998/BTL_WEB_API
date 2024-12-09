using BTL_CALLWEB_API.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BTL_CALLWEB_API.Controllers
{
    public class CartController : Controller
    {
        private List<Cart> carts = new List<Cart>();
        string url = "https://localhost:44316/api/";
        HttpClient client = new HttpClient();
        public CartController()
        {
            client.BaseAddress = new Uri(url);
        }

        public IActionResult Cart()
        {
            var cart = HttpContext.Session.GetString("cart");
            if (cart != null)
            {
                carts = JsonConvert.DeserializeObject<List<Cart>>(cart);
            }
            ViewBag.Url = "https://localhost:44316";
            return View(carts);
        }

        public async Task<IActionResult> Add(int id, int quantity = 1)
        {
            var cart = HttpContext.Session.GetString("cart");
            if (cart != null)
            {
                carts = JsonConvert.DeserializeObject<List<Cart>>(cart);
            }
            if (carts.Any(x => x.Id == id))
            {
                carts.Where(x => x.Id == id).First().Quantity += quantity;
            }
            else
            {
                var test = id;
                var dish = JsonConvert.DeserializeObject<Dish>(await client.GetStringAsync("dishes/" + id));
                if (dish != null)
                {
                    var item = new Cart { Id = dish.DishId, Name = dish.DishName, Image = dish.Image, Price = dish.SalePrice <= 0 ? dish.Price : dish.SalePrice, Quantity = quantity };
                    carts.Add(item);
                }
            }
            HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(carts));

            return RedirectToAction("Cart", "Cart");
        }

        public IActionResult Update(int id, int quantity)
        {
            var cart = HttpContext.Session.GetString("cart");
            if (cart != null)
            {
                carts = JsonConvert.DeserializeObject<List<Cart>>(cart);
            }
            if (carts.Any(x => x.Id == id))
            {
                carts.Where(x => x.Id == id).First().Quantity = quantity;
                HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(carts));
            }
            return RedirectToAction("Cart", "Cart");
        }

        public IActionResult Remove(int id)
        {
            var cart = HttpContext.Session.GetString("cart");
            if (cart != null)
            {
                carts = JsonConvert.DeserializeObject<List<Cart>>(cart);
            }
            if (carts.Any(x => x.Id == id))
            {
                var item = carts.Where(x => x.Id == id).First();
                carts.Remove(item);
                HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(carts));
            }
            return RedirectToAction("Cart", "Cart");
        }

        public IActionResult Clear()
        {
            HttpContext.Session.Remove("cart");
            return RedirectToAction("Menu", "Home");
        }

        public IActionResult Checkout()
        {
            var cart = HttpContext.Session.GetString("cart");
            if (cart != null)
            {
                carts = JsonConvert.DeserializeObject<List<Cart>>(cart);
            }
            return View(carts);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(string name = "", string phone = "", string address = "", float totalPrice = 0, int accountId = 0)
        {
            var cart = HttpContext.Session.GetString("cart");
            if (cart != null)
            {
                carts = JsonConvert.DeserializeObject<List<Cart>>(cart);
            }
            else
            {
                ViewBag.ErrorCheckout = "Please choose dishes!";
                return View(carts);
            }
            Order order = new Order();
            if (name.Equals(""))
            {
                order.Name = HttpContext.Session.GetString("username");
            }
            else
            {
                order.Name = name;
            }
            order.Phone = phone;
            order.Address = address;
            order.TotalPrice = totalPrice;
            order.AccountId = accountId;
            var response = await client.PostAsJsonAsync("orders", order);
            var orders = JsonConvert.DeserializeObject<List<Order>>(await client.GetStringAsync("orders/getAll"));
            var orderTakeId = orders.OrderByDescending(o => o.OrderId).FirstOrDefault();
            foreach(var item in carts)
            {
                OrderDetail orderDetail = new OrderDetail();
                orderDetail.OrderId = orderTakeId.OrderId;
                orderDetail.DishId = item.Id;
                orderDetail.Quantity = item.Quantity;
                orderDetail.Price = item.Price;
                await client.PostAsJsonAsync("orders/detail", orderDetail);
            }
            TempData["CheckoutSuccess"] = "Checkout success!";
            return RedirectToAction("Index", "Home", new { message = "Checkout success!" });
        }
    }
}

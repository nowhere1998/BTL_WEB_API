using BTL_CALLWEB_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Xml.Linq;

namespace BTL_CALLWEB_API.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        string url = "https://localhost:44316/api/";
        HttpClient client = new HttpClient();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            client.BaseAddress = new Uri(url);
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Menu(string name = "", int currentPage = 1, int categoryId = 0)
        {
            int pageSize = 6;
            var allDishes = JsonConvert.DeserializeObject<List<Dish>>(await client.GetStringAsync("dishes/getAll"));
            var dishes = JsonConvert.DeserializeObject<List<Dish>>(await client.GetStringAsync("dishes?currentPage=" + currentPage));
            if(name.Trim().Equals("") && categoryId!=0)
            {
                dishes = JsonConvert.DeserializeObject<List<Dish>>(await client.GetStringAsync("dishes?categoryId=" + categoryId));
                allDishes = JsonConvert.DeserializeObject<List<Dish>>(await client.GetStringAsync("dishes?categoryId=" + categoryId));
            }
            if (!name.Trim().Equals("") && categoryId==0)
            {
                dishes = JsonConvert.DeserializeObject<List<Dish>>(await client.GetStringAsync("dishes?name=" + name));
                allDishes = JsonConvert.DeserializeObject<List<Dish>>(await client.GetStringAsync("dishes?name=" + name));
            }
            if(name.Trim().Equals("") && categoryId == 0)
            {
                dishes = JsonConvert.DeserializeObject<List<Dish>>(await client.GetStringAsync("dishes?currentPage=" + currentPage));
            }
            if(!name.Trim().Equals("") && categoryId != 0)
            {
                dishes = JsonConvert.DeserializeObject<List<Dish>>(await client.GetStringAsync("dishes?categoryId=" + categoryId + "&name="+name));
                allDishes = JsonConvert.DeserializeObject<List<Dish>>(await client.GetStringAsync("dishes?categoryId=" + categoryId + "&name=" + name));
            }
            ViewBag.TotalPage = allDishes.Count() % pageSize == 0 ? allDishes.Count() / pageSize : allDishes.Count() / pageSize + 1;
            ViewBag.Categories = JsonConvert.DeserializeObject<List<Category>>(await client.GetStringAsync("categories/getAll"));
            ViewBag.Url = "https://localhost:44316";
            ViewBag.CurrentPage = currentPage;
            ViewBag.Name = name;
            ViewBag.CategoryId = categoryId;
            return View(dishes);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var dish = JsonConvert.DeserializeObject<Dish>(await client.GetStringAsync("dishes/"+id));
            ViewBag.Categories = JsonConvert.DeserializeObject<List<Category>>(await client.GetStringAsync("categories"));
            ViewBag.Url = "https://localhost:44316";
            return View(dish);
        }
        public IActionResult Login(string page = "")
        {
            ViewBag.Page = page;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email = "", string password = "", string page = "")
        {
            string passmd5 = "";
            passmd5 = Cipher.GenerateMD5(password);
            var accounts = JsonConvert.DeserializeObject<List<Account>>(await client.GetStringAsync("accounts/getAll"));
            var account = new Account();
            if(accounts != null && email!=null && passmd5!=null)
            {
                foreach(var item in accounts)
                {
                    if(item.Email.Equals(email) && item.Password.Equals(passmd5) && item.Role!=2)
                    {
                        HttpContext.Session.SetString("username", item.Name);
                        HttpContext.Session.SetString("accountId", item.AccountId.ToString());

                        if (page.ToLower().Contains("checkout"))
                        {
                            return RedirectToAction("Checkout", "Cart");
                        }
                        return RedirectToAction("Index");
                    }
                }
            }
            ViewBag.error = "<p class='alert alert-danger'>Email or password is incorrect!</p>";
            ViewBag.Email = email;
            ViewBag.password = password;
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register([Bind("Name, Email, Password")] Account account, string confirmPassword = "")
        {
            bool check = true;
            var accounts = JsonConvert.DeserializeObject<List<Account>>(await client.GetStringAsync("accounts/getAll"));
            if(accounts!=null)
            {
                foreach(var item in accounts)
                {
                    if(item.Email == account.Email)
                    {
                        ViewBag.Email = "Email is already exist!";
                        check = false;
                    }
                }
            }
            if (account.Email == null || account.Email.Trim().Equals(""))
            {
                ViewBag.ErrorEmail = "Email can not empty!";
                check = false;
            }
            if (account.Password == null || account.Password.Trim().Equals(""))
            {
                ViewBag.ErrorEmail = "Email can not empty!";
                check = false;
            }
            if (confirmPassword.Equals(account.Password))
            {
                account.Password = Cipher.GenerateMD5(account.Password);
            }
            else
            {
                ViewBag.ErrorConfirmPassword = "Confirm password does not match!";
                check = false;
            }
            if(!check)
            {
                return View(account);
            }
            account.Phone = "";
            var formData = new MultipartFormDataContent();
            formData.Add(new StringContent(account.Name), "name");
            formData.Add(new StringContent(account.Password), "password");
            formData.Add(new StringContent(account.Email), "email");
            formData.Add(new StringContent(account.Phone), "phone");
            formData.Add(new StringContent(account.Role.ToString()), "role");
            var response = await client.PostAsync("accounts", formData);
            TempData["RegisterSuccess"] = "Register success";
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> History(int id = 0)
        {
            var accountId = id;
            if (HttpContext.Session.GetString("accountId")!=null)
            {
                accountId = int.Parse(HttpContext.Session.GetString("accountId"));
            }
            var order = JsonConvert.DeserializeObject<List<Order>>(await client.GetStringAsync("orders/getAllByAccountId/" + accountId));
            ViewBag.OrderDetails = JsonConvert.DeserializeObject<List<OrderDetail>>(await client.GetStringAsync("orders/detail/getAll"));
            ViewBag.Dishes = JsonConvert.DeserializeObject<List<Dish>>(await client.GetStringAsync("dishes/getAll"));
            return View(order);
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Branch()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

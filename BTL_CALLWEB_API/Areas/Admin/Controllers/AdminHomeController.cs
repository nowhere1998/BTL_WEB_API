using BTL_CALLWEB_API.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace BTL_CALLWEB_API.Areas.Admin.Controllers
{
	[Area("admin")]
	[Route("admin")]
	public class AdminHomeController : Controller
	{
        string url = "https://localhost:44316/api/";
        HttpClient client = new HttpClient();

        public AdminHomeController()
        {
            client.BaseAddress = new Uri(url);
        }
        [Route("")]
        [Authorize]
		public IActionResult Index()
		{
            ViewBag.Url = "https://localhost:44316";
			return View();
		}

        [Route("search")]
        [Authorize]
        public IActionResult Search(string search, string path)
        {
            if (path.ToLower().Contains("order/details"))
            {
                return Redirect("/admin/orders" + "?name=" + search);
            }
            return Redirect(path + "?name=" + search);
        }

        [Route("login")]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(string email = "", string password = "")
        {
            string passMD5 = "";
            passMD5 = Cipher.GenerateMD5(password);
            var accounts = JsonConvert.DeserializeObject<List<Account>>(await client.GetStringAsync("accounts/getAll"));
            var acc = new Account();
            if (accounts != null)
            {
                foreach (var account in accounts)
                {
                    if(account.Email == email && account.Password == passMD5 && account.Role == 1)
                    {
                        acc = account;
                    }
                }
            }
            if (acc != null)
            {
                var identity = new ClaimsIdentity(
                    new[]
                    {
                        new Claim(ClaimTypes.Name, acc.Name??""),
                        new Claim("accountId", acc.AccountId.ToString()),
                        new Claim("image", acc.Image??""),
                        new Claim("email", acc.Email??"")
                    }, "RESTINASecurityScheme");
                var principal = new ClaimsPrincipal(identity);
                var login = HttpContext.SignInAsync("RESTINASecurityScheme", principal);
                return RedirectToAction("Index");
            }
            ViewBag.error = "<p class='alert alert-danger'>Email or password is incorrect!</p>";
            ViewBag.Email = email;
            ViewBag.Password = password;
            return View();
        }

        [Route("logout")]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync("RESTINASecurityScheme");
            return Redirect("~/admin");
        }
    }
}

using BTL_CALLWEB_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Security.Principal;
using System.Xml.Linq;

namespace BTL_CALLWEB_API.Areas.Admin.Controllers
{
    [Area("admin")]
    [Authorize]
    public class AccountsController : Controller
    {
        string url = "https://localhost:44316/api/";
        HttpClient client = new HttpClient();
        public AccountsController()
        {
            client.BaseAddress = new Uri(url);
        }
        // GET: AccountsController
        public async Task<ActionResult> Index(string name = "", int currentPage = 1)
        {
            int pageSize = 6;
            var allAccounts = JsonConvert.DeserializeObject<List<Account>>(await client.GetStringAsync("accounts/getAll"));
            var accounts = JsonConvert.DeserializeObject<List<Account>>(await client.GetStringAsync("accounts?currentPage=" + currentPage));
            if (!name.Equals(""))
            {
                accounts = JsonConvert.DeserializeObject<List<Account>>(await client.GetStringAsync("accounts?name=" + name));
                allAccounts = JsonConvert.DeserializeObject<List<Account>>(await client.GetStringAsync("accounts?name=" + name));
            }
            else
            {
                accounts = JsonConvert.DeserializeObject<List<Account>>(await client.GetStringAsync("accounts?currentPage=" + currentPage));
            }
            ViewBag.TotalPage = allAccounts.Count() % pageSize == 0 ? allAccounts.Count() / pageSize : allAccounts.Count() / pageSize + 1;
            ViewBag.CurrentPage = currentPage;
            ViewBag.Name = name;
            ViewBag.Url = "https://localhost:44316";
            return View(accounts);
        }

        // GET: AccountsController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            ViewBag.Url = "https://localhost:44316";
            var account = JsonConvert.DeserializeObject<Account>(await client.GetStringAsync("accounts/" + id));
            return View(account);
        }

        // GET: AccountsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AccountsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Account account, IFormFile fileImage, string confirmPassword = "")
        {
            bool check = true;
            var accounts = JsonConvert.DeserializeObject<List<Account>>(await client.GetStringAsync("accounts/getAll"));
            if (account.Name == null || account.Name.Trim().Equals(""))
            {
                ViewBag.ErrorName = "Account name can not empty!";
                check = false;
            }
            if (account.Password == null || account.Password.Trim().Equals(""))
            {
                ViewBag.ErrorPassword = "Password can not empty!";
                check = false;
            }
            if (confirmPassword!=null && account.Password!=null)
            {
                if (!account.Password.Equals(confirmPassword))
                {
                    ViewBag.ErrorConfirmPassword = "Confirm password is not correct!";
                    check = false;
                }
                else
                {
                    account.Password = Cipher.GenerateMD5(account.Password);
                }
            }
            if (account.Email == null || account.Email.Trim().Equals(""))
            {
                ViewBag.ErrorEmail = "Email can not empty!";
                check = false;
            }
            if (account.Phone == null)
            {
                account.Phone = "";
            }
            if (accounts!=null)
            {
                foreach(var item in accounts)
                {
                    if (item.Email.Equals(account.Email))
                    {
                        ViewBag.ErrorEmail = "Email is already exist!";
                        check = false;
                        break;
                    }
                }
            }
            if (!check)
            {
                return View(account);
            }
            var formData = new MultipartFormDataContent();
            formData.Add(new StringContent(account.Name), "name");
            formData.Add(new StringContent(account.Password), "password");
            formData.Add(new StringContent(account.Email), "email");
            formData.Add(new StringContent(account.Phone), "phone");
            formData.Add(new StringContent(account.Role.ToString()), "role");
            if (fileImage != null && fileImage.Length > 0)
            {
                formData.Add(new StreamContent(fileImage.OpenReadStream()), "image", fileImage.FileName);
            }
            else
            {
                account.Image = "";
            }
            var response = await client.PostAsync("accounts", formData);
            TempData["msg"] = response.Content.ReadAsStringAsync().Result;
            return RedirectToAction("Index");
        }

        // GET: AccountsController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var account = JsonConvert.DeserializeObject<Account>(await client.GetStringAsync("accounts/" + id));
            ViewBag.Url = "https://localhost:44316";
            return View(account);
        }

        // POST: AccountsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Account account, IFormFile fileImage, string confirmPassword = "", string oldImage = "")
        {
            var accounts = JsonConvert.DeserializeObject<List<Account>>(await client.GetStringAsync("accounts"));
            //if (!account.Password.Equals(confirmPassword))
            //{
            //    return View(account);
            //}
            //else
            //{
            //    account.Password = Cipher.GenerateMD5(account.Password);
            //}
            if(account.Phone == null)
            {
                account.Phone = "";
            }
            var formData = new MultipartFormDataContent();
            formData.Add(new StringContent(account.AccountId.ToString()), "accountId");
            formData.Add(new StringContent(account.Name), "name");
            formData.Add(new StringContent(account.Password), "password");
            formData.Add(new StringContent(account.Email), "email");
            formData.Add(new StringContent(account.Phone), "phone");
            formData.Add(new StringContent(account.Role.ToString()), "role");
            if (fileImage != null && fileImage.Length > 0)
            {
                formData.Add(new StreamContent(fileImage.OpenReadStream()), "image", fileImage.FileName);
            }
            else
            {
                formData.Add(new StringContent(oldImage), "oldImage");
            }
            var response = await client.PutAsync("accounts/" + id, formData);
            TempData["msg"] = response.Content.ReadAsStringAsync().Result;
            if (TempData["msg"].ToString().ToLower().Contains("warning"))
            {
                ViewBag.Error = "Email is already exist!";
                return View(account);
            }
            return RedirectToAction("Index");
        }

        // GET: AccountsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AccountsController/Delete/5
        public async Task<ActionResult> Deactive(int id)
        {
            var account = JsonConvert.DeserializeObject<Account>(await client.GetStringAsync("accounts/" + id));
            if(account.Phone == null)
            {
                account.Phone = "";
            }
			if (account.Image == null)
			{
				account.Image = "";
			}
			account.Role = 2;
            var formData = new MultipartFormDataContent();
            formData.Add(new StringContent(account.AccountId.ToString()), "accountId");
            formData.Add(new StringContent(account.Name), "name");
            formData.Add(new StringContent(account.Password), "password");
            formData.Add(new StringContent(account.Email), "email");
            formData.Add(new StringContent(account.Phone), "phone");
            formData.Add(new StringContent(account.Image), "oldImage");
            formData.Add(new StringContent(account.Role.ToString()), "role");
            var response = await client.PutAsync("accounts/"+id, formData);
            TempData["msg"] = response.Content.ReadAsStringAsync().Result;
            return RedirectToAction("Index");
        }
    }
}

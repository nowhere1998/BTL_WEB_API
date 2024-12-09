using BTL_CALLWEB_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Xml.Linq;

namespace BTL_CALLWEB_API.Areas.Admin.Controllers
{
    [Area("admin")]
    [Authorize]
    public class CategoriesController : Controller
    {
        string uri = "https://localhost:44316/api/";
        HttpClient client = new HttpClient();
        // GET: CategoriesController
        public async Task<ActionResult> Index(string name = "", int currentPage = 1)
        {
            client.BaseAddress = new Uri(uri);
            int pageSize = 6;
            var allCategories = JsonConvert.DeserializeObject<List<Category>>(await client.GetStringAsync("categories/getAll"));
            var categories = JsonConvert.DeserializeObject<List<Category>>(await client.GetStringAsync("categories/?currentPage=" + currentPage));
            if (!name.Equals(""))
            {
                categories = JsonConvert.DeserializeObject<List<Category>>(await client.GetStringAsync("categories/?name=" + name));
                allCategories = JsonConvert.DeserializeObject<List<Category>>(await client.GetStringAsync("categories/?name=" + name));
            }
            else
            {
                categories = JsonConvert.DeserializeObject<List<Category>>(await client.GetStringAsync("categories/?currentPage=" + currentPage));
            }
            ViewBag.TotalPage = allCategories.Count() % pageSize == 0 ? allCategories.Count() / pageSize : allCategories.Count() / pageSize + 1;
            ViewBag.CurrentPage = currentPage;
            ViewBag.Name = name;
            return View(categories);
        }

        // GET: CategoriesController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            client.BaseAddress = new Uri(uri + id);
            var category = JsonConvert.DeserializeObject<Category>(await client.GetStringAsync("categories/"+id));
            return View(category);
        }

        // GET: CategoriesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CategoriesController/Create
        [HttpPost]
        public async Task<ActionResult> Create(Category category)
        {
            client.BaseAddress = new Uri(uri);
            var categories = JsonConvert.DeserializeObject<List<Category>>(await client.GetStringAsync("categories/getAll"));
            if(category.CategoryName==null || category.CategoryName.Trim().Equals(""))
            {
                ViewBag.ErrorName = "Category name can not empty!";
                return View(category);
            }
            if (categories != null)
            {
                foreach(var item in categories)
                {
                    if (item.CategoryName.ToLower().Equals(category.CategoryName.Trim().ToLower()))
                    {
                        ViewBag.ErrorName = "Category name is already exist!";
                        return View(category);
                    }                    
                }
            }
            var response = await client.PostAsJsonAsync("categories", new { CategoryName = category.CategoryName, Status = category.Status });
            TempData["msg"] = response.Content.ReadAsStringAsync().Result;
            return RedirectToAction("Index");
        }

        // GET: CategoriesController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            client.BaseAddress = new Uri(uri);
            var category = JsonConvert.DeserializeObject<Category>(await client.GetStringAsync("categories/"+id));
            return View(category);
        }

        // POST: CategoriesController/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(int id, Category category)
        {
            client.BaseAddress = new Uri(uri);
            var categories = JsonConvert.DeserializeObject<List<Category>>(await client.GetStringAsync("categories/getAll"));
            if (category.CategoryName == null || category.CategoryName.Trim().Equals(""))
            {
                ViewBag.ErrorName = "Category name can not empty!";
                return View(category);
            }
            if (categories != null)
            {
                foreach (var item in categories)
                {
                    if (item.CategoryName.ToLower().Equals(category.CategoryName.Trim().ToLower()) && item.CategoryId != category.CategoryId)
                    {
                        ViewBag.ErrorName = "Category name is already exist!";
                        return View(category);
                    }
                }
            }
            var response = await client.PutAsJsonAsync("categories/"+id, category);
            TempData["msg"] = response.Content.ReadAsStringAsync().Result;
            return RedirectToAction("Index");
        }

        // GET: CategoriesController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            client.BaseAddress = new Uri(uri);
            bool check = true;
            var dishes = JsonConvert.DeserializeObject<List<Dish>>(await client.GetStringAsync("dishes/getAll"));
            if(dishes != null)
            {
                foreach (var dish in dishes)
                {
                    if(dish.CategoryId == id)
                    {
                        check = false;
                        break;
                    }
                }
            }
            if(!check)
            {
                TempData["msg"] = "Can not delete this category because has item related!";
                return RedirectToAction("Index");
            }
            var responsd = await client.DeleteAsync("");
            TempData["msg"] = responsd.Content.ReadAsStringAsync().Result;
            return RedirectToAction("Index");
        }

        // POST: CategoriesController/Delete/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Delete(int id, IFormCollection collection)
        //{
        //    try
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
    }
}

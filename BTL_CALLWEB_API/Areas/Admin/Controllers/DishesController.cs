using BTL_CALLWEB_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace BTL_CALLWEB_API.Areas.Admin.Controllers
{
    [Area("admin")]
    [Authorize]
    public class DishesController : Controller
    {
        string url = "https://localhost:44316/api/";
        HttpClient client = new HttpClient();

        public DishesController()
        {
            client.BaseAddress = new Uri(url);
        }
        // GET: DishesController
        public async Task<ActionResult> Index(string name = "", int currentPage = 1)
        {
            client.BaseAddress = new Uri(url);
            int pageSize = 6;
            var allDishes = JsonConvert.DeserializeObject<List<Dish>>(await client.GetStringAsync("dishes/getAll"));
            var dishes = JsonConvert.DeserializeObject<List<Dish>>(await client.GetStringAsync("dishes?currentPage=" + currentPage));
            if (!name.Equals(""))
            {
                dishes = JsonConvert.DeserializeObject<List<Dish>>(await client.GetStringAsync("dishes?name="+name));
                allDishes = JsonConvert.DeserializeObject<List<Dish>>(await client.GetStringAsync("dishes/getAll?name=" + name));
            }
            else
            {
                dishes = JsonConvert.DeserializeObject<List<Dish>>(await client.GetStringAsync("dishes?currentPage=" + currentPage));   
            }
            ViewBag.TotalPage = allDishes.Count() % pageSize == 0 ? allDishes.Count()/pageSize : allDishes.Count()/pageSize + 1;
            ViewBag.Categories = JsonConvert.DeserializeObject<List<Category>>(await client.GetStringAsync("categories/getAll"));
            ViewBag.Url = "https://localhost:44316";
            ViewBag.CurrentPage = currentPage;
            ViewBag.Name = name;
            return View(dishes);
        }

        // GET: DishesController/Details/5
        public ActionResult Details(int id)
        {
            return Redirect("/home/detail?id="+id);
        }

        // GET: DishesController/Create
        public async Task<ActionResult> Create()
        {
            client.BaseAddress = new Uri(url);
            Dish dish = new Dish();
            var categories = JsonConvert.DeserializeObject<List<Category>>(await client.GetStringAsync("categories/getAll"));
            if (categories.Count()==0)
            {
                TempData["msg"] = "Do not has any category, please create 1!";
                return RedirectToAction("Index");
            }
            ViewBag.Categories = new SelectList(categories, "CategoryId", "CategoryName");
            return View(dish);
        }

        // POST: DishesController/Create
        [HttpPost]
        public async Task<ActionResult> Create(Dish dish, IFormFile fileImage)
        {
            client.BaseAddress = new Uri(url);
            bool check = true;
            var categories = JsonConvert.DeserializeObject<List<Category>>(await client.GetStringAsync("categories/getAll"));
            var dishes = JsonConvert.DeserializeObject<List<Dish>>(await client.GetStringAsync("dishes/getAll"));
            ViewBag.Categories = new SelectList(categories, "CategoryId", "CategoryName");
            if (dish.DishName == null || dish.DishName.Trim().Equals(""))
            {
                ViewBag.ErrorName = "Dish name can not empty!";
                check = false;
            }
            if (dishes != null && dish.DishName!=null)
            {
                foreach(var item in dishes)
                {
                    if (item.DishName.ToLower().Equals(dish.DishName.Trim().ToLower()))
                    {
                        ViewBag.ErrorName = "Dish name is already exist!";
                    }
                }
            }
            if (dish.Price <= 0)
            {
                ViewBag.ErrorPrice = "Price is invalid!";
                check = false;
            }
            if (dish.SalePrice >= dish.Price || dish.SalePrice < 0)
            {
                ViewBag.ErrorSalePrice = "Sale price is invalid!";
                check = false;
            }
            if (dish.Description == null)
            {
                dish.Description = "";
            }
            if (fileImage == null || fileImage.Length <= 0)
            {
                ViewBag.ErrorImage = "Please choose Image!";
                check = false;
            }
            if (!check)
            {
                return View(dish);
            }
            var formData = new MultipartFormDataContent();
            formData.Add(new StringContent(dish.DishName.Trim()), "dishName");
            formData.Add(new StringContent(dish.Price.ToString()), "price");
            formData.Add(new StringContent(dish.SalePrice.ToString()), "salePrice");
            formData.Add(new StringContent(dish.Size.ToString()), "size");
            formData.Add(new StringContent(dish.Description), "description");
            formData.Add(new StringContent(dish.Status.ToString()), "status");
            formData.Add(new StringContent(dish.CategoryId.ToString()), "categoryId");
            formData.Add(new StreamContent(fileImage.OpenReadStream()), "image", fileImage.FileName);
            var response = await client.PostAsync("dishes", formData);
            TempData["msg"] = response.Content.ReadAsStringAsync().Result;
            return RedirectToAction("Index");
        }

        // GET: DishesController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            client.BaseAddress = new Uri(url);
            var dish = JsonConvert.DeserializeObject<Dish>(await client.GetStringAsync("dishes/"+id));
            var categories = JsonConvert.DeserializeObject<List<Category>>(await client.GetStringAsync("categories/getAll"));
            ViewBag.Categories = new SelectList(categories, "CategoryId", "CategoryName");
            ViewBag.Url = "https://localhost:44316";
            return View(dish);
        }

        // POST: DishesController/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(int id, Dish dish, IFormFile fileImage, string oldImage="")
        {
            client.BaseAddress = new Uri(url);
            bool check = true;
            var categories = JsonConvert.DeserializeObject<List<Category>>(await client.GetStringAsync("categories/getAll"));
            var dishes = JsonConvert.DeserializeObject<List<Dish>>(await client.GetStringAsync("dishes/getAll"));
            ViewBag.Categories = new SelectList(categories, "CategoryId", "CategoryName");
            if (dish.DishName == null || dish.DishName.Trim().Equals(""))
            {
                ViewBag.ErrorName = "Dish name can not empty!";
                check = false;
            }
            if (dishes != null)
            {
                foreach (var item in dishes)
                {
                    if (item.DishName.ToLower().Equals(dish.DishName.Trim().ToLower()) && item.DishId != dish.DishId)
                    {
                        ViewBag.ErrorName = "Dish name is already exist!";
                        check = false;
                        break;
                    }
                }
            }
            if (dish.Price <= 0)
            {
                ViewBag.ErrorPrice = "Price is invalid!";
                check = false;
            }
            if (dish.SalePrice >= dish.Price || dish.SalePrice < 0)
            {
                ViewBag.ErrorSalePrice = "Sale price is invalid!";
                check = false;
            }
            if (dish.Description == null)
            {
                dish.Description = "";
            }
            if (!check)
            {
                return View(dish);
            }
            var formData = new MultipartFormDataContent();
            formData.Add(new StringContent(dish.DishId.ToString()), "dishId");
            formData.Add(new StringContent(dish.DishName), "dishName");
            formData.Add(new StringContent(dish.Price.ToString()), "price");
            formData.Add(new StringContent(dish.SalePrice.ToString()), "salePrice");
            formData.Add(new StringContent(dish.Size.ToString()), "size");
            formData.Add(new StringContent(dish.Description), "description");
            formData.Add(new StringContent(dish.Status.ToString()), "status");
            formData.Add(new StringContent(dish.CategoryId.ToString()), "categoryId");
            if (fileImage != null && fileImage.Length > 0)
            {
                formData.Add(new StreamContent(fileImage.OpenReadStream()), "image", fileImage.FileName);
            }
            else
            {                
                formData.Add(new StringContent(oldImage), "oldImage");
            }
            var response = await client.PutAsync("dishes/"+id, formData);
            TempData["msg"] = response.Content.ReadAsStringAsync().Result;
            return RedirectToAction("Index");
        }

        // GET: DishesController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            var orderDetails = JsonConvert.DeserializeObject<List<OrderDetail>>(await client.GetStringAsync("orders/detail/getAll"));
            if (orderDetails != null)
            {
                foreach (var orderDetail in orderDetails)
                {
                    if(orderDetail.DishId == id)
                    {
                        TempData["msg"] = "Can not delete this dish because has item related!";
                        return RedirectToAction("Index");
                    }
                }
            }
            var response = await client.DeleteAsync("dishes/" + id);
            TempData["msg"] = response.Content.ReadAsStringAsync().Result;
			return RedirectToAction("Index");
		}

        // POST: DishesController/Delete/5
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

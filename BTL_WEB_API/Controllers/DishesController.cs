using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BTL_WEB_API.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Authorization;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using Microsoft.IdentityModel.Tokens;

namespace BTL_WEB_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class DishesController : ControllerBase
    {
        private readonly RESTINAContext _context;

        public DishesController(RESTINAContext context)
        {
            _context = context;
        }

        // GET: api/Dishes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Dish>>> GetDishes(string name="", int categoryId=0, int currentPage = 1)
        {
            int pageSize = 6;
            if (name.IsNullOrEmpty() && categoryId == 0)
            {
                return await _context.Dishes
                    .OrderByDescending(d=>d.DishId)
                    .Skip((currentPage-1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(); 
            }
            if(!name.IsNullOrEmpty() && categoryId == 0)
            {
                return await _context.Dishes
                    .OrderByDescending(d => d.DishId)
                    .Where(d=>d.DishName.ToLower().Contains(name.Trim().ToLower()))
                    .Skip((currentPage - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            if (name.IsNullOrEmpty() && categoryId != 0)
            {
                return await _context.Dishes
                    .OrderByDescending(d => d.DishId)
                    .Where(d => d.CategoryId==categoryId)
                    .Skip((currentPage - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            if (!name.IsNullOrEmpty() && categoryId != 0)
            {
                return await _context.Dishes
                    .OrderByDescending(d => d.DishId)
                    .Where(d => d.DishName.ToLower().Contains(name.Trim().ToLower()) && d.CategoryId == categoryId)
                    .Skip((currentPage - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            return await _context.Dishes.ToListAsync();

        }

        [HttpGet("getAll")]
        public async Task<ActionResult<IEnumerable<Dish>>> GetAll(string name = "", int categoryId = 0)
        {
            if (!name.IsNullOrEmpty() && categoryId==0)
            {
                return await _context.Dishes
                    .Where(d=>d.DishName.ToLower().Contains(name.Trim().ToLower()))
                    .ToListAsync();
            }
            if (name.IsNullOrEmpty() && categoryId == 0)
            {
                return await _context.Dishes
                    .ToListAsync();
            }
            if (!name.IsNullOrEmpty() && categoryId != 0)
            {
                return await _context.Dishes
                    .Where(d => d.DishName.ToLower().Contains(name.Trim().ToLower()) && d.CategoryId==categoryId)
                    .ToListAsync();
            }
            if (name.IsNullOrEmpty() && categoryId != 0)
            {
                return await _context.Dishes
                    .Where(d => d.CategoryId==categoryId)
                    .ToListAsync();
            }
            return await _context.Dishes.ToListAsync();
        } 


        // GET: api/Dishes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Dish>> GetDish(int id)
        {
            var dish = await _context.Dishes.FindAsync(id);

            if (dish == null)
            {
                return NotFound();
            }

            return dish;
        }

        // PUT: api/Dishes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDish(int id,[FromForm] DishImage data)
        {

            var dish = new Dish {DishId=data.DishId, DishName = data.DishName, Price = data.Price, SalePrice = data.SalePrice, Size = data.Size, Image = data.OldImage, Description=data.Description, CategoryId=data.CategoryId, Status=data.Status };
            
            if(data.Image != null && data.Image.Length > 0)
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", data.Image.FileName);
                using (var stream = System.IO.File.Create(path))
                {
                    await data.Image.CopyToAsync(stream);
                }
                dish.Image = "/images/" + data.Image.FileName;
            } 

            _context.Entry(dish).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DishExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok("Update dish success!");
        }

        // POST: api/Dishes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Dish>> PostDish([FromForm] DishImage data)
        {
            if (_context.Dishes == null)
            {
                return Problem("Entity set 'RESTINAContext.Dishes'  is null.");
            }
            var dish = new Dish { DishName = data.DishName, Price = data.Price, SalePrice = data.SalePrice, Size = data.Size, Description = data.Description, CategoryId = data.CategoryId, Status = data.Status };

            if (data.Image != null && data.Image.Length > 0)
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", data.Image.FileName);
                using (var stream = System.IO.File.Create(path))
                {
                    await data.Image.CopyToAsync(stream);
                }
                dish.Image = "/images/" + data.Image.FileName;
            }
            if(dish.DishName == null)
            {
                return Ok("Name dish can not null!");
            }
            _context.Dishes.Add(dish);
            await _context.SaveChangesAsync();

            return Ok("Create new dish success");
        }

        // DELETE: api/Dishes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDish(int id)
        {
            var dish = await _context.Dishes.FindAsync(id);
            if (dish == null)
            {
                return NotFound();
            }

            _context.Dishes.Remove(dish);
            await _context.SaveChangesAsync();

            return Ok("Delete dish success!");
        }

        private bool DishExists(int id)
        {
            return _context.Dishes.Any(e => e.DishId == id);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BTL_WEB_API.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BTL_WEB_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly RESTINAContext _context;
        IConfiguration _configuration;

        public AccountsController(RESTINAContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: api/Accounts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Account>>> GetAccount(string name = "", int currentPage = 1)
        {
            int pageSize = 6;
            if (name.IsNullOrEmpty())
            {
                return await _context.Accounts
                    .OrderByDescending(a => a.AccountId)
                    .Skip((currentPage - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            return await _context.Accounts
                    .OrderByDescending(a => a.AccountId)
                    .Where(a => a.Name.ToLower().Contains(name.Trim().ToLower()))
                    .Skip((currentPage - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
        }

        [HttpGet("getAll")]
        public async Task<ActionResult<IEnumerable<Account>>> GetAll(string name = "")
        {
            if (!name.IsNullOrEmpty())
            {
                return await _context.Accounts
                    .Where(a => a.Name.ToLower().Contains(name.Trim().ToLower()))
                    .ToListAsync();
            }
            return await _context.Accounts.ToListAsync();
        }

        // GET: api/Accounts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Account>> GetAccount(int id)
        {
            var account = await _context.Accounts.FindAsync(id);

            if (account == null)
            {
                return NotFound();
            }

            return account;
        }

        // PUT: api/Accounts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAccount(int id, [FromForm] AccountImage data)
        {
            var account = new Account { AccountId = data.AccountId, Name = data.Name, Password = data.Password, Email = data.Email, Role = data.Role, Image = data.OldImage, Phone = data.Phone};

            if (data.Image != null && data.Image.Length > 0)
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", data.Image.FileName);
                using (var stream = System.IO.File.Create(path))
                {
                    await data.Image.CopyToAsync(stream);
                }
                account.Image = "/images/" + data.Image.FileName;
            }

            if (_context.Accounts.Any(a=>a.Email == data.Email && a.AccountId != account.AccountId))
            {
                return Ok("Warning: Email is already exist!");
            }

            _context.Entry(account).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccountExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok("Update account success!");
        }

        // POST: api/Accounts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Account>> PostAccount([FromForm] AccountImage data)
        {
            if (_context.Accounts == null)
            {
                return Problem("Entity set 'RESTINAContext.Accounts'  is null.");
            }
            var account = new Account { AccountId = data.AccountId, Name = data.Name, Password = data.Password, Email = data.Email, Role = data.Role, Image = "", Phone = data.Phone };
            if (data.Image != null && data.Image.Length > 0)
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", data.Image.FileName);
                using (var stream = System.IO.File.Create(path))
                {
                    await data.Image.CopyToAsync(stream);
                }
                account.Image = "/images/" + data.Image.FileName;
            }
            if (account.Email == null)
            {
                return Ok("Account email can not null!");
            }
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();
            return Ok("Create new account success");
        }

        // DELETE: api/Accounts/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteAccount(int id)
        //{
        //    var account = await _context.Accounts.FindAsync(id);
        //    if (account == null)
        //    {
        //        return NotFound();
        //    }
        //    _context.Accounts.Remove(account);
        //    await _context.SaveChangesAsync();
        //    return NoContent();
        //}

        private bool AccountExists(int id)
        {
            return _context.Accounts.Any(e => e.AccountId == id);
        }

        //[HttpPost("login")]
        //public IActionResult Login(AccountValidate accountVal)
        //{
        //    var passmd5 = Cipher.GenerateMD5(accountVal.Password);
        //    var acc = _context.Accounts.FirstOrDefault(x => x.Name == accountVal.Username && x.Password == passmd5);
        //    if (acc != null)
        //    {
        //        //lấy key trong file cấu hình
        //        var key = _configuration["Jwt:Key"];
        //        //mã hóa ky
        //        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        //        //ký vào key đã mã hóa
        //        var signingCredential = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        //        //tạo claims chứa thông tin người dùng (nếu cần)
        //        var claims = new List<Claim>
        //        {
        //            new Claim(ClaimTypes.Role,acc.Role==1?"admin":"user"),
        //            new Claim(ClaimTypes.Name,accountVal.Username),
        //            new Claim("Picture",acc.Image??""),
        //            new Claim("AccountId",acc.AccountId.ToString())
        //        };

        //        //tạo token với các thông số khớp với cấu hình trong startup để validate
        //        var token = new JwtSecurityToken
        //        (
        //            issuer: _configuration["Jwt:Issuer"],
        //            audience: _configuration["Jwt:Audience"],
        //            expires: DateTime.Now.AddHours(1),
        //            signingCredentials: signingCredential,
        //            claims: claims
        //        );
        //        //sinh ra chuỗi token với các thông số ở trên
        //        var tokenVal = new JwtSecurityTokenHandler().WriteToken(token);
        //        //trả về kết quả cho client username và chuỗi token
        //        return new JsonResult(new { token = tokenVal });
        //    }
        //    //trả về lỗi
        //    return new JsonResult(new { message = "Đăng nhập sai" });

        //}
    }
}

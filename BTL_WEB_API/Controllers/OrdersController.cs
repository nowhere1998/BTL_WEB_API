using BTL_WEB_API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BTL_WEB_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly RESTINAContext _context;

        public OrdersController(RESTINAContext context)
        {
            _context = context;
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders(string name = "", int currentPage = 1)
        {
            int pageSize = 6;
            if (name.IsNullOrEmpty())
            {
                return await _context.Orders
                    .OrderByDescending(o=>o.OrderId)
                    .Skip((currentPage - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            return await _context.Orders
                    .OrderByDescending(o => o.OrderId)
                    .Skip((currentPage - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
        }

        [HttpGet("getAll")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders(string name = "")
        {
            if (name.IsNullOrEmpty())
            {
                return await _context.Orders
                    .ToListAsync();
            }
            return await _context.Orders
                    .ToListAsync();
        }

        [HttpGet("getAllByAccountId/{id}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrdersByAccountId(int id)
        {
            return await _context.Orders
                .Where(o=>o.AccountId == id)
                    .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _context.Orders.Where(o => o.OrderId == id).FirstAsync();

            if (order == null)
            {
                return Ok("Warning: No item!");
            }

            return order;
        }

        [HttpGet("detail/{id}")]
        public async Task<ActionResult<List<OrderDetail>>> GetOrderDetail(int id)
        {
            var orderDetail = await _context.OrderDetails
                .Where(o=>o.OrderId==id).ToListAsync();

            if (orderDetail == null)
            {
                return Ok("Warning: No item!");
            }

            return orderDetail;
        }

        [HttpGet("detail/getAll")]
        public async Task<ActionResult<List<OrderDetail>>> GetAllOrderDetail(int id)
        {
            var orderDetail = await _context.OrderDetails.ToListAsync();

            if (orderDetail == null)
            {
                return Ok("Warning: No item!");
            }

            return orderDetail;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, Order order)
        {
            if (id != order.OrderId)
            {
                return BadRequest();
            }

            _context.Entry(order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok("Update Order success!");
        }

        [HttpPost]
        public async Task<ActionResult<Category>> PostOrder(Order order)
        {
            if (_context.Orders == null)
            {
                return Ok("Warning: Entity set 'RESTINAContext.Orders'  is null.");
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return Ok("Checkout  success!");
        }

        [HttpPost("detail")]
        public async Task<ActionResult<Category>> PostOrderDetail(OrderDetail orderDetail)
        {
            if (_context.OrderDetails == null)
            {
                return Ok("Warning: Entity set 'RESTINAContext.OrderDetails'  is null.");
            }

            _context.OrderDetails.Add(orderDetail);
            await _context.SaveChangesAsync();

            return Ok("Add new order detail success!");
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(o => o.OrderId == id);
        }
    }
}

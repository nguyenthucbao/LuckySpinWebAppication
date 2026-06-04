using LuckySpin.Dto;
using LuckySpin.DTO;
using LuckySpin.Models;
using LuckySpin.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuckySpin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillsController : ControllerBase
    {
        private readonly LuckySpinContext _context;
        private readonly IBillService _billService;
        private readonly ILogger<BillsController> _logger;

        public BillsController(LuckySpinContext context, IBillService billService, ILogger<BillsController> logger)
        {
            _context = context;
            _billService = billService;
            _logger = logger;
        }


        [HttpGet]
        public async Task<ActionResult<BillWithProductsDto>> GetBills()
        {
            var b = await _context.Bills.Include(b => b.Products).Include(b => b.RewardCode).ToListAsync();

            List<BillWithProductsDto> billwithproductsdto = b.Select(b => new BillWithProductsDto
            {
                Id = b.Id,
                Code = b.RewardCode.Code,
                StoreId = b.StoreId ?? "",
                StoreLocate = b.StoreLocate,
                TotalAmount = b.TotalAmount,
                PaymentMethod = b.PaymentMethod,
                Products = b.Products.Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Quantity = p.Quantity
                }).ToList()
            }).ToList();

            return Ok(billwithproductsdto);
        }

        // GET: api/Bills/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BillWithProductsDto>> GetBillById(string id)
        {
            var bill = await _context.Bills.Include(b => b.Products).Include(b => b.RewardCode).FirstOrDefaultAsync(b => b.Id == id);

            if (bill == null)
            {
                return NotFound();
            }

            var result = new BillWithProductsDto
            {
                Id = bill.Id,
                Code = bill.RewardCode.Code,
                StoreId = bill.StoreId ?? "",
                StoreLocate = bill.StoreLocate,
                TotalAmount = bill.TotalAmount,
                PaymentMethod = bill.PaymentMethod,
                Products = bill.Products.Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Quantity = p.Quantity
                }).ToList()
            };

            return Ok(result);
        }

        // DELETE: api/Bill/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBill(string id)
        {
            var bill = await _context.Bills.FindAsync(id);
            if (bill == null)
            {
                return NotFound();
            }

            _context.Bills.Remove(bill);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        ///////////////////////////////////////////////////////////////////////////////////
        /// </summary>

        [HttpPost]
        [ProducesResponseType(typeof(GetBillResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateBill([FromBody] PostBillRequest request)
        {
            try
            {
                var result = await _billService.CreateBillAsync(request);
                return CreatedAtAction(nameof(CreateBill), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                // Bill ID bị trùng
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo bill {BillId}", request.Id);
                return StatusCode(500, new { message = "Đã xảy ra lỗi" });
            }
        }
    }
}

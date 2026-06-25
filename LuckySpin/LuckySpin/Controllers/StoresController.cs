using LuckySpin.Dto;
using LuckySpin.DTO;
using LuckySpin.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.CodeAnalysis.CSharp.SyntaxTokenParser;

namespace LuckySpin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoresController : ControllerBase
    {
        private readonly LuckySpinContext _context;

        public StoresController(LuckySpinContext context)
        {
            _context = context;
        }

        //GET: api/Stores
        //[HttpGet]
        //public async Task<ActionResult<GetStoresInfoDto>> GetStore()
        //{
        //    var s = await _context.Stores.ToListAsync();

        //    foreach (var store in s) 
        //    { 

        //    }

        //    List<GetStoresInfoDto> getStoresInfoDto = s.Select(s => new GetStoresInfoDto
        //    {
        //        Id = s.Id,
        //        StoreLocate = s.StoreLocate,
        //        StoreAmount = bill.Sum(x => x.TotalAmount),
        //        StoreSpinCount = rewardcode.Sum(x => x.SpinCount),
        //        StoreUsedSpinCount = rewardcode.Sum(x => x.RemainingSpins),
        //    }).ToList();

        //    return Ok(billwithproductsdto);
        //}


        //GET: api/Stores/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GetStoresInfoDto>> GetStoreById(string id)
        {
            var store = await _context.Stores.FindAsync(id);
            if (store == null)
                return NotFound();

            var bills = await _context.Bills
                .Include(b => b.Products)
                .Include(b => b.RewardCode)
                .Where(b => b.StoreId == id)
                .ToListAsync();

            if (!bills.Any())
                return NotFound();

            var billIds = bills.Select(b => b.Id).ToList();

            var rewardCodes = await _context.RewardCodes
                .Where(r => billIds.Contains(r.BillId))
                .ToListAsync();

            List<BillWithProductsDto> billwithproductsdto = bills.Select(b => new BillWithProductsDto
            {
                Id = b.Id,
                Code = b.RewardCode.Code,
                StoreId = b.StoreId ?? "",
                StoreLocate = b.StoreLocate,
                TotalAmount = b.TotalAmount,
                PaymentMethod = b.PaymentMethod,
                Products = b.Products.Select(p => new DbProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Quantity = p.Quantity
                }).ToList()
            }).ToList();


            var result = new GetStoresInfoDto
            {
                Id = store.Id,
                StoreLocate = store.StoreLocate,
                StoreAmount = bills.Sum(x => x.TotalAmount),
                StoreSpinCount = rewardCodes.Sum(x => x.SpinCount),
                StoreUsedSpinCount = rewardCodes.Sum(x => x.SpinCount) - rewardCodes.Sum(x => x.RemainingSpins),
                BillWithProducts = billwithproductsdto,
            };
            return Ok(result);
        }

    }
}

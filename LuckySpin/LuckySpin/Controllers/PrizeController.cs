using LuckySpin.Auth;
using LuckySpin.Models;
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
    public class PrizeController : ControllerBase
    {
        private readonly LuckySpinContext _context;

        public PrizeController(LuckySpinContext context)
        {
            _context = context;
        }

        //GET: api/Prize
        [HttpGet("getprizes")]
        public async Task<ActionResult<IEnumerable<Prize>>> GetPrizes()
        {
            return await _context.Prizes.ToListAsync();
        }


        /// DEBUNGING
        [HttpPost("resetprizes")]
        [AdminApiKey]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ResetPrize()
        {

            await _context.Prizes // reset prize to default values
                .ExecuteUpdateAsync(setters => setters
                .SetProperty(p => p.WinnerId, (string)null) 
                .SetProperty(p => p.IsActive, true));

            var rewardCodes = await _context.RewardCodes // reset reward codes to default values
                .Include(rc => rc.Bill)
                .ToListAsync();

            foreach (var rc in rewardCodes)
            {
                rc.RemainingSpins = rc.SpinCount;
            }

            await _context.WinnerSessions.ExecuteDeleteAsync();

            await _context.SaveChangesAsync();

            return Ok();
        }


        //[HttpPost("admin/prizesupply")]
        //public async Task<IActionResult> PrizeSupply()
        //{

        //    await _context.Prizes // reset prize to default values
        //        .ExecuteUpdateAsync(setters => setters
        //        .SetProperty(p => p.WinnerId, (string)null)
        //        .SetProperty(p => p.IsActive, true));

        //    var rewardCodes = await _context.RewardCodes // reset reward codes to default values
        //        .Include(rc => rc.Bill)
        //        .ToListAsync();

        //    foreach (var rc in rewardCodes)
        //    {
        //        rc.RemainingSpins = rc.SpinCount;
        //    }

        //    await _context.WinnerSessions.ExecuteDeleteAsync();

        //    await _context.SaveChangesAsync();

        //    return Ok();
        //}


    }
}



    



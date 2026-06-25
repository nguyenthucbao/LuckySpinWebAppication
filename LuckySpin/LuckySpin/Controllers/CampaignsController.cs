using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LuckySpin.Models;

[Route("api/[controller]")]
[ApiController]
public class CampaignsController : ControllerBase
{
    private readonly LuckySpinContext _context;
    public CampaignsController(LuckySpinContext context)
    {
        _context = context;
    }

    // GET: api/Campaign
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Campaign>>> GetCampaign()
    {
        return await _context.Campaigns.ToListAsync();
    }
   
}

using LuckySpin.Dto;
using LuckySpin.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class WinnerSessionsController : ControllerBase
{
    private readonly IWinnerSessionService _winnerSessionService;
    private readonly LuckySpinContext _db;
    private readonly ILogger<WinnerSessionsController> _logger;

    public WinnerSessionsController(
        IWinnerSessionService winnerSessionService,
        LuckySpinContext db,
        ILogger<WinnerSessionsController> logger)
    {
        _winnerSessionService = winnerSessionService;
        _db = db;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateWinnerSession([FromBody] PostCustomerRequest request)
    {
        try
        {
            var winnerId = await _winnerSessionService.CreateWinnerSessionAsync(request);

            // Return 200 OK with winner_id
            return Ok(new { winner_id = winnerId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi tạo winner session");
            return StatusCode(500, new { message = "Đã xảy ra lỗi hệ thống nội bộ" });
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<GetCustomerResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllWinnerSessions()
    {
        try
        {
            var sessions = await _db.WinnerSessions
                .Select(ws => new GetCustomerResponse
                {
                    Id = ws.Id,
                    RewardCodeId = ws.RewardCodeId,
                    FullName = ws.FullName,
                    Phone = ws.Phone,
                    Email = ws.Email,
                    Address = ws.Address,
                    CreatedAt = ws.CreatedAt
                })
                .ToListAsync();

            return Ok(sessions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi lấy toàn bộ danh sách winner sessions");
            return StatusCode(500, new { message = "Đã xảy ra lỗi hệ thống khi lấy dữ liệu" });
        }
    }


}
using Azure.Core;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.InkML;
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
    private readonly LuckySpinContext _context;
    private readonly ILogger<WinnerSessionsController> _logger;

    public WinnerSessionsController(
        LuckySpinContext context,
        ILogger<WinnerSessionsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateWinnerSession([FromBody] PostCustomerRequest req)
    {
        try
        {
            string sessionId = $"CUSTOMER-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";

            var winnerSession = new WinnerSession
            {
                Id = sessionId,
                FullName = req.FullName,
                Phone = req.Phone,
                Email = req.Email,
                Address = req.Address,
                CreatedAt = DateTime.UtcNow
            };

            _context.WinnerSessions.Add(winnerSession);
            await _context.SaveChangesAsync();

            // Return 200 OK with winner_id
            return Ok(new { winner_id = sessionId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi tạo winner session");
            return StatusCode(500, new { message = "Đã xảy ra lỗi hệ thống nội bộ" });
        }
    }

    [HttpPost("AssignWinnerToPrize")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AssignWinnerToPrize([FromBody] AssignPrizeWinnerRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.WinnerId) || string.IsNullOrWhiteSpace(request.PrizeId))
            return BadRequest(new { message = "WinnerId and PrizeId are required" });

        try
        {
            // Find prize
            var prize = await _context.Prizes.FindAsync(request.PrizeId);
            if (prize == null)
                return NotFound(new { message = "Prize not found", prize_id = request.PrizeId });

            // Optional: verify winner exists
            var winner = await _context.WinnerSessions.FindAsync(request.WinnerId);
            if (winner == null)
                return NotFound(new { message = "Winner session not found", winner_id = request.WinnerId });

            // Assign winner // Gán winner cho prize
            prize.WinnerId = request.WinnerId;
            _context.Prizes.Update(prize);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Winner assigned to prize", prize_id = prize.Id, winner_id = prize.WinnerId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi gán winner cho prize");
            return StatusCode(500, new { message = "Đã xảy ra lỗi hệ thống khi cập nhật prize" });
        }
    }

    [HttpGet("GetCustomerList")]
    [ProducesResponseType(typeof(List<GetCustomerResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllWinnerSessions()
    {
        try
        {
            var sessions = await _context.WinnerSessions
                .Select(ws => new GetCustomerResponse
                {
                    Id = ws.Id,
                    FullName = ws.FullName,
                    Phone = ws.Phone,
                    Email = ws.Email,
                    Address = ws.Address,
                    CreatedAt = ws.CreatedAt ?? default(DateTime)
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

    [HttpGet("GetByPhoneNumber/{phone}")]
    public async Task<IActionResult> GetCustomerByPhone(string phone)
    {
        try
        {
            var customer = await _context.WinnerSessions
                .Where(c => c.Phone == phone)
                .FirstOrDefaultAsync();

            if (customer == null)
                return NotFound(new { message = "Không tìm thấy khách hàng" });

            var result = new GetAccountByPhone
            {
                Id = customer.Id,
                Name = customer.FullName,
                Phone = customer.Phone,
                Mail = customer.Email,
                Address = customer.Address
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi lấy thông tin giải thưởng theo số điện thoại {Phone}", phone);
            return StatusCode(500, new { message = "Đã xảy ra lỗi hệ thống khi lấy dữ liệu" });
        }
    }


    [HttpGet("GetCustomerPrizeById/{winnerId}")]
    public async Task<IActionResult> GetCustomerPrizeById(string winnerId)
    {
        try
        {
            var customer = await _context.WinnerSessions.FindAsync(winnerId);

            if (customer == null)
                return NotFound(new { message = "Không tìm thấy khách hàng" });

            var result = _context.Prizes
                    .Where(p => p.WinnerId == winnerId)
                    .Select(r => new GetCustomerPrize
                    {
                        Id = r.Id,
                        Name = r.Name,
                        PrizeType = r.PrizeType,
                        Quantity = r.Quantity,
                        SignatureKey = r.SignatureKey
                    }).ToList();

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Đã xảy ra lỗi hệ thống khi lấy dữ liệu" });
        }
    }


    



}
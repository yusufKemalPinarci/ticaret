using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotebookTherapy.Infrastructure.Data;

namespace NotebookTherapy.API.Controllers;

[ApiController]
[Route("api/admin/stats")]
[Authorize(Roles = "Admin")]
public class AdminStatsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AdminStatsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetStats()
    {
        var totalRevenue = await _context.Orders
            .Where(o => o.PaymentStatus == "Succeeded")
            .SumAsync(o => o.TotalAmount);

        var totalOrders = await _context.Orders.CountAsync();

        var pendingOrders = await _context.Orders
            .CountAsync(o => o.Status == "Pending" || o.Status == "Processing");

        var topProducts = await _context.OrderItems
            .GroupBy(i => new { i.ProductId, i.Product.Name })
            .Select(g => new
            {
                ProductId = g.Key.ProductId,
                ProductName = g.Key.Name,
                TotalSold = g.Sum(i => i.Quantity),
                Revenue = g.Sum(i => i.Quantity * i.UnitPrice)
            })
            .OrderByDescending(x => x.TotalSold)
            .Take(5)
            .ToListAsync();

        return Ok(new
        {
            TotalRevenue = totalRevenue,
            TotalOrders = totalOrders,
            PendingOrders = pendingOrders,
            TopProducts = topProducts
        });
    }
}

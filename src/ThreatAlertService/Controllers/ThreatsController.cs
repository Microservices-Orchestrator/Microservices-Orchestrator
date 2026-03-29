using Microsoft.AspNetCore.Mvc;
using ThreatAlertService.Models;
using ThreatAlertService.Services;

namespace ThreatAlertService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ThreatsController : ControllerBase
{
    private readonly MongoDbService _mongoDbService;

    public ThreatsController(MongoDbService mongoDbService)
    {
        _mongoDbService = mongoDbService;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] ThreatAlert newAlert)
    {
        await _mongoDbService.CreateAsync(newAlert);
        return CreatedAtAction(nameof(Post), new { id = newAlert.Id }, newAlert);
    }
}
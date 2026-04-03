using Microsoft.AspNetCore.Mvc;
using ThreatAlertService.Models;
using ThreatAlertService.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ThreatAlertService.Controllers;

[ApiController]
[Route("api/threats")]
public class ThreatsController : ControllerBase
{
    private readonly MongoDbService _mongoDbService;

    public ThreatsController(MongoDbService mongoDbService)
    {
        _mongoDbService = mongoDbService;
    }

    [HttpGet]
    public async Task<ActionResult<List<ThreatAlert>>> Get()
    {
        var alerts = await _mongoDbService.GetAsync();
        return Ok(alerts);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] ThreatAlert newAlert)
    {
        // ID'nin veritabanı tarafından otomatik atanmasını sağlamak için null yapıyoruz.
        newAlert.Id = null;
        // Kayıt zamanını, istemciye güvenmek yerine sunucu tarafında belirliyoruz.
        newAlert.Timestamp = DateTime.UtcNow;

        await _mongoDbService.CreateAsync(newAlert);

        // Kaynak oluşturulduktan sonra, o kaynağa erişilebilecek URL'i döndürmek en iyi REST pratiğidir.
        // Bir sonraki adımda (Commit 17) oluşturacağımız tekil getirme metoduna şimdiden referans veriyoruz.
        return CreatedAtAction("GetThreatById", new { id = newAlert.Id }, newAlert);
    }
}
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

    [HttpGet("{id:length(24)}", Name = "GetThreatById")]
    public async Task<ActionResult<ThreatAlert>> GetThreatById(string id)
    {
        var alert = await _mongoDbService.GetAsync(id);

        if (alert is null)
        {
            return NotFound();
        }

        return Ok(alert);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] ThreatAlert newAlert)
    {
        // ID'yi kaydetmeden önce kendimiz oluşturuyoruz ki rota kurallarına (length:24) uysun ve 500 hatası vermesin.
        newAlert.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
        // Kayıt zamanını, istemciye güvenmek yerine sunucu tarafında belirliyoruz.
        newAlert.Timestamp = DateTime.UtcNow;

        await _mongoDbService.CreateAsync(newAlert);

        // Kaynak oluşturulduktan sonra, o kaynağa erişilebilecek URL'i döndürmek en iyi REST pratiğidir.
        // Bir sonraki adımda (Commit 17) oluşturacağımız tekil getirme metoduna şimdiden referans veriyoruz.
        return CreatedAtRoute("GetThreatById", new { id = newAlert.Id }, newAlert);
    }
}
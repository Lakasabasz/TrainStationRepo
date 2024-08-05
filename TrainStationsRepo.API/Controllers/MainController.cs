using Microsoft.AspNetCore.Mvc;
using TrainStationsRepo.API.Models;

namespace TrainStationsRepo.API.Controllers;

public record RoutePoints(
    List<int> stations
);

[Route("/")]
[Route("/api")]
[ApiController]
public class MainController : Controller
{
    [HttpGet("/")]
    public IActionResult Index()
    {
        return Ok(new { motd = "sudo rm -rf /" });
    }

    [HttpGet("stations")]
    public IActionResult StationsQuery([FromQuery(Name = "query")] string? query = null)
    {
        query = string.IsNullOrWhiteSpace(query) ? "" : query;
        var result = World.Instance.Posts
            .Where(x => $"{x.Name} {x.Type}".Contains(query, StringComparison.InvariantCultureIgnoreCase))
            .Select(x => new { x.Id, x.Name, Discriminant = x.Type })
            .OrderBy(x => x.Name).ThenBy(x => x.Discriminant)
            .Take(10).ToList();
        return Ok(new{stations = result});
    }

    [HttpGet("route")]
    [HttpPost("route")]
    public IActionResult AlternativeRoutes([FromBody] RoutePoints request)
    {
        if (request.stations[0] == request.stations[^1])
            return BadRequest(new ErrorModel("Start and end cannot be the same", ErrorCodes.StartSameAsEnd));
        try
        {
            List<List<Post>> routes = World.Instance.GetAlternatives(request.stations.First(), request.stations.Last());
            return Ok(new { routes = routes.Select(r => r.Select(p => new { p.Id, p.Name, Discriminant = p.Type })) });
        }
        catch (InvalidOperationException)
        {
            return BadRequest(new ErrorModel("Start or end id is invalid", ErrorCodes.StationsIdInvalid));
        }
        catch (Exception)
        {
            return BadRequest(new { error = "SWW", ecc = 69 });
        }
    }

    [HttpGet("stations/all")]
    public IActionResult StationsAll()
    {
        return Ok(new Stations(World.Instance.Posts.Select(x => new Station(x.Id, x.Name, x.Type))));
    }
}
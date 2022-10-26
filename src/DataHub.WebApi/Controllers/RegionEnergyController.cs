namespace DataHub.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class RegionEnergyController : ControllerBase
{
    private IRegionEnergyRepository _repo;

    public RegionEnergyController(IRegionEnergyRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public async Task<IEnumerable<RegionEnergyResponse>> Get(CancellationToken token)
    {
        var entires = await _repo.GetAllRegionEnergy(token);
        var result = entires.Select(x => new RegionEnergyResponse(x.Region, x.Energy)).ToArray();
        return result;
    }
}
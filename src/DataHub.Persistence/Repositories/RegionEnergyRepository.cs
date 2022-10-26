namespace DataHub.Persistence;

public interface IRegionEnergyRepository
{
    Task<RegionEnergy[]> GetAllRegionEnergy(CancellationToken token);
}

public class RegionEnergyRepository : IRegionEnergyRepository
{
    private readonly DataHubContext _context;

    public RegionEnergyRepository(DataHubContext context)
    {
        _context = context;
    }

    public async Task<RegionEnergy[]> GetAllRegionEnergy(CancellationToken token)
    {
        var entries = await _context.RegionEnergies.AsNoTracking().ToArrayAsync();
        return entries;
    }
}
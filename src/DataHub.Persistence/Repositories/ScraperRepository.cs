namespace DataHub.Persistence;

public interface IScraperRepository
{
    Task<bool> EnergyReportExists(string hash, CancellationToken token);
    Task InsertEnergyReport(string hash, CancellationToken token);
    Task<IDictionary<string, RegionEnergy>> GetRegionEnergyDictionary(CancellationToken token);
    Task InsertRegionEnergy(RegionEnergy regionEnergy, CancellationToken token);
    Task<int> Commit(CancellationToken token);
}

public class ScraperRepository : IScraperRepository 
{
    private readonly DataHubContext _context;

    public ScraperRepository(DataHubContext context)
    {
        _context = context;
    }

    public async Task<bool> EnergyReportExists(string hash, CancellationToken token)
    {
        var result = await _context.EnergyReports.AnyAsync(x => x.Hash == hash, token);
        return result;
    }   

    public async Task InsertEnergyReport(string hash, CancellationToken token)
    {
        var energyReport = new EnergyReport()
        {
            Hash = hash
        };
        
        await _context.EnergyReports.AddAsync(energyReport, token);
    }

    public async Task<IDictionary<string, RegionEnergy>> GetRegionEnergyDictionary(CancellationToken token)
    {
        var entries = await _context.RegionEnergies.ToArrayAsync();
        if (entries is null || entries.Length == 0)
            return new Dictionary<string, RegionEnergy>();
        var dict = entries.ToDictionary(x => x.Region, x => x);
        return dict;
    }

    public async Task InsertRegionEnergy(RegionEnergy regionEnergy, CancellationToken token)
    {
        await _context.RegionEnergies.AddAsync(regionEnergy, token);
    }

    public async Task<int> Commit(CancellationToken token)
    {
        var result = await _context.SaveChangesAsync(token);
        return result;
    }
}
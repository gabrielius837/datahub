namespace DataHub.Persistence;

public class DataHubContext : DbContext
{
    public DataHubContext(DbContextOptions options) : base(options)
    {
        
    }

    public DbSet<EnergyReport> EnergyReports => Set<EnergyReport>();
    public DbSet<RegionEnergy> RegionEnergies => Set<RegionEnergy>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataHubContext).Assembly);
    }
}
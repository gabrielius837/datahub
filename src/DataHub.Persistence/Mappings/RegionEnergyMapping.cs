namespace DataHub.Persistence;

public class RegionEnergyMapping : IEntityTypeConfiguration<RegionEnergy>
{
    public void Configure(EntityTypeBuilder<RegionEnergy> builder)
    {
        builder
            .ToTable("region_energy");
        builder
            .Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        builder
            .HasKey(x => x.Id)
            .HasName("PK_region_energy");
        builder
            .Property(x => x.Region)
            .IsRequired(true)
            .HasColumnName("region");
        builder
            .HasIndex(x => x.Region)
            .HasDatabaseName("UQ_region_energy_energy")
            .IsUnique(true);
        builder
            .Property(x => x.Energy)
            .IsRequired(true)
            .HasColumnName("energy");
    }
}
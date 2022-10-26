namespace DataHub.Persistence;

public class EnergyReportMapping : IEntityTypeConfiguration<EnergyReport>
{
    public void Configure(EntityTypeBuilder<EnergyReport> builder)
    {
        builder
            .ToTable("energy_report");
        builder
            .Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        builder
            .HasKey(x => x.Id)
            .HasName("PK_energy_report");
        builder
            .Property(x => x.Hash)
            .IsRequired(true)
            .HasColumnName("hash");
        builder
            .HasIndex(x => x.Hash)
            .HasDatabaseName("UQ_energy_report_hash")
            .IsUnique(true);
    }
}
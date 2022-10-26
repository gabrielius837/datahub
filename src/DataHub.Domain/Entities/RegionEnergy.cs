namespace DataHub.Domain;

public class RegionEnergy : BaseEntity<int>
{
    public string Region { get; set; } = default!;
    public decimal Energy { get; set; }
}
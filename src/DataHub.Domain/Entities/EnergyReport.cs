namespace DataHub.Domain;

public class EnergyReport : BaseEntity<int>
{
    public string Hash { get; set; } = default!;
}
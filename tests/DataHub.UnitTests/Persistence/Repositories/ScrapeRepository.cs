namespace DataHub.UnitTests;

public class EnergyReportRepository_Tests
{
    private const string EXISTING_HASH = "9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08";
    private const string NONEXISTING_HASH = "3975a18c2114c4d05cf045572f1aa70fdecff1f999127a537d189765e990ae8b";

    [Theory]
    [InlineData(EXISTING_HASH, true)]
    [InlineData(NONEXISTING_HASH, false)]
    public async Task EnergyReportExists_MustMatchExpectedResult(string hash, bool expected)
    {
        // arrange
        var options = new DbContextOptionsBuilder<DataHubContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        using var context = new DataHubContext(options);
        var energyReport = new EnergyReport() { Hash = EXISTING_HASH };
        await context.EnergyReports.AddAsync(energyReport);
        await context.SaveChangesAsync();
        var repo = new ScraperRepository(context);

        // act
        var result = await repo.EnergyReportExists(hash, default);

        // assert
        Assert.Equal(expected, result);
    }
}
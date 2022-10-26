namespace DataHub.UnitTests;

public class RegionEnergyScraper_Tests
{
    [Fact]
    public async Task RegionEnergyScraper_MustReturnZero_WhenProcessingDuplicateReport()
    {
        // arrange
        const string MSG = "TINKLAS,OBT_PAVADINIMAS,OBJ_GV_TIPAS,OBJ_NUMERIS,P+,PL_T,P-\nAlytaus regiono tinklas,Butas,Ne GV,37502,0.05,2020-06-30 00:00:00,0.0\nVilniaus regiono tinklas,Namas,G,747522,0.4298,2020-06-30 00:00:00,0.0\nVilniaus regiono tinklas,Namas,G,690996,,2020-06-30 00:00:00,\nVilniaus regiono tinklas,Butas,Ne GV,600022,0.023,2020-06-30 00:00:00,";
        const string URL = "https://data.gov.lt/dataset/1975/download/10766/2022-05.csv";
        var bytes = RegionEnergyScraper.CP_1257.GetBytes(MSG);
        var response = new HttpResponseMessage()
        {
            Content = new ByteArrayContent(bytes),
            StatusCode = HttpStatusCode.OK
        };
        var handler = new StubHttpHandler(response);
        var client = new HttpClient(handler);
        var logger = NullLogger<RegionEnergyScraper>.Instance;
        var repo = new Mock<IScraperRepository>();
        repo.Setup(x => x.EnergyReportExists(It.IsAny<string>(), default))
            .ReturnsAsync(true);
        var scraper = new RegionEnergyScraper(logger, client, repo.Object);

        // act
        var result = await scraper.ScrapePowerData(URL, default);

        // arrange
        // no changes occured
        Assert.Equal(0, result);
    }

    [Theory]
    // add
    [InlineData(2, "TINKLAS,OBT_PAVADINIMAS,OBJ_GV_TIPAS,OBJ_NUMERIS,P+,PL_T,P-\nAlytaus regiono tinklas,Butas,Ne GV,37502,0.05,2020-06-30 00:00:00,0.0\nVilniaus regiono tinklas,Butas,Ne GV,600022,0.023,2020-06-30 00:00:00,")]
    // group
    [InlineData(1, "TINKLAS,OBT_PAVADINIMAS,OBJ_GV_TIPAS,OBJ_NUMERIS,P+,PL_T,P-\nAlytaus regiono tinklas,Butas,Ne GV,37502,0.05,2020-06-30 00:00:00,0.0\nAlytaus regiono tinklas,Butas,Ne GV,600022,0.023,2020-06-30 00:00:00,")]
    // ignore incorrect
    [InlineData(0, "TINKLAS,OBT_PAVADINIMAS,OBJ_GV_TIPAS,OBJ_NUMERIS,P+,PL_T,P-\nAlytaus regiono tinklas,Butas,Ne GV,37502,-0.05,2020-06-30 00:00:00,0.0\n  ,Butas,Ne GV,600022,0.023,2020-06-30 00:00:00,")]
    // ignore unnecessary
    [InlineData(0, "TINKLAS,OBT_PAVADINIMAS,OBJ_GV_TIPAS,OBJ_NUMERIS,P+,PL_T,P-\nAlytaus regiono tinklas,Namas,Ne GV,37502,0.05,2020-06-30 00:00:00,0.0")]
    public async Task ScrapePowerData_DictMustContainExpectedCount(int expectedCount, string csv)
    {
        // arrange
        const string URL = "https://data.gov.lt/dataset/1975/download/10766/2022-05.csv";
        const int COMMIT = -1;
        var bytes = RegionEnergyScraper.CP_1257.GetBytes(csv);
        var response = new HttpResponseMessage()
        {
            Content = new ByteArrayContent(bytes),
            StatusCode = HttpStatusCode.OK
        };
        var handler = new StubHttpHandler(response);
        var client = new HttpClient(handler);
        var logger = NullLogger<RegionEnergyScraper>.Instance;
        var repo = new Mock<IScraperRepository>();
        repo.Setup(x => x.EnergyReportExists(It.IsAny<string>(), default))
            .ReturnsAsync(false);
        repo.Setup(x => x.Commit(default))
            .ReturnsAsync(COMMIT);
        var flag = false;
        repo.Setup(x => x.InsertEnergyReport(It.IsAny<string>(), default))
            .Returns(Task.CompletedTask)
            .Callback(() => flag = true);
        var count = 0;
        repo.Setup(x => x.InsertRegionEnergy(It.IsAny<RegionEnergy>(), default))
            .Returns(Task.CompletedTask)
            .Callback(() => count++);
        var dict = new Dictionary<string, RegionEnergy>();
        repo.Setup(x => x.GetRegionEnergyDictionary(default))
            .ReturnsAsync(dict);
        var scraper = new RegionEnergyScraper(logger, client, repo.Object);

        // act
        var result = await scraper.ScrapePowerData(URL, default);

        // arrange
        Assert.Equal(COMMIT, result);
        Assert.True(flag);
        Assert.Equal(expectedCount, count);
    }
}

public class StubHttpHandler : HttpMessageHandler
{
    private HttpResponseMessage _msg;

    public StubHttpHandler(HttpResponseMessage msg)
    {
        _msg = msg;
    }
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return Task.FromResult(_msg);
    }
}
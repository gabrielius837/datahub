using System.Security.Cryptography;
using System.Text;

namespace DataHub.WebApi;

public interface IRegionEnergyScraper
{
    Task<int> ScrapePowerData(string source, CancellationToken token);
}

public class RegionEnergyScraper : IRegionEnergyScraper
{
    private const int CSV_COLUMN_COUNT = 7;
    private const char NEWLINE = '\n';
    public static readonly Encoding CP_1257 =
        CodePagesEncodingProvider.Instance.GetEncoding(1257) ?? throw new ArgumentNullException("cp-1257 encoding is missing");

    private readonly ILogger<RegionEnergyScraper> _logger;
    private readonly IScraperRepository _repository;
    private readonly HttpClient _client;

    public RegionEnergyScraper(ILogger<RegionEnergyScraper> logger, HttpClient client, IScraperRepository repository)
    {
        _logger = logger;
        _repository = repository;
        _client = client;
    }

    public async Task<int> ScrapePowerData(string source, CancellationToken token)
    {
        var name = Guid.NewGuid().ToString();

        try
        {
            using (var ds = await _client.GetStreamAsync(source, token))
            using (var fs = File.Open(name, FileMode.Create))
            {
                await ds.CopyToAsync(fs);
            }

            var hash = GetHash(name);
            var exists = await _repository.EnergyReportExists(hash, token);

            if (exists)
            {
                _logger.LogInformation("Skipping over consumed report ({hash}) from: {source}", hash, source);
                return 0;
            }

            await _repository.InsertEnergyReport(hash, token);

            var dict = await _repository.GetRegionEnergyDictionary(token);
            using (var fs = File.OpenRead(name))
            using (var sr = new StreamReader(fs, CP_1257))
            {
                // skip csv header
                var line = await sr.ReadLineAsync();
                while ((line = await sr.ReadLineAsync()) is not null)
                    await ProcessLine(line, dict, token);
            }

            var result = await _repository.Commit(token);
            return result;
        }
        finally
        {
            if (File.Exists(name))
                File.Delete(name);
        }
    }

    private async Task ProcessLine(string line, IDictionary<string, RegionEnergy> dict, CancellationToken token)
    {
        var columns = line.Split(',');
        if (columns.Length != CSV_COLUMN_COUNT)
        {
            _logger.LogWarning("Unexpected column count in csv line: {line}", line);
            return;
        }

        if (columns[1] != "Butas")
            return;

        var region = columns[0];
        decimal.TryParse(columns[4], out decimal energy);

        if (string.IsNullOrWhiteSpace(region) || energy < 0)
        {
            _logger.LogWarning("Unexpected values in line: {line}", line);
            return;
        }

        if (dict.ContainsKey(region))
            dict[region].Energy += energy;
        else
        {
            var regionEnergy = new RegionEnergy()
            {
                Region = region,
                Energy = energy
            };

            await _repository.InsertRegionEnergy(regionEnergy, token);
            dict.Add(region, regionEnergy);
        }
    }

    private static string GetHash(string name)
    {
        using var fs = File.OpenRead(name);
        using SHA256 sha256Hash = SHA256.Create();
        var output = sha256Hash.ComputeHash(fs);
        var sb = new StringBuilder();
        for (int i = 0; i < output.Length; i++)
            sb.Append(output[i].ToString("x2"));
        return sb.ToString();
    }
}
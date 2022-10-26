var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, services, config) =>
    config
        .ReadFrom.Configuration(ctx.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console()
);

// Add services to the container.
builder.Services.AddScoped<IScraperRepository, ScraperRepository>();
builder.Services.AddScoped<IRegionEnergyRepository, RegionEnergyRepository>();
builder.Services.AddScoped<IRegionEnergyScraper, RegionEnergyScraper>();
builder.Services.AddDbContext<DataHubContext>
(
    options => options.UseNpgsql
    (
        builder.Configuration.GetConnectionString("Default"),
        x => x.MigrationsAssembly(typeof(DataHubContext).Assembly.ToString())
    )
);
builder.Services.AddHttpClient<IRegionEnergyScraper, RegionEnergyScraper>()
    .AddPolicyHandler(Policy.HandleResult<HttpResponseMessage>(resp => !resp.IsSuccessStatusCode).RetryAsync(3));
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

await InitializeData(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseTraceId();
app.UseErrorHandling();
//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

// I could scrape the provided webpage by retrieving two most recent datasets, but whatever...
static async Task InitializeData(WebApplication app)
{

    var urls = app.Configuration.GetSection("DataSets:Urls").Get<string[]>();
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<DataHubContext>();
    context.Database.EnsureCreated();
    var scraper = scope.ServiceProvider.GetRequiredService<IRegionEnergyScraper>();
    foreach (var url in urls)
    {
        var count = await scraper.ScrapePowerData(url, default);
        System.Console.WriteLine(count);
    }
}

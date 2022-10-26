
namespace DataHub.UnitTests;

public class RegionEnergyController_Tests
{
    [Fact]
    public async Task Get_ReturnsSameResultArray()
    {
        // arrange
        var repo = new Mock<IRegionEnergyRepository>();
        var array = new RegionEnergy[]
        {
            new RegionEnergy() { Id = 1, Region = "test", Energy = 2m },
            new RegionEnergy() { Id = 2, Region = "pest", Energy = 3m },
        };
        repo.Setup(x => x.GetAllRegionEnergy(default))
            .Returns(Task.FromResult(array));
        var controller = new RegionEnergyController(repo.Object);

        // act
        var result = await controller.Get(default);

        // assert
        Assert.NotNull(result);
        Assert.Equal(array.Length, result.Length);
        for (int i = 0; i < array.Length; i++)
        {
            var inputElement = array[i];
            var outputElement = result[i];
            Assert.Equal(inputElement.Region, outputElement.region);
            Assert.Equal(inputElement.Energy, outputElement.energy);
        }
    }
}
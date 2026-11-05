using Gym.Dsa;

namespace Gym.Tests.Dsa;

public class CountingTests
{

    [Fact]
    public void Get_on_empty_map_returns_minus_one()
    {
        var nums = new int[] {1,3,3,2,1,3};
        var result = Counting.CountEach(nums);

        Assert.Equal(2, result[1]);
        Assert.Equal(3, result[3]);
        Assert.Equal(1, result[2]);
    }
}
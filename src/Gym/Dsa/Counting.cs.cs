namespace Gym.Dsa;

public static class Counting
{
    public static Dictionary<int, int> CountEach(int[] nums)
    {
        var seen = new Dictionary<int, int>();
        for (int i = 0; i < nums.Length; i++)
        {
            int need = nums[i];
            if (seen.TryGetValue(need, out int j))
            {
                seen[nums[i]] = j+1;
                continue;
            }
            seen[nums[i]] = 1;
        }
        return seen;
    }
    // public static int MostCommon(int[] nums)
}
namespace Gym.Day01
{
    public static class Warmup
    {
        public static List<string> FizzBuzz(int n)
        {
            if(n > 0)
            {
                var result = new List<string>();
                for (int i = 1; i <= n; i++)
                {
                    if (i % 3 == 0 && i % 5 == 0)
                        result.Add("FizzBuzz");
                    else if (i % 3 == 0)
                        result.Add("Fizz");
                    else if (i % 5 == 0)
                        result.Add("Buzz");
                    else
                        result.Add(i.ToString());
                }
                return result;
            }
            else if(n == 0)
            {
                return new List<string>();
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(n), "Input must be a non-negative integer.");
            }
        }
    }
}
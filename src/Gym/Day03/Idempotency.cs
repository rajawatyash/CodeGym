namespace Gym.Day03
{
    public static class Idempotency
    {
        public static string? FirstRepeatedKey(IEnumerable<string> keys)
        {
            ArgumentNullException.ThrowIfNull(keys);

            var seenKeys = new HashSet<string>();

            foreach(var key in keys)
            {
                if (!seenKeys.Add(key))
                {
                    return key;
                }
            }

            return null;
        }
    }
}
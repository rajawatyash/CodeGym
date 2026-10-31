using System.Text;

namespace Gym.Day01
{
    public static class Strings
    {
        public static string ReverseWords(string input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input), "Input cannot be null.");

            var words = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            Array.Reverse(words);
            return string.Join(" ", words);


        }

        public static string ReverseWordsUsingLinq(string input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input), "Input cannot be null.");

            return string.Join(" ", input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Reverse());
        }

        public static string ReverseWordsUsingStack(string input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input), "Input cannot be null.");

            var words = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var stack = new Stack<string>(words);
            return string.Join(" ", stack);
        }

        public static string ReverseWordsUsingList(string input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input), "Input cannot be null.");

            var words = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            words.Reverse();
            return string.Join(" ", words);
        }

        public static string ReverseWordsManual(string input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input), "Input cannot be null.");

            var words = new List<string>();
            int start = -1;
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] != ' ' && start == -1)
                {
                    start = i;
                }
                else if ((input[i] == ' ' || i == input.Length - 1) && start != -1)
                {
                    int end = (input[i] == ' ') ? i : i + 1;
                    words.Add(input.Substring(start, end - start));
                    start = -1;
                }
            }
            return string.Join(" ", words);
        }

        public static IReadOnlyList<(string Word, int Count)> TopWords(string text, int n)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text), "Text cannot be null.");
            if (n <= 0)
                throw new ArgumentOutOfRangeException(nameof(n), "n must be a positive integer.");

            // Step 1: walk the text, build each word, count it when a separator arrives.
            var counts = new Dictionary<string, int>();
            var current = new StringBuilder();

            foreach (char c in text)
            {
                if (char.IsLetterOrDigit(c))
                {
                    current.Append(char.ToLowerInvariant(c));
                }
                else
                {
                    AddWord(counts, current);
                }
            }
            AddWord(counts, current); // the last word

            // Steps 2 and 3: sort by count desc, then word asc; take n.
            return counts
                .OrderByDescending(kv => kv.Value)
                .ThenBy(kv => kv.Key, StringComparer.Ordinal)
                .Take(n)
                .Select(kv => (kv.Key, kv.Value))
                .ToList();
        }

        private static void AddWord(Dictionary<string, int> counts, StringBuilder current)
        {
            if (current.Length == 0)
                return;

            string word = current.ToString();
            counts[word] = counts.TryGetValue(word, out int count) ? count + 1 : 1;
            current.Clear();
        }
    }
}
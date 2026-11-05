using System.Globalization;
using System.Runtime.CompilerServices;

namespace Gym.Day03
{
    public record BankData(DateOnly Day, string Reference, decimal Amount);
    public record ParseResult(IReadOnlyList<BankData> Lines, IReadOnlyList<string> Errors);

    public static class StatementParser
    {
        public static ParseResult Parse(IReadOnlyList<string> lines)
        {
            ArgumentNullException.ThrowIfNull(lines);

            var bankLines = new List<BankData>();
            var errors = new List<string>();

            for (int i = 1; i < lines.Count; i++)
            {
                var lineData = CsvLine.Split(lines[i]);
                if(lineData.Count == 0 || lineData.Count ==1)
                {
                    continue;
                }
                else
                {
                    if(lineData.Count != 3)
                    {
                        errors.Add($"Line {i+1}: expected 3 fields, got {lineData.Count}");
                        continue;
                    }
                    if (!DateOnly.TryParseExact(lineData[0], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly day))
                    {
                        errors.Add($"Line {i+1}: invalid date '{lineData[0]}'");
                        continue;
                    }
                    if (string.IsNullOrWhiteSpace(lineData[1]))
                    {
                        errors.Add($"Line {i+1}: missing reference");
                        continue;
                    }
                    if (!decimal.TryParse(lineData[2], NumberStyles.Number, CultureInfo.InvariantCulture, out decimal amount))
                    {
                        errors.Add($"Line {i+1}: invalid amount '{lineData[2]}'");
                        continue;
                    }
                    bankLines.Add(new BankData(day, lineData[1], amount));
                }
            }

            return new ParseResult(bankLines, errors);
            
        }
    }
}
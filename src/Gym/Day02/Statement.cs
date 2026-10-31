using System.Text;

namespace Gym.Day02
{
    public record Payment(DateOnly Day, decimal Amount);

    public static class Statement
    {
        public static List<(DateOnly Day, decimal Total)> DailyTotals(IEnumerable<Payment> payments)
        {
            if(payments == null)
            {
                throw new ArgumentNullException(nameof(payments));
            }

            Dictionary<DateOnly, decimal> perDayStatement = new();

            foreach(var payment in payments)
            {
                if(perDayStatement.TryGetValue(payment.Day, out decimal currentValue))
                {
                    perDayStatement[payment.Day] = currentValue + payment.Amount;
                }
                else
                {
                    perDayStatement.Add(payment.Day, payment.Amount);
                }
            }

            List<(DateOnly, decimal)> values = perDayStatement.Select(kvp => (kvp.Key, kvp.Value)).OrderBy(x => x.Key).ToList();
            return values;

        }
    }
}
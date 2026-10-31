using System.Text;

namespace Gym.Day02
{
    public static class Ledger
    {
        public static decimal[] RunningBalance(decimal opening, decimal[] movements)
        {
            if(movements == null)
            {
                throw new ArgumentNullException(nameof(movements), "Input cannot be null.");
            }

            if(movements.Length == 0) return Array.Empty<decimal>();

            decimal[] results = new decimal[movements.Length];
            decimal balance = opening;

            for (int i = 0; i < movements.Length; i++)
            {
                balance = balance + movements[i];
                results[i] = balance;
            }

            return results;
        }

        public static int FirstOverdraft(decimal opening, decimal[] movements)
        {
            if(movements == null)
            {
                throw new ArgumentNullException(nameof(movements), "Input cannot be null.");
            }

            decimal balance = opening;

            for( int i = 0 ; i < movements.Length; i++)
            {
                balance = balance + movements[i];
                if(balance < 0) return i;
            }

            return -1;
        }
    }
    
}
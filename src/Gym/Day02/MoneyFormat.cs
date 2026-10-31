using System.Text;

namespace Gym.Day02
{
    public static class MoneyFormat
    {
        public static string Format(long minorUnits, string currency)
        {
            if(string.IsNullOrWhiteSpace(currency))
            {
                throw new ArgumentNullException(nameof(currency));
            }

            bool isNegative = minorUnits < 0;
            minorUnits = Math.Abs(minorUnits);

            StringBuilder sb = new StringBuilder();

            long whole = minorUnits / 100;
            long decimalPart = minorUnits % 100;

            int tensDecimal = (int)(decimalPart / 10);
            int onesDecimal = (int)(decimalPart % 10);
            sb.Insert(0, (char)('0' + onesDecimal));
            sb.Insert(0, (char)('0' + tensDecimal));
            sb.Insert(0, '.');

            if(currency == "USD")
            {
                if(whole == 0)
                {
                    sb.Insert(0, '0');
                }
                else
                {
                    int digitCount = 0;
                    
                    while (whole > 0)
                    {
                        if (digitCount > 0 && digitCount % 3 == 0)
                        {
                            sb.Insert(0, ',');
                        }
    
                        int digit = (int)(whole % 10);
                        sb.Insert(0, (char)('0' + digit));
                        digitCount++;
                        
                        whole /= 10;
                    }
                }
                sb.Insert(0, '$');

            }
            else if(currency == "INR")
            {
                if(whole == 0)
                {
                    sb.Insert(0, '0');
                }
                else
                {
                    // 1. Process the last 3 digits first (Hundreds/Tens/Ones)
                    int initialDigits = 0;
                    while (whole > 0 && initialDigits < 3)
                    {
                        int digit = (int)(whole % 10);
                        sb.Insert(0, (char)('0' + digit));
                        initialDigits++;
                        whole /= 10;
                    }
    
                    // 2. Process remaining digits in groups of 2 (Thousands, Lakhs, Crores)
                    int remainingDigitCount = 0;
                    while (whole > 0)
                    {
                        // Inject a comma right before starting a new group of 2
                        if (remainingDigitCount % 2 == 0)
                        {
                            sb.Insert(0, ',');
                        }
    
                        int digit = (int)(whole % 10);
                        sb.Insert(0, (char)('0' + digit));
                        remainingDigitCount++;
                        whole /= 10;
                    }
                }
                sb.Insert(0, '₹');
            }
            else
            {
                throw new ArgumentException(nameof(currency));
            }

            if (isNegative) sb.Insert(0, '-');
            return sb.ToString();
        }   
    }
}
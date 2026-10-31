using System.Text;

namespace Gym.Day01
{
    public static class Cards
    {
        public static bool IsValidCardNumber(string? number)
        {
            if (number != null && number.Length > 0 && number.Length >= 13 && number.Length <= 19)
            {
                number = number.Replace(" ", "");
                int count = 1;
                int sum = 0;
                for(int i = number.Length - 1; i >= 0; i--)
                {
                    if (char.IsDigit(number[i]))
                    {
                        int digit = number[i] - '0';
                        if (count %2 ==0)
                        {
                            digit *=2;
                            if(digit > 9)
                            {
                                digit -= 9;
                            }
                        }

                        sum += digit;
                        count++;
                    }
                    else
                    {
                        return false;
                    }
                }
                return sum % 10 == 0;
            }
            return false;
        }

        public static string DetectBrand(string number)
        {
            if (number != null && number.Length > 0 && number.Length >= 13 && number.Length <= 19)
            {
                number = number.Replace(" ", "");
                int digits = int.Parse(number[..4]);
                if (digits >= 4000 && digits <= 4999)
                {
                    return CardBrand.Visa.ToString();
                }
                else if (digits >= 5100 && digits <= 5599 || digits >= 2221 && digits <= 2720)
                {
                    return CardBrand.Mastercard.ToString();
                }
                else if (digits >= 3400 && digits <= 3499 || digits >= 3700 && digits <= 3799)
                {
                    return CardBrand.Amex.ToString();
                }
                else
                {
                    return CardBrand.Unknown.ToString();
                }
            }
            return CardBrand.Unknown.ToString();
        }

        public enum CardBrand { Unknown, Visa, Mastercard, Amex }
    }
}
using System.Text;

namespace Gym.Day04;

public static class CardMask
{
    public static string Mask(string number)
    {
        ArgumentNullException.ThrowIfNull(number);

        int totalLength = 0;

        foreach(char ch in number)
        {
            if(char.IsDigit(ch))
            {
                totalLength++;
                continue;
            }

            if(ch != ' ')
            {
                throw new ArgumentException("Card number should have digit or a space", nameof(number));
            }
        }

        if(totalLength < 4) throw new ArgumentException("Card number should be atleast 4 digit", nameof(number));

        var maskedNumber = new StringBuilder();
        int seen = 0;

        foreach(char c in number)
        {
            if(c == ' ')
            {
                maskedNumber.Append(c);
            }

            if(char.IsDigit(c))
            {
                if(seen < totalLength -4)
                {
                    maskedNumber.Append('*');
                }
                else
                {
                    maskedNumber.Append(c);
                }
                seen++;
            }
        }

        return maskedNumber.ToString();
    }
}
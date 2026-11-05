using System.Globalization;
using System.Text;

namespace Gym.Day05;

public record ReceiptItem(string Name, int Quantity, decimal UnitPrice);

public static class Receipt
{
    public static string Render(IReadOnlyList<ReceiptItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        var receipt = new List<string>();
        decimal total = 0;

        foreach( var item in items)
        {
            if(item.Quantity <= 0)
            {
                throw new ArgumentException("Quantity should be 1 or more than 1", nameof(item.Quantity));
            }

            var shownName = item.Name.Length > 20 ? item.Name[..17] + "..." : item.Name;
            var lineTotal = item.Quantity * item.UnitPrice;
            var lineString = $"{shownName,-20}{item.Quantity,3}{lineTotal.ToString("0.00", CultureInfo.InvariantCulture),10}";
            total += lineTotal;
            
            receipt.Add(lineString);
        }
        receipt.Add(new string('-', 33));
        receipt.Add($"{"TOTAL",-23}{total.ToString("0.00", CultureInfo.InvariantCulture),10}");

        return string.Join("\n", receipt);

    }
}
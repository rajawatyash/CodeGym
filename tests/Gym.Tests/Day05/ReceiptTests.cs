using Gym.Day05;

namespace Gym.Tests.Day05;

public class ReceiptTests
{
    [Fact]
    public void Renders_aligned_lines_and_total()
    {
        var items = new[]
        {
            new ReceiptItem("Coffee", 2, 3.50m),
            new ReceiptItem("Extra large blueberry muffin", 1, 4.25m),
            new ReceiptItem("Water", 12, 0.99m),
        };

        var expected = string.Join("\n",
            "Coffee                2      7.00",
            "Extra large blueb...  1      4.25",
            "Water                12     11.88",
            "---------------------------------",
            "TOTAL                       23.13");

        Assert.Equal(expected, Receipt.Render(items));
    }

    [Fact]
    public void Empty_receipt_shows_zero_total()
    {
        var expected = string.Join("\n",
            "---------------------------------",
            "TOTAL                        0.00");

        Assert.Equal(expected, Receipt.Render(Array.Empty<ReceiptItem>()));
    }

    [Fact]
    public void Zero_quantity_throws() =>
        Assert.Throws<ArgumentException>(() => Receipt.Render(new[] { new ReceiptItem("Tea", 0, 1m) }));

    [Fact]
    public void Null_throws() =>
        Assert.Throws<ArgumentNullException>(() => Receipt.Render(null!));
}
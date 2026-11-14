namespace Gym.Day08.Twin;

public enum Direction { In, Out }

public record StockLine(string Location, Direction Direction, int Quantity);

public record StockTransfer(string Id, string Note, IReadOnlyList<StockLine> Lines);

public interface ITransferStore
{
    bool TryGet(string id, out StockTransfer? transfer);
    void Add(StockTransfer transfer);
    IReadOnlyList<StockTransfer> All();
}

public sealed class InMemoryTransferStore : ITransferStore
{
    private readonly Dictionary<string, StockTransfer> _byId = new();
    private readonly List<StockTransfer> _inOrder = new();

    public bool TryGet(string id, out StockTransfer? transfer) => _byId.TryGetValue(id, out transfer);

    public void Add(StockTransfer transfer)
    {
        _byId.Add(transfer.Id, transfer);
        _inOrder.Add(transfer);
    }

    public IReadOnlyList<StockTransfer> All() => _inOrder;
}

public sealed class StockJournal
{
    private readonly ITransferStore _store;

    public StockJournal(ITransferStore store) =>
        _store = store ?? throw new ArgumentNullException(nameof(store));

    public bool Record(StockTransfer transfer)
    {
        ArgumentNullException.ThrowIfNull(transfer);
        if (string.IsNullOrWhiteSpace(transfer.Id))
            throw new ArgumentException("Id is required.", nameof(transfer));
        if (transfer.Lines is null || transfer.Lines.Count < 2)
            throw new ArgumentException("A transfer needs at least 2 lines.", nameof(transfer));
        foreach (var line in transfer.Lines)
        {
            if (string.IsNullOrWhiteSpace(line.Location))
                throw new ArgumentException("Every line needs a location.", nameof(transfer));
            if (line.Quantity <= 0)
                throw new ArgumentException("Every quantity must be positive.", nameof(transfer));
        }
        if (!IsBalanced(transfer.Lines))
            throw new ArgumentException("Total In must equal total Out.", nameof(transfer));

        if (_store.TryGet(transfer.Id, out var existing))
        {
            if (SameContent(existing!, transfer)) return false;
            throw new InvalidOperationException($"Id '{transfer.Id}' was already used for a different transfer.");
        }

        _store.Add(transfer);
        return true;
    }

    public int Stock(string location)
    {
        int stock = 0;
        foreach (var transfer in _store.All())
            foreach (var line in transfer.Lines)
                if (line.Location == location)
                    stock += line.Direction == Direction.In ? line.Quantity : -line.Quantity;
        return stock;
    }

    private static bool IsBalanced(IReadOnlyList<StockLine> lines)
    {
        int ins = lines.Where(l => l.Direction == Direction.In).Sum(l => l.Quantity);
        int outs = lines.Where(l => l.Direction == Direction.Out).Sum(l => l.Quantity);
        return ins == outs;
    }

    private static bool SameContent(StockTransfer a, StockTransfer b) =>
        a.Note == b.Note && a.Lines.SequenceEqual(b.Lines);
}
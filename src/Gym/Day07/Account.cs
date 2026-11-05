using Gym.Day06;

namespace Gym.Day07;

public enum AccountStatus { Active, Frozen, Closed }

public sealed class InsufficientFundsException : InvalidOperationException
{
    public InsufficientFundsException(Money requested, Money available)
        : base($"Requested {requested}, but only {available} is available.")   // the message
    {
        Available = available;
    }
    public Money Available { get; }
}

public sealed class Account
{
    private readonly List<string> _history = new();
    public Account(string id, Currency currency, Money overdraftLimit)
    {
        if(string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Id is required.", nameof(id));
        if(overdraftLimit.Currency != currency) throw new ArgumentException("Overdraft Currency should be same as account currency", nameof(overdraftLimit.Currency));
        if(overdraftLimit.Amount < 0) throw new ArgumentException("Overdraft Amount should be greater than 0", nameof(overdraftLimit.Amount));
 
        Id = id;
        Currency = currency;
        OverdraftLimit = overdraftLimit;
        Balance = Money.FromMinor(0, currency);
        Status = AccountStatus.Active;
    }

    public string Id { get; }
    public Currency Currency { get; }
    public Money Balance { get; private set; }
    public Money OverdraftLimit { get; }
    public AccountStatus Status { get; private set; }
    public IReadOnlyList<string> History => _history;

    public void Deposit(Money amount)
    {
        EnsureNotClosed();
        EnsureValidAmount(amount);

        Balance = Balance + amount;                      // changes
        _history.Add($"Deposit {amount}");
    }
    public void Withdraw(Money amount)
    {
        EnsureNotClosed();
        EnsureNotFrozen();
        EnsureValidAmount(amount);

        var newBalance = Balance - amount;          // work it out, but don't save it yet
        if (newBalance.MinorUnits < -OverdraftLimit.MinorUnits)
        {
            var available = Money.FromMinor(Balance.MinorUnits + OverdraftLimit.MinorUnits, Currency);
            throw new InsufficientFundsException(amount, available);
        }

        Balance = newBalance;                            // all checks passed: now change
        _history.Add($"Withdraw {amount}");
        
    }
    public void Freeze()
    {
        if(Status != AccountStatus.Active) throw new InvalidOperationException($"Account {Id} is {Status}.");

        Status = AccountStatus.Frozen;
        _history.Add("Freeze");
    }
    public void Unfreeze()
    {
        if(Status != AccountStatus.Frozen) throw new InvalidOperationException($"Account {Id} is {Status}.");

        Status = AccountStatus.Active;
        _history.Add("Unfreeze");
    }
    public void Close()
    {
        if(Status == AccountStatus.Closed) throw new InvalidOperationException($"Account {Id} is already {Status}.");
        if(Balance.Amount != 0) throw new InvalidOperationException($"Account {Id} cannout be closed due to balance amount {Balance.Amount}");
        Status = AccountStatus.Closed;
        _history.Add("Unfreeze");
    }

    private void EnsureValidAmount(Money amount)         // shared by Deposit and Withdraw
    {
        if (amount.Currency != Currency)
            throw new CurrencyMismatchException(Currency, amount.Currency);
        if (amount.MinorUnits <= 0)
            throw new ArgumentException("Amount must be positive.", nameof(amount));
    }

    private void EnsureNotClosed()
    {
        if(Status == AccountStatus.Closed) throw new InvalidOperationException($"Account {Id} is closed.");
    }

    private void EnsureNotFrozen()
    {
        if(Status == AccountStatus.Frozen) throw new InvalidOperationException($"Account {Id} is Frozen.");
    }
}
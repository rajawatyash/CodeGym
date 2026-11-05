using Gym.Day06;
using Gym.Day07;

namespace Gym.Tests.Day07;

public class AccountTests
{
    private static Money Usd(decimal amount) => Money.Of(amount, Currency.USD);

    private static Account NewAccount() => new("acc_1", Currency.USD, Usd(50m));

    [Fact]
    public void New_account_is_active_and_empty()
    {
        var account = NewAccount();

        Assert.Equal(AccountStatus.Active, account.Status);
        Assert.Equal(Usd(0m), account.Balance);
        Assert.Empty(account.History);
    }

    [Fact]
    public void Deposits_and_withdraws_into_the_overdraft()
    {
        var account = NewAccount();

        account.Deposit(Usd(100m));
        account.Withdraw(Usd(120m));

        Assert.Equal(Usd(-20m), account.Balance);
        Assert.Equal(new[] { "Deposit USD 100.00", "Withdraw USD 120.00" }, account.History.ToArray());
    }

    [Fact]
    public void Cannot_go_past_the_overdraft_limit()
    {
        var account = NewAccount();
        account.Deposit(Usd(100m));
        account.Withdraw(Usd(120m));

        var ex = Assert.Throws<InsufficientFundsException>(() => account.Withdraw(Usd(31m)));

        Assert.Equal(Usd(30m), ex.Available);
        Assert.Equal(Usd(-20m), account.Balance);
        Assert.Equal(2, account.History.Count);
    }

    [Fact]
    public void Exactly_the_limit_is_allowed()
    {
        var account = NewAccount();
        account.Withdraw(Usd(50m));
        Assert.Equal(Usd(-50m), account.Balance);
    }

    [Fact]
    public void Rejects_bad_amounts()
    {
        var account = NewAccount();
        Assert.Throws<ArgumentException>(() => account.Deposit(Usd(0m)));
        Assert.Throws<ArgumentException>(() => account.Withdraw(Usd(-1m)));
        Assert.Throws<CurrencyMismatchException>(() => account.Deposit(Money.Of(1m, Currency.EUR)));
    }

    [Fact]
    public void Frozen_account_takes_deposits_but_not_withdrawals()
    {
        var account = NewAccount();
        account.Freeze();

        account.Deposit(Usd(10m));
        Assert.Throws<InvalidOperationException>(() => account.Withdraw(Usd(1m)));

        account.Unfreeze();
        account.Withdraw(Usd(1m));
        Assert.Equal(Usd(9m), account.Balance);
        Assert.Equal(new[] { "Freeze", "Deposit USD 10.00", "Unfreeze", "Withdraw USD 1.00" }, account.History.ToArray());
    }

    [Fact]
    public void Status_changes_must_make_sense()
    {
        var account = NewAccount();
        Assert.Throws<InvalidOperationException>(() => account.Unfreeze());
        account.Freeze();
        Assert.Throws<InvalidOperationException>(() => account.Freeze());
    }

    [Fact]
    public void Close_needs_a_zero_balance_and_then_blocks_everything()
    {
        var account = NewAccount();
        account.Deposit(Usd(5m));
        Assert.Throws<InvalidOperationException>(() => account.Close());

        account.Withdraw(Usd(5m));
        account.Close();

        Assert.Equal(AccountStatus.Closed, account.Status);
        Assert.Throws<InvalidOperationException>(() => account.Deposit(Usd(1m)));
        Assert.Throws<InvalidOperationException>(() => account.Freeze());
    }

    [Fact]
    public void Constructor_checks_its_arguments()
    {
        Assert.Throws<ArgumentException>(() => new Account(" ", Currency.USD, Usd(0m)));
        Assert.Throws<ArgumentException>(() => new Account("a", Currency.USD, Usd(-1m)));
        Assert.Throws<ArgumentException>(() => new Account("a", Currency.USD, Money.Of(10m, Currency.EUR)));
    }
}
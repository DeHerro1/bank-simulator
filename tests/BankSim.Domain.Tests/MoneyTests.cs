using BankSim.Domain.ValueObjects;
using Xunit;

namespace BankSim.Domain.Tests;

public class MoneyTests
{
    [Fact]
    public void Money_TwoInstancesWithSameAmountAndCurrency_AreEqual()
    {
        var a = new Money(100m, "USD");
        var b = new Money(100m, "usd"); // currency is normalized to upper-invariant

        Assert.Equal(a, b);
        Assert.True(a == b);
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void Money_DifferentAmounts_AreNotEqual()
    {
        Assert.NotEqual(new Money(100m), new Money(101m));
    }

    [Fact]
    public void Addition_SameCurrency_SumsAmounts()
    {
        Assert.Equal(new Money(150m), new Money(100m) + new Money(50m));
    }

    [Fact]
    public void Addition_MismatchedCurrencies_Throws()
    {
        Assert.Throws<InvalidOperationException>(() => new Money(100m, "USD") + new Money(50m, "EUR"));
    }

    [Theory]
    [InlineData(100, 50, true)]
    [InlineData(50, 100, false)]
    [InlineData(50, 50, false)]
    public void GreaterThan_ComparesAmounts(decimal left, decimal right, bool expected)
    {
        Assert.Equal(expected, new Money(left) > new Money(right));
    }
}

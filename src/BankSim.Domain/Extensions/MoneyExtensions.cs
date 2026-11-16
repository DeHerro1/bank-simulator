using System.Globalization;
using BankSim.Domain.ValueObjects;

namespace BankSim.Domain.Extensions;

public static class MoneyExtensions
{
    public static string ToCurrencyString(this Money money) =>
        money.Currency switch
        {
            "USD" => money.Amount.ToString("C", CultureInfo.GetCultureInfo("en-US")),
            "EUR" => money.Amount.ToString("C", CultureInfo.GetCultureInfo("de-DE")),
            "GBP" => money.Amount.ToString("C", CultureInfo.GetCultureInfo("en-GB")),
            _ => $"{money.Amount:0.00} {money.Currency}"
        };
}

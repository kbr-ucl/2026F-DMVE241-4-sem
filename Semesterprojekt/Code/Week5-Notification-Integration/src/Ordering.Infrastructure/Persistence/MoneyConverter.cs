using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Ordering.Domain.ValueObjects;

namespace Ordering.Infrastructure.Persistence;

public class MoneyConverter : ValueConverter<Money, decimal>
{
    public MoneyConverter() : base(
        v => v.Amount,
        v => Money.FromDecimalUnsafe(v))
    {
    }
}

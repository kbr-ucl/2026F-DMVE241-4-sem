using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using EventCatalog.Domain.ValueObjects;

namespace EventCatalog.Infrastructure.Persistence;

public class MoneyConverter : ValueConverter<Money, decimal>
{
    public MoneyConverter() : base(
        v => v.Amount,
        v => Money.FromDecimalUnsafe(v))
    {
    }
}

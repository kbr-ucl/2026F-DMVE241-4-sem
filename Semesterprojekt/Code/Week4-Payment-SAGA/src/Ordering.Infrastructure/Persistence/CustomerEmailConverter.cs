using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Ordering.Domain.ValueObjects;

namespace Ordering.Infrastructure.Persistence;

public class CustomerEmailConverter : ValueConverter<CustomerEmail, string>
{
    public CustomerEmailConverter() : base(
        v => v.Value,
        v => CustomerEmail.FromUnsafe(v))
    {
    }
}

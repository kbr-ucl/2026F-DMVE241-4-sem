namespace Ordering.Domain.ValueObjects;

/// <summary>
/// Value object representing a customer email address.
/// </summary>
public record CustomerEmail
{
    public string Value { get; init; }

    public static CustomerEmail From(string email) => new(email);

    // EF Core value converter support - skips validation for persistence
    public static CustomerEmail FromUnsafe(string email) => new(email, skipValidation: true);

    private CustomerEmail(string email, bool skipValidation = false)
    {
        if (!skipValidation)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new DomainException("Email is required.");
            if (!email.Contains('@'))
                throw new DomainException("Invalid email format.");
        }
        Value = email;
    }

    public override string ToString() => Value;
}

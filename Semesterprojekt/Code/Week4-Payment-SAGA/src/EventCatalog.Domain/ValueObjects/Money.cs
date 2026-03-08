namespace EventCatalog.Domain.ValueObjects;

/// <summary>
/// Value object representing money. Must be a positive number.
/// </summary>
public record Money
{
    public decimal Amount { get; init; }

    public static Money FromDecimal(decimal amount) => new(amount);
    
    // EF Core value converter support - allows zero/negative for persistence
    public static Money FromDecimalUnsafe(decimal amount) => new(amount, skipValidation: true);
    
    private Money(decimal amount, bool skipValidation = false)
    {
        if (!skipValidation && amount <= 0)
            throw new DomainException("Money amount must be positive.");
        Amount = amount;
    }

    public static Money operator +(Money left, Money right) => new(left.Amount + right.Amount);
    public static Money operator -(Money left, Money right)
    {
        var result = left.Amount - right.Amount;
        if (result <= 0)
            throw new DomainException("Money subtraction result must be positive.");
        return new(result);
    }

    public static Money operator *(Money money, int multiplier)
    {
        if (multiplier <= 0)
            throw new DomainException("Multiplier must be positive.");
        return new(money.Amount * multiplier);
    }

    public static Money operator *(int multiplier, Money money) => money * multiplier;

    public override string ToString() => Amount.ToString("F2");
}

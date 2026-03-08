namespace Payment.Facade.DTOs;
public record ProcessPaymentRequest(Guid OrderId, decimal Amount, string CustomerEmail);
public record PaymentResponse(bool Success, string? TransactionId, string? ErrorMessage);

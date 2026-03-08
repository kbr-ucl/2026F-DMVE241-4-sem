namespace Ordering.UseCases.Ports;
public interface IPaymentService { Task<PaymentResult> ProcessPaymentAsync(Guid orderId, decimal amount, string email); }
public record PaymentResult(bool Success, string? TransactionId, string? ErrorMessage);

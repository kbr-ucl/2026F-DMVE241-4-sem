using Payment.Facade.DTOs;
namespace Payment.Facade.Interfaces;
/// <summary>Undtagelse fra CQS: returnerer resultat, da SAGA kræver det.</summary>
public interface IProcessPaymentUseCase { Task<PaymentResponse> Execute(ProcessPaymentRequest request); }

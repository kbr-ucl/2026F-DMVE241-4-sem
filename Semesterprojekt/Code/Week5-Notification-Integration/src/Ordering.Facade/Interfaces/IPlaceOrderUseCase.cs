using Ordering.Facade.DTOs;
namespace Ordering.Facade.Interfaces;
public interface IPlaceOrderUseCase { Task Execute(PlaceOrderRequest request); }
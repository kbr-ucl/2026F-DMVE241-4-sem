using Inventory.Facade.DTOs;
namespace Inventory.Facade.Interfaces;
public interface IReserveTicketsUseCase { Task Execute(ReserveTicketsRequest request); }
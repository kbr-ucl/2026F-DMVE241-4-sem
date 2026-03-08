using Inventory.Facade.DTOs;
namespace Inventory.Facade.Interfaces;
public interface IReleaseTicketsUseCase { Task Execute(ReleaseTicketsRequest request); }
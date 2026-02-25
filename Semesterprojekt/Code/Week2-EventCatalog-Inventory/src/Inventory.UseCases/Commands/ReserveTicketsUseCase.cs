using Inventory.Facade.DTOs;
using Inventory.Facade.Interfaces;
using Inventory.UseCases.Repositories;
namespace Inventory.UseCases.Commands;

public class ReserveTicketsUseCase : IReserveTicketsUseCase
{
    private readonly ITicketStockRepository _repo;
    public ReserveTicketsUseCase(ITicketStockRepository repo) => _repo = repo;

    public async Task Execute(ReserveTicketsRequest request)
    {
        foreach (var line in request.Lines)
        {
            var stock = await _repo.GetByCategoryIdAsync(line.CategoryId)
                ?? throw new InvalidOperationException($"Stock not found for {line.CategoryId}.");
            stock.Reserve(line.Quantity);
        }
        await _repo.SaveAsync();
    }
}

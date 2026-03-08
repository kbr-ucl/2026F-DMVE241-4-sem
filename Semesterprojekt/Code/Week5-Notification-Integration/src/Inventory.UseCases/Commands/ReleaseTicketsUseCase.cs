using Inventory.Facade.DTOs;
using Inventory.Facade.Interfaces;
using Inventory.UseCases.Repositories;
namespace Inventory.UseCases.Commands;

public class ReleaseTicketsUseCase : IReleaseTicketsUseCase
{
    private readonly ITicketStockRepository _repo;
    public ReleaseTicketsUseCase(ITicketStockRepository repo) => _repo = repo;

    public async Task Execute(ReleaseTicketsRequest request)
    {
        foreach (var line in request.Lines)
        {
            var stock = await _repo.GetByCategoryIdAsync(line.CategoryId)
                ?? throw new InvalidOperationException($"Stock not found for {line.CategoryId}.");
            stock.Release(line.Quantity);
        }
        await _repo.SaveAsync();
    }
}

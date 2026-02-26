using Ordering.Domain.ValueObjects;

namespace Ordering.UseCases.Ports;
public interface IEventCatalogService { Task<CategoryPriceInfo?> GetCategoryPriceAsync(Guid eventId, Guid categoryId); }
public record CategoryPriceInfo(Guid CategoryId, string CategoryName, Money Price);

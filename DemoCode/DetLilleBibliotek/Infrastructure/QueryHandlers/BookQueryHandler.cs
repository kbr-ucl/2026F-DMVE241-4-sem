using Facade.Queries;

namespace Infrastructure.QueryHandlers
{
    public class BookQueryHandler : IBookQueries
    {
        BookDto IBookQueries.GetMedlem(int guid)
        {
            throw new NotImplementedException();
        }

        IEnumerable<BookDto> IBookQueries.GetMedlemmer()
        {
            throw new NotImplementedException();
        }
    }
}

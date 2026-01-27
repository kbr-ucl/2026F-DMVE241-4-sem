using Facade.Queries;
using Infrastructure.Database;

namespace Infrastructure.QueryHandlers
{
    public class BookQueryHandler : IBookQueries
    {
        private readonly BibliotekContext _db;

        public BookQueryHandler(BibliotekContext db)
        {
            _db = db;
        }
        BookDto IBookQueries.GetBook(Guid id)
        {
            throw new NotImplementedException();
        }

        IEnumerable<BookDto> IBookQueries.GetBooks()
        {
            return _db.Bøger.Select(b=> new BookDto(b.Id, b.Isbn, b.Titel, b.Forfatter, !b.ErUdlånt)).ToList();
        }
    }
}

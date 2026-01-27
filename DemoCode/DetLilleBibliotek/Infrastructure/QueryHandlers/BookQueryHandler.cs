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
            var bog = _db.Bøger.Find(id);
            if (bog == null) return null;

                return new BookDto(bog.Id, bog.Isbn, bog.Titel, bog.Forfatter, !bog.ErUdlånt);
        }

        IEnumerable<BookDto> IBookQueries.GetBooks()
        {
            return _db.Bøger.Select(b=> new BookDto(b.Id, b.Isbn, b.Titel, b.Forfatter, !b.ErUdlånt)).ToList();
        }
    }
}

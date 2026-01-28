using Facade.Queries;
using Infrastructure.Database;

namespace Infrastructure.QueryHandlers
{
    public class BookQueryHandler : IBogQueries
    {
        private readonly BibliotekContext _db;

        public BookQueryHandler(BibliotekContext db)
        {
            _db = db;
        }
        BogDto IBogQueries.HentBog(string isbn)
        {
            var bog = _db.Bøger.Find(isbn);
            if (bog == null) return null;

                return new BogDto(bog.Isbn, bog.Titel, bog.Forfatter, !bog.ErUdlånt);
        }

        IEnumerable<BogDto> IBogQueries.HentBøger()
        {
            return _db.Bøger.Select(b=> new BogDto(b.Isbn, b.Titel, b.Forfatter, !b.ErUdlånt)).ToList();
        }
    }
}

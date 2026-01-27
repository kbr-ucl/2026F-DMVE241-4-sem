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
        BogDto IBogQueries.GetBog(Guid id)
        {
            var bog = _db.Bøger.Find(id);
            if (bog == null) return null;

                return new BogDto(bog.Id, bog.Isbn, bog.Titel, bog.Forfatter, !bog.ErUdlånt);
        }

        IEnumerable<BogDto> IBogQueries.GetBøger()
        {
            return _db.Bøger.Select(b=> new BogDto(b.Id, b.Isbn, b.Titel, b.Forfatter, !b.ErUdlånt)).ToList();
        }
    }
}

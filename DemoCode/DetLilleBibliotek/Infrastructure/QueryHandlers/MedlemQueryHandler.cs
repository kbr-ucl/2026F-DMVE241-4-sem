using Facade.Queries;
using Infrastructure.Database;

namespace Infrastructure.QueryHandlers
{
    public class MedlemQueryHandler : IMedlemQueries
    {
        private BibliotekContext _db;

        public MedlemQueryHandler(BibliotekContext db)
        {
            _db = db;
        }
        MedlemDto IMedlemQueries.HentMedlem(string isbn)
        {
            var medlem = _db.Medlemmer.Find(isbn);
            if (medlem == null) return null;

            return new MedlemDto(medlem.Medlemsnummer, medlem.Navn);
        }

        IEnumerable<MedlemDto> IMedlemQueries.HentMedlemmer()
        {
            return _db.Medlemmer.Select(m => new MedlemDto(m.Medlemsnummer, m.Navn)).ToList();
        }
    }
}

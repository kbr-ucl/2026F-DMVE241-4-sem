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
        MedlemDto IMedlemQueries.GetMedlem(Guid id)
        {
            var medlem = _db.Medlemmer.Find(id);
            if (medlem == null) return null;

            return new MedlemDto(medlem.Id, medlem.Navn);
        }

        IEnumerable<MedlemDto> IMedlemQueries.GetMedlemmer()
        {
            return _db.Medlemmer.Select(m => new MedlemDto(m.Id, m.Navn)).ToList();
        }
    }
}

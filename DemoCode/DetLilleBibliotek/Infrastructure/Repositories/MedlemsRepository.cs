using Application.InfrastructureFacade;
using Domain.Entities;
using Infrastructure.Database;

namespace Infrastructure.Repositories
{
    public class MedlemsRepository : IMedlemsRepository
    {
        private readonly BibliotekContext _db;

        public MedlemsRepository(BibliotekContext db)
        {
            _db = db;
        }
        void IMedlemsRepository.Opret(Medlem medlem)
        {
            _db.Medlemmer.Add(medlem);
            _db.SaveChanges();
        }

        Medlem? IMedlemsRepository.Hent(int medlemsNummer)
        {
            return _db.Medlemmer.Find(medlemsNummer);
        }

        void IMedlemsRepository.Opdater(Medlem medlem)
        {
            _db.SaveChanges();
        }
    }
}

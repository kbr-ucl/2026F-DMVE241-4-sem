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
        void IMedlemsRepository.Gem(Medlem medlem)
        {
            _db.SaveChanges();
        }
        Medlem IMedlemsRepository.HentPåId(Guid id)
        {
            return _db.Medlemmer.Find(id);
        }
    }
}

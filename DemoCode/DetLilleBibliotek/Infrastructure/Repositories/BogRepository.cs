using Application.InfrastructureFacade;
using Domain.Entities;
using Infrastructure.Database;

namespace Infrastructure.Repositories
{
    public class BogRepository : IBogRepository
    {
        private readonly BibliotekContext _db;
        
        public BogRepository(BibliotekContext db)
        {
            _db = db;
        }

        void IBogRepository.Opret(Bog bog)
        {
            _db.Bøger.Add(bog);
            _db.SaveChanges();
        }

        Bog IBogRepository.Hent(string isbn)
        {
            return _db.Bøger.Find(isbn);
        }
    }
}

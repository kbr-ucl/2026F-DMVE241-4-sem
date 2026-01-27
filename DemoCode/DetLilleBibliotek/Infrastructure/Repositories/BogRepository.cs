using Application.InfrastructureFacade;
using Domain.Entities;

namespace Infrastructure.Repositories
{
    public class BogRepository : IBogRepository
    {
        void IBogRepository.Gem(Bog bog)
        {
            throw new NotImplementedException();
        }

        Bog IBogRepository.HentPåId(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}

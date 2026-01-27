using Application.InfrastructureFacade;
using Domain.Entities;

namespace Infrastructure.Repositories
{
    public class MedlemsRepository : IMedlemsRepository
    {
        void IMedlemsRepository.Gem(Medlem medlem)
        {
            throw new NotImplementedException();
        }
        Medlem IMedlemsRepository.HentPåId(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}

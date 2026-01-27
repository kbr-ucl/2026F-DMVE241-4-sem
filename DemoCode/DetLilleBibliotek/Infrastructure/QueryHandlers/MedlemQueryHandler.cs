using Facade.Queries;

namespace Infrastructure.QueryHandlers
{
    public class MedlemQueryHandler : IMedlemQueries
    {
        MedlemDto IMedlemQueries.GetMedlem(Guid id)
        {
            throw new NotImplementedException();
        }

        IEnumerable<MedlemDto> IMedlemQueries.GetMedlemmer()
        {
            throw new NotImplementedException();
        }
    }
}

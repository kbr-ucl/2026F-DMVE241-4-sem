namespace Facade.Queries
{
    public interface IMedlemQueries
    {
        MedlemDto GetMedlem(int guid);
        IEnumerable<MedlemDto> GetMedlemmer();
    }

    public record MedlemDto(Guid MedlemsId, string Navn);
}

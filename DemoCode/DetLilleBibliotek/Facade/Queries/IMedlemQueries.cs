namespace Facade.Queries
{
    public interface IMedlemQueries
    {
        MedlemDto GetMedlem(Guid id);
        IEnumerable<MedlemDto> GetMedlemmer();
    }

    public record MedlemDto(Guid MedlemsId, string Navn);
}

namespace Facade.Queries
{
    public interface IMedlemQueries
    {
        MedlemDto HentMedlem(string isbn);
        IEnumerable<MedlemDto> HentMedlemmer();
    }

    public record MedlemDto(int Medlemsnummer, string Navn);
}

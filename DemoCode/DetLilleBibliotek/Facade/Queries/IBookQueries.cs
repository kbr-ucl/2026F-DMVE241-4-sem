namespace Facade.Queries
{
    public interface IBogQueries
    {
        BogDto HentBog(string isbn);
        IEnumerable<BogDto> HentBøger();
    }

    public record BogDto(string Isbn, string Titel, string Forfatter, bool ErHjemme);
}

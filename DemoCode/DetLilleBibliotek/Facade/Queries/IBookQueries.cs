namespace Facade.Queries
{
    public interface IBogQueries
    {
        BogDto GetBog(Guid id);
        IEnumerable<BogDto> GetBøger();
    }

    public record BogDto(Guid BogId, string Isbn, string Titel, string Forfatter, bool ErHjemme);
}

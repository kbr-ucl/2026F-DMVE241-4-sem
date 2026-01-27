namespace Facade.Queries
{
    public interface IBookQueries
    {
        BookDto GetMedlem(int guid);
        IEnumerable<BookDto> GetMedlemmer();
    }

    public record BookDto(Guid BogId, string Isbn, string Titel, string Forfatter, bool ErHjemme);
}

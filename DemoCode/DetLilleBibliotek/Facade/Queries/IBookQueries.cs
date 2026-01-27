namespace Facade.Queries
{
    public interface IBookQueries
    {
        BookDto GetBook(Guid id);
        IEnumerable<BookDto> GetBooks();
    }

    public record BookDto(Guid BogId, string Isbn, string Titel, string Forfatter, bool ErHjemme);
}

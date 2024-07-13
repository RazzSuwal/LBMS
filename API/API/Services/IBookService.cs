using API.Entities;

namespace API.Services
{
    public interface IBookService
    {
        Task<IEnumerable<Books>> GetBooksAsync(string searchTerm = null, int pageNumber = 1, int pageSize = 10);
        Task<Book> GetBookByIdAsync(int id);
        Task<Book> UpdateBookByIdAsync(Book books);
    }


}

using System.Data;
using API.Entities;
using Dapper;
using Microsoft.Data.SqlClient;

namespace API.Services
{
    public class BookService : IBookService
    {
        private IConfiguration _configuration;

        public BookService(IConfiguration configuration)
        {
            _configuration = configuration;
            ConnectionString = _configuration.GetConnectionString("DB");
            providerName = "System.Data.SqlClient";
        }

        public string ConnectionString { get; }
        public string providerName { get; }
        public IDbConnection connection
        {
            get { return new SqlConnection(ConnectionString); }
        }

        public async Task<IEnumerable<Books>> GetBooksAsync(string searchTerm = null, int pageNumber = 1, int pageSize = 10)
        {
            List<Books> books = new List<Books>();
            try
            {
                using (IDbConnection dbConnection = connection)
                {
                    var parameters = new
                    {
                        SearchTerm = searchTerm,
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    };

                    books = (await dbConnection.QueryAsync<Books>("GetBooksAndCategories",
                                                                 parameters,
                                                                 commandType: CommandType.StoredProcedure))
                                                                 .ToList();
                }
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message;
            }

            return books;
        }

        public async Task<Book> GetBookByIdAsync(int id)
        {
            Book book = null;
            try
            {
                using (IDbConnection dbConnection = connection)
                {
                    string sqlQuery = @"
                    SELECT 
                        * 
                    FROM Books 
                    WHERE Id = @BookId";

                    var parameters = new { BookId = id };

                    book = await dbConnection.QueryFirstOrDefaultAsync<Book>(sqlQuery, parameters);
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                string errorMessage = ex.Message;
            }

            return book;
        }
        public async Task<Book> UpdateBookByIdAsync(Book bookToUpdate)
        {
            Book updatedBook = null;
            try
            {
                using (IDbConnection dbConnection = connection) // 'connection' is assumed to be your IDbConnection instance
                {
                    string sqlQuery = @"
                UPDATE Books
                SET 
                    Title = @Title,
                    Author = @Author,
                    Price = @Price,
                    Ordered = @Ordered,
                    BookCategoryId = @BookCategoryId
                WHERE 
                    Id = @Id;
                ";

                    // Use anonymous type to pass parameters
                    var parameters = new
                    {
                        Id = bookToUpdate.Id,
                        Title = bookToUpdate.Title,
                        Author = bookToUpdate.Author,
                        Price = bookToUpdate.Price,
                        Ordered = bookToUpdate.Ordered,
                        BookCategoryId = bookToUpdate.BookCategoryId
                    };

                    // Execute the update query and get the updated book
                    int rowsAffected = await dbConnection.ExecuteAsync(sqlQuery, parameters);

                    if (rowsAffected > 0)
                    {
                        // If rows were affected, fetch the updated book from the database
                        sqlQuery = "SELECT * FROM Books WHERE Id = @Id;";
                        updatedBook = await dbConnection.QueryFirstOrDefaultAsync<Book>(sqlQuery, new { Id = bookToUpdate.Id });
                    }
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                string errorMessage = ex.Message;
            }

            return updatedBook;
        }

    }
}

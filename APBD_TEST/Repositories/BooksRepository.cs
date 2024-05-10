using APBD_TEST.Models.DTOs;
using Microsoft.Data.SqlClient;
namespace APBD_TEST.Repositories;

public class BooksRepository : IBooksRepository
{
    
    private readonly IConfiguration _configuration;
    public BooksRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public async Task<BookDTO> GetBook(int id)
    {
        var query =
            "SELECT b.PK as IdBook, b.Title, a.First_Name, a.Last_Name, g.Name FROM Books b " +
            "JOIN Books_Authors ba ON b.PK = ba.FK_BOOK " +
            "JOIN Authors a ON ba.FK_AUTHOR = a.PK " +
            "JOIN Books_Genres bg ON b.PK = bg.FK_BOOK " +
            "JOIN Genres g ON bg.FK_Genre = g.PK " +
            "WHERE b.PK = @ID";
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();
        
        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", id);
        
        await connection.OpenAsync();

        var reader = await command.ExecuteReaderAsync();
        
        var BookIdOrdinal = reader.GetOrdinal("IdBook");
        var TitleOrdinal = reader.GetOrdinal("Title");
        var FirstNameOrdinal = reader.GetOrdinal("First_Name");
        var LastNameOrdinal = reader.GetOrdinal("Last_Name");
        var GenreOrdinal = reader.GetOrdinal("Name");
        
        BookDTO book = null;
        
        while (await reader.ReadAsync())
        {
            if (book is not null)
            {
                book.Authors.Add(new AuthorDTO
                {
                    firstName = reader.GetString(FirstNameOrdinal),
                    lastName = reader.GetString(LastNameOrdinal)
                });
                book.Genres.Add(reader.GetString(GenreOrdinal));
            }
            else
            {
                book = new BookDTO()
                {
                    Id = reader.GetInt32(BookIdOrdinal),
                    Title = reader.GetString(TitleOrdinal),
                    Authors = new List<AuthorDTO>
                    {
                        new AuthorDTO
                        {
                            firstName = reader.GetString(FirstNameOrdinal),
                            lastName = reader.GetString(LastNameOrdinal)
                        }
                    },
                    Genres = new List<string>
                    {
                        reader.GetString(GenreOrdinal)
                    }
                };
            }
        }
        if(book is null) throw new Exception();
        return book;
    }
    
    public async Task<bool> DoesBookExist(int id)
    {
        var query = "SELECT 1 FROM Books WHERE PK = @ID";
        
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", id);

        await connection.OpenAsync();

        var res = await command.ExecuteScalarAsync();
        
        return res is not null;
    }

    public async Task<bool> DoesGenreExist(int id)
    {
        var query = "SELECT 1 FROM Genres WHERE PK = @ID";
        
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", id);

        await connection.OpenAsync();

        var res = await command.ExecuteScalarAsync();
        
        return res is not null;
    }
    

    public async Task DeleteGenre(int id)
    {
        
        //our query should delete the genre along with references in Books_Genres
        var query = "DELETE FROM child " +
                    "FROM books_genres AS child " +
                    "INNER JOIN genres AS parent ON child.FK_Genre = parent.PK " +
                    "WHERE PK =@ID; " +
                    "DELETE FROM Genres WHERE PK = @ID";
        
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", id);
        
        await connection.OpenAsync();
        
        await command.ExecuteNonQueryAsync();
    }
}
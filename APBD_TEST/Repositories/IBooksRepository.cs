using APBD_TEST.Models.DTOs;

namespace APBD_TEST.Repositories;

public interface IBooksRepository
{
    public Task<BookDTO> GetBook(int id);
    Task<bool> DoesBookExist(int id);
    Task<bool> DoesGenreExist(int id);
    Task DeleteGenre(int id);
}
using System.Net;
using APBD_TEST.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace APBD_TEST.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BooksController : ControllerBase
{
    private readonly IBooksRepository _booksRepository;
    public BooksController(IBooksRepository booksRepository)
    {
        _booksRepository = booksRepository;
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetBook(int id)
    {
        if (!await _booksRepository.DoesBookExist(id))
            return NotFound($"Book with given ID - {id} doesn't exist");

        var book = await _booksRepository.GetBook(id);
        
        return Ok(book);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGenre(int id)
    {
        if (!await _booksRepository.DoesGenreExist(id))
            return NotFound($"Genre with given ID - {id} doesn't exist");

        await _booksRepository.DeleteGenre(id);

        return NoContent();
    }
}
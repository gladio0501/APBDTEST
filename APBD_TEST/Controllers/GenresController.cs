using Microsoft.AspNetCore.Mvc;
using APBD_TEST.Repositories;
namespace APBD_TEST.Controllers;

[Route("api/[controller]")]
[ApiController]

public class GenresController : ControllerBase

{
    private readonly IBooksRepository _booksRepository;
    public GenresController(IBooksRepository booksRepository)
    {
        _booksRepository = booksRepository;
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
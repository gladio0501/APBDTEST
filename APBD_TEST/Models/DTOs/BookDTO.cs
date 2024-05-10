namespace APBD_TEST.Models.DTOs;

public class BookDTO
{
    public int Id { get; set; }
    public String Title { get; set; }= string.Empty;
    public List<AuthorDTO> Authors { get; set; } = null!;
    public List<String> Genres { get; set; } = null!;
}


public class AuthorDTO
{
    public String firstName { get; set; } = string.Empty;
    public String lastName { get; set; } = string.Empty;  
}
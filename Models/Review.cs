using BookZoneAPI.Models;

public class Review
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public int BookId { get; set; }
    public Book Book { get; set; }
    public int Rating { get; set; }  // Note 1 à 5
    public string Comment { get; set; }
    public DateTime DatePosted { get; set; } = DateTime.UtcNow;
}

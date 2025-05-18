using BookZoneAPI.Models;
using System.ComponentModel.DataAnnotations;

public class Book
{
    public int Id { get; set; }

    [Required]
    public string Title { get; set; }

    [Required]
    public int AuthorId { get; set; }

    public Author Author { get; set; }  

    [Required]
    public int CategoryId { get; set; }

    public Category Category { get; set; } 

    [Required]
    public int Stock { get; set; }

    [Required]
    public double Price { get; set; }
}

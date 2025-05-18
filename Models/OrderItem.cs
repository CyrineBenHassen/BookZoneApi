using BookZoneAPI.Models;

public class OrderItem  //Article de Commande
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public Book Book { get; set; }
    public int Quantity { get; set; }
    public int OrderId { get; set; }
}
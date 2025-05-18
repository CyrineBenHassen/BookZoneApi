using BookZoneAPI.Models;

public class CartItem  //Élément de Panier
{
    public int Id { get; set; }
    public string UserId { get; set; }  // Utilisateur propriétaire
    public int BookId { get; set; }
    public Book Book { get; set; }
    public int Quantity { get; set; }
}

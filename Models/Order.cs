public class Order  //command
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public DateTime Date { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.EnAttente;
    public List<OrderItem> Items { get; set; } = new List<OrderItem>();


}

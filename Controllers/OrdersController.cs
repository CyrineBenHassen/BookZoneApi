using BookZoneAPI.Data;
using BookZoneAPI.Models;
using BookZoneAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookZoneAPI.Controllers
{
    [Authorize(Roles = "Client")]
    [ApiController]
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailService _emailService;

        public OrdersController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            IEmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
        }

        [HttpPost("place")]
        public async Task<IActionResult> PlaceOrder()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Unauthorized();

            var cartItems = await _context.CartItems
                .Include(c => c.Book)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            if (!cartItems.Any())
                return BadRequest("Panier vide");

            foreach (var item in cartItems)
            {
                if (item.Quantity > item.Book.Stock)
                    return BadRequest($"Stock insuffisant pour le livre '{item.Book.Title}'. Disponible : {item.Book.Stock}");
            }

            var order = new Order
            {
                UserId = userId,
                Date = DateTime.UtcNow,
                Status = OrderStatus.EnAttente,
                Items = cartItems.Select(c => new OrderItem
                {
                    BookId = c.BookId,
                    Quantity = c.Quantity
                }).ToList()
            };

            _context.Orders.Add(order);

            foreach (var item in cartItems)
            {
                item.Book.Stock -= item.Quantity;
                _context.Books.Update(item.Book);
            }

            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            string subject = "Confirmation de votre commande BookZone";
            string body = $"Bonjour {user.UserName},<br/><br/>" +
                          $"Votre commande #{order.Id} a bien été enregistrée le {order.Date:dd/MM/yyyy}.<br/>" +
                          $"Merci pour votre confiance et à bientôt sur BookZone !";

            await _emailService.SendEmailAsync(user.Email, subject, body);

            return Ok(order);
        }
    

    // GET api/orders/{orderId}
// Récupérer la commande par id (avec son statut)
[HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderById(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
                return NotFound();

            return Ok(order); // order contient aussi la propriété Status
        }

        // PUT api/orders/{orderId}/cancel
        // Annuler la commande (changer statut à Annulee)
        [HttpPut("{orderId}/cancel")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
                return NotFound();

            if (order.Status == OrderStatus.Livree)
                return BadRequest("Impossible d'annuler une commande déjà livrée.");

            order.Status = OrderStatus.Annulee;
            await _context.SaveChangesAsync();

            return Ok(order);
        }

        // PUT api/orders/{orderId}/status
        // Mettre à jour le statut (exemple : passer à EnCours, Expediee, Livree)
        [HttpPut("{orderId}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromBody] OrderStatus newStatus)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
                return NotFound();

            order.Status = newStatus;
            await _context.SaveChangesAsync();

            return Ok(order);
        }

    }
}

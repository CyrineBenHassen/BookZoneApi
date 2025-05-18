using BookZoneAPI.Data;
using BookZoneAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookZoneAPI.Controllers
{
    [Authorize(Roles = "Client")]
    [ApiController]
    [Route("api/cart")]
    public class CartController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ Ajouter un livre au panier
        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] CartItem cartItem)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // ✅ Utilisation du GUID
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            cartItem.UserId = userId;
            cartItem.Book = null; // Éviter les erreurs d'insertion liées aux entités liées

            var existingItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.UserId == userId && c.BookId == cartItem.BookId);

            if (existingItem != null)
            {
                existingItem.Quantity += cartItem.Quantity;
            }
            else
            {
                _context.CartItems.Add(cartItem);
            }

            await _context.SaveChangesAsync();
            return Ok(cartItem);
        }

        // ✅ Voir le panier
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // ✅ Consistent avec Add et PlaceOrder
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var cart = await _context.CartItems
                .Include(c => c.Book)
                    .ThenInclude(b => b.Author)
                .Include(c => c.Book)
                    .ThenInclude(b => b.Category)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            return Ok(cart);
        }

        // ✅ Supprimer un élément
        [HttpDelete("remove/{id}")]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // ✅
            var item = await _context.CartItems.FindAsync(id);

            if (item == null || item.UserId != userId)
                return NotFound();

            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

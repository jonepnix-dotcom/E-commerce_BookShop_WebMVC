using Microsoft.EntityFrameworkCore;
using TheLight_JoneBookShop_WebMVC.Data;

namespace TheLight_JoneBookShop_WebMVC.helper
{
    public class SessionMiddleware
    {
        private readonly RequestDelegate _next;
        public SessionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var session = context.Session;

            if (string.IsNullOrEmpty(session.GetString("SessionId")))
            {
                session.SetString("SessionId", Guid.NewGuid().ToString());
            }
            await CleanupExpiredAsync(context.RequestServices.GetRequiredService<DbjonebookshopContext>());
            await _next(context);
        }
        private async Task CleanupExpiredAsync(DbjonebookshopContext dbContext)
        {
            var expiredCarts = await dbContext.Shopcarts
                .Where(cart => cart.ExpirationDate <= DateTime.UtcNow && cart.Iduser == 1)
                .ToListAsync();
            var expiredEmail = await dbContext.EmailVerifications
                .Where(email => email.ExpiresAt <= DateTime.UtcNow)
                .ToListAsync();
            if (expiredCarts.Any())
            {
                dbContext.Shopcarts.RemoveRange(expiredCarts);
                await dbContext.SaveChangesAsync();
            }
            if (expiredEmail.Any())
            {
                dbContext.EmailVerifications.RemoveRange(expiredEmail);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}

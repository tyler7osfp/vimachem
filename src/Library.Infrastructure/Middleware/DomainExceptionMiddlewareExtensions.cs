using Library.Domain.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace Library.Infrastructure.Middleware
{
    public static class DomainExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseDomainExceptionHandler(this IApplicationBuilder app)
        {
            return app.UseExceptionHandler(err => err.Run(async ctx =>
            {
                ctx.Response.StatusCode = 400;
                var ex = ctx.Features.Get<IExceptionHandlerFeature>()?.Error;
                if (ex is DomainException)
                    await ctx.Response.WriteAsJsonAsync(new { error = ex.Message });
                else
                {
                    ctx.Response.StatusCode = 500;
                    await ctx.Response.WriteAsJsonAsync(new { error = "Internal error." });
                }
            }));
        }
    }
}

using Library.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IBookQueryService, BookQueryService>();
            services.AddScoped<IBookCommandService, BookCommandService>();
            services.AddScoped<IBookInventoryService, BookInventoryService>();

            services.AddScoped<IBorrowingQueryService, BorrowingQueryService>();
            services.AddScoped<IBorrowingCommandService, BorrowingCommandService>();

            services.AddScoped<ICategoryQueryService, CategoryQueryService>();
            services.AddScoped<ICategoryCommandService, CategoryCommandService>();

            services.AddScoped<IPartyQueryService, PartyQueryService>();
            services.AddScoped<IPartyCommandService, PartyCommandService>();
            services.AddScoped<IPartyRoleCommandService, PartyRoleCommandService>();
            return services;
        }
    }
}

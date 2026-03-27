using Library.Domain;
using Library.Domain.Interfaces;
using Library.Infrastructure.Caching;
using Microsoft.Extensions.Options;
using Library.Infrastructure.Events;
using Library.Infrastructure.Events.CacheInvalidators;
using Library.Infrastructure.Events.Publishers;
using Library.Infrastructure.Persistence;
using Library.Infrastructure.Persistence.Repos;
using MassTransit;
using Vimachem.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<CacheOptions>(config.GetSection(CacheOptions.SectionName));
            var postgresConnectionString = config.GetRequiredPostgresConnectionString();

            services.AddDbContext<LibraryDbContext>(opts =>
                opts.UseNpgsql(postgresConnectionString));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

            services.AddScoped<IBeforeCommitDomainEventHandler, BookEventPublisher>();
            services.AddScoped<IBeforeCommitDomainEventHandler, CategoryEventPublisher>();
            services.AddScoped<IBeforeCommitDomainEventHandler, PartyEventPublisher>();
            services.AddScoped<IBeforeCommitDomainEventHandler, BorrowingEventPublisher>();

            services.AddScoped<IAfterCommitDomainEventHandler, BookCacheInvalidator>();
            services.AddScoped<IAfterCommitDomainEventHandler, CategoryCacheInvalidator>();
            services.AddScoped<IAfterCommitDomainEventHandler, PartyCacheInvalidator>();
            services.AddScoped<IAfterCommitDomainEventHandler, BorrowingCacheInvalidator>();

            services.AddKeyedScoped<IBookRepository, BookRepository>(RepositoryKeys.Db);
            services.AddKeyedScoped<ICategoryRepository, CategoryRepository>(RepositoryKeys.Db);
            services.AddKeyedScoped<IPartyRepository, PartyRepository>(RepositoryKeys.Db);

            services.AddKeyedScoped<IBookRepository, CachingBookRepository>(RepositoryKeys.Cached);
            services.AddKeyedScoped<ICategoryRepository, CachingCategoryRepository>(RepositoryKeys.Cached);
            services.AddKeyedScoped<IPartyRepository, CachingPartyRepository>(RepositoryKeys.Cached);

            services.AddScoped<IBorrowingRepository, BorrowingRepository>();

            services.AddMassTransit(x =>
            {
                x.AddEntityFrameworkOutbox<LibraryDbContext>(o =>
                {
                    o.UsePostgres();
                    o.UseBusOutbox();
                    o.QueryDelay = TimeSpan.FromSeconds(1);
                    o.DuplicateDetectionWindow = TimeSpan.FromMinutes(10);
                });

                x.UsingRabbitMq((_, cfg) => cfg.ConfigureVimachemRabbitMqHost(config));
            });

            return services;
        }
    }
}

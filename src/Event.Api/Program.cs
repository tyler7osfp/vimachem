using EventService;
using EventService.Consumers;
using EventService.Repos;
using EventService.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using MassTransit;
using Vimachem.Messaging;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<PagedListQueryValidator>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<EventCleanupOptions>(builder.Configuration.GetSection(EventCleanupOptions.SectionName));

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    return new MongoClient(config.GetRequiredMongoConnectionString());
});

builder.Services.AddSingleton<IEventRepository, EventRepository>();
builder.Services.AddSingleton<IEventReadRepository>(sp => sp.GetRequiredService<IEventRepository>());
builder.Services.AddSingleton<IEventWriteRepository>(sp => sp.GetRequiredService<IEventRepository>());
builder.Services.AddSingleton<IEventRetentionRepository>(sp => sp.GetRequiredService<IEventRepository>());

builder.Services.AddHostedService<EventCleanupJob>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<LibraryEventConsumer>();
    x.AddConsumer<LibraryEventFaultConsumer>();

    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.ConfigureVimachemRabbitMqHost(builder.Configuration);

        cfg.ReceiveEndpoint("library-events", e =>
        {
            e.UseMessageRetry(r => r.Exponential(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(5)));
            e.UseDelayedRedelivery(r => r.Intervals(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(15)));
            e.ConfigureConsumer<LibraryEventConsumer>(ctx);
        });

        cfg.ReceiveEndpoint("library-events-dlq", e =>
        {
            e.ConfigureConsumer<LibraryEventFaultConsumer>(ctx);
        });
    });
});

var app = builder.Build();

var repo = app.Services.GetRequiredService<IEventRepository>();
repo.EnsureIndexes();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.MapDefaultEndpoints();
app.Run();

using FluentValidation;
using FluentValidation.AspNetCore;
using Library.Api.Validators;
using Library.Application;
using Library.Infrastructure;
using Library.Infrastructure.Middleware;
using Library.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateBookRequestValidator>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddMemoryCache();

var app = builder.Build();

await DatabaseInitializer.InitializeAsync(app);

app.UseSwagger();
app.UseSwaggerUI();

app.UseDomainExceptionHandler();

app.MapControllers();
app.MapDefaultEndpoints();
app.Run();

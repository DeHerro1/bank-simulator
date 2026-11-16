using System.Text.Json.Serialization;
using BankSim.Api.BackgroundServices;
using BankSim.Api.Middleware;
using BankSim.Api.Notifications;
using BankSim.Application.Repositories;
using BankSim.Application.Services;
using BankSim.Application.Validation;
using BankSim.Infrastructure.Persistence;
using BankSim.Infrastructure.Repositories;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Persistence
string connectionString = builder.Configuration.GetConnectionString("BankSim")
    ?? "Data Source=banksim.db";
builder.Services.AddDbContext<BankSimDbContext>(options => options.UseSqlite(connectionString));

// Repositories
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

// Application services
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ITransferService, TransferService>();
builder.Services.AddScoped<IStatementService, StatementService>();
builder.Services.AddScoped<IInterestAccrualService, InterestAccrualService>();
builder.Services.AddScoped<INotificationService, ConsoleNotificationService>();

// Background job
builder.Services.AddHostedService<InterestAccrualBackgroundService>();

// Validation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateCustomerRequestValidator>();

builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddOpenApi();

var app = builder.Build();

// Apply pending EF Core migrations automatically on startup — a convenience for a learning
// project's dev loop; a production deployment would run migrations as a separate release step.
if (app.Environment.IsDevelopment())
{
    using IServiceScope scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<BankSimDbContext>();
    await db.Database.MigrateAsync();

    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseBankSimExceptionHandling();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

// Exposed for WebApplicationFactory-based integration tests, if added later.
public partial class Program { }

using System.Reflection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath)) c.IncludeXmlComments(xmlPath);
});

// Development CORS policy (use restrictive origins for non-dev)
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCors", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Support forwarded headers if running behind a proxy / TLS termination
builder.Services.Configure<ForwardedHeadersOptions>(opts =>
{
    opts.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

// Register DbContext and application services
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection is required.");

builder.Services.AddDbContext<Unison.LibraryManagement.Infrastructure.Persistence.LibraryManagementDbContext>(options =>
    options.UseSqlServer(connectionString));

// Repositories and infrastructure
builder.Services.AddScoped<Unison.LibraryManagement.Domain.Repositories.IUserRepository, Unison.LibraryManagement.Infrastructure.Repositories.EfUserRepository>();
builder.Services.AddScoped<Unison.LibraryManagement.Domain.Repositories.IRoleRepository, Unison.LibraryManagement.Infrastructure.Repositories.EfRoleRepository>();

// Library repositories
builder.Services.AddScoped<Unison.LibraryManagement.Domain.Repositories.IBookRepository, Unison.LibraryManagement.Infrastructure.Repositories.EfBookRepository>();
builder.Services.AddScoped<Unison.LibraryManagement.Domain.Repositories.IBookCopyRepository, Unison.LibraryManagement.Infrastructure.Repositories.EfBookCopyRepository>();
builder.Services.AddScoped<Unison.LibraryManagement.Domain.Repositories.ILoanRepository, Unison.LibraryManagement.Infrastructure.Repositories.EfLoanRepository>();
builder.Services.AddScoped<Unison.LibraryManagement.Domain.Repositories.IFineRepository, Unison.LibraryManagement.Infrastructure.Repositories.EfFineRepository>();
builder.Services.AddScoped<Unison.LibraryManagement.Domain.Repositories.IUnitOfWork, Unison.LibraryManagement.Infrastructure.Persistence.LibraryManagementUnitOfWork>();

// Password hasher
builder.Services.AddSingleton<Unison.LibraryManagement.Application.Security.IPasswordHasher, Unison.LibraryManagement.Infrastructure.Security.PasswordHasher>();

// Application handlers
builder.Services.AddScoped<Unison.LibraryManagement.Application.Handlers.RegisterUserHandler>();
builder.Services.AddScoped<Unison.LibraryManagement.Application.Handlers.AssignRoleHandler>();
builder.Services.AddScoped<Unison.LibraryManagement.Application.Handlers.CreateBookHandler>();
builder.Services.AddScoped<Unison.LibraryManagement.Application.Handlers.BorrowBookHandler>();
builder.Services.AddScoped<Unison.LibraryManagement.Application.Handlers.ReturnBookHandler>();

// Library services
// book search handler
builder.Services.AddScoped<Unison.LibraryManagement.Application.Handlers.SearchBooksHandler>();
builder.Services.AddScoped<Unison.LibraryManagement.Application.Handlers.GetUserLoansHandler>();
builder.Services.AddScoped<Unison.LibraryManagement.Application.Handlers.GetOverdueLoansHandler>();
builder.Services.AddScoped<Unison.LibraryManagement.Application.Handlers.GetOutstandingFinesHandler>();
// keep fine handler/service
builder.Services.AddScoped<Unison.LibraryManagement.Application.Services.IFineService, Unison.LibraryManagement.Application.Services.FineService>();

// Authentication
builder.Services.AddAuthentication("Basic").AddScheme<Microsoft.AspNetCore.Authentication.AuthenticationSchemeOptions, Unison.LibraryManagement.Api.Auth.BasicAuthenticationHandler>("Basic", null);
builder.Services.AddAuthorization();

var app = builder.Build();

// Process forwarded headers early
app.UseForwardedHeaders();
app.UseMiddleware<Unison.LibraryManagement.Api.ExceptionHandlingMiddleware>();

// Seed admin user if environment variables provided
await Unison.LibraryManagement.Api.Startup.AdminSeeder.SeedAsync(app.Services);

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseCors("DevCors");
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Unison.LibraryManagement.Api v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

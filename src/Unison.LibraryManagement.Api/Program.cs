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
    ?? "Server=127.0.0.1,1433;User Id=sa;Password=YourStrong!Passw0rd;Database=LibraryManagementDb;TrustServerCertificate=True;";

builder.Services.AddDbContext<Unison.LibraryManagement.Infrastructure.Persistence.LibraryManagementDbContext>(options =>
    options.UseSqlServer(connectionString));

// Repositories and infrastructure
builder.Services.AddScoped<Unison.LibraryManagement.Domain.Repositories.IUserRepository, Unison.LibraryManagement.Infrastructure.Repositories.EfUserRepository>();
builder.Services.AddScoped<Unison.LibraryManagement.Domain.Repositories.IRoleRepository, Unison.LibraryManagement.Infrastructure.Repositories.EfRoleRepository>();

// Password hasher
builder.Services.AddSingleton<Unison.LibraryManagement.Application.Security.IPasswordHasher, Unison.LibraryManagement.Infrastructure.Security.PasswordHasher>();

// Application handlers
builder.Services.AddScoped<Unison.LibraryManagement.Application.Handlers.RegisterUserHandler>();
builder.Services.AddScoped<Unison.LibraryManagement.Application.Handlers.AssignRoleHandler>();

// Authentication
builder.Services.AddAuthentication("Basic").AddScheme<Microsoft.AspNetCore.Authentication.AuthenticationSchemeOptions, Unison.LibraryManagement.Api.Auth.BasicAuthenticationHandler>("Basic", null);
builder.Services.AddAuthorization();

var app = builder.Build();

// Process forwarded headers early
app.UseForwardedHeaders();

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

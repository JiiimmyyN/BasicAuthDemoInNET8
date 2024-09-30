using Identity.API.Data;
using Identity.API.HostedServices;
using Identity.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

// https://andrewlock.net/exploring-the-dotnet-8-preview-introducing-the-identity-api-endpoints/
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthorization();

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services
    .AddIdentityApiEndpoints<ApplicationUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddHostedService<MigrationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapGet("/un-secure", () => "No authorization required for this endpoint")
    .WithName("UnSecure")
    .WithOpenApi();

app.MapGet("/secured", () => "Requires logged in user")
    .WithName("Secured")
    .RequireAuthorization()
    .WithOpenApi();

app.MapGroup("/identity").MapIdentityApi<ApplicationUser>();

app.Run();

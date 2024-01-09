using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PandaTech.IEnumerableFilters.Dto;
using PandaTech.IEnumerableFilters.Extensions;
using TestFilters.Components;
using TestFilters.db;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContext<PostgresContext>(
    optionsBuilder => optionsBuilder.UseNpgsql(
        "Host=localhost;Database=filter_test;Username=test;Password=test"
    ));

builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<PostgresContext>();
db.Database.EnsureCreated();

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapPost("/api/generate/{count:int}", (PostgresContext context, int count) => context.Populate(count));
app.MapGet("/api/companies", (PostgresContext context, [FromQuery] int page, [FromQuery] int pageSize,
    [FromQuery] string q) => S.Companies(context, page, pageSize, q));

app.Run();


class S
{
    public static async Task<List<Company>> Companies(PostgresContext context, [FromQuery] int page,
        [FromQuery] int pageSize,
        [FromQuery] string q)
    {
        var req = GetDataRequest.FromString(q);


        return await context.Companies
            .ApplyFilters(req.Filters)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}
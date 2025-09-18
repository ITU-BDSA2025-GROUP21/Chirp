using SimpleDB;
using System.Text.Json;
using WebAPI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IDatabaseRepository<Cheep>, CSVDatabase<Cheep>>();

var app = builder.Build();

app.MapGet("/cheeps", async (int? limit, IDatabaseRepository < Cheep> dbRepo) =>
{
    IEnumerable<Cheep> cheeps = dbRepo.Read(limit);
    return cheeps;
});

app.MapPost("/cheep", async (IDatabaseRepository<Cheep> dbRepo, Cheep cheep) =>
{

    if (cheep != null)
    {
        dbRepo.Store(cheep);
        return Results.Ok("Cheep stored successfully.");
    } else
    {
        return Results.BadRequest("Invalid Cheep data.");
    }
});


app.Run();

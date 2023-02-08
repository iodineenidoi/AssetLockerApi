using AssetLockerApi.Core;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
var projectsController = new ProjectsController();
var random = new Random();

app.MapGet("/getRandom", () =>
{
    int number = random.Next();
    return $"Random number is: {number}";
});

app.MapGet("/getAll/{projectName}", (string projectName) =>
{
    Project project = projectsController.GetProject(projectName);
    GetAllResponse response = project.GetAll();
    return response;
});

app.MapPost("/isLocked", ([FromBody] IsLockedRequest req) =>
{
    Project project = projectsController.GetProject(req.Project);
    IsLockedResponse response = project.IsLocked(req);
    return response;
});

app.MapPost("/lockAssets", ([FromBody] LockAssetsRequest req) =>
{
    Project project = projectsController.GetProject(req.Project);
    LockAssetsResponse response = project.LockAssets(req);
    return response;
});

app.MapPost("/unlockAssets", ([FromBody] UnlockAssetsRequest req) =>
{
    Project project = projectsController.GetProject(req.Project);
    UnlockAssetsResponse response = project.UnlockAssets(req);
    return response;
});

app.Run();
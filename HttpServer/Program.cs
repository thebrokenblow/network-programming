using HttpServer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

builder.Services.AddSingleton<List<User>>();
builder.Services.AddSingleton<UserService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/users", (UserService userService) =>
{
    var users = userService.GetAll();
    return Results.Ok(users);
});

app.MapGet("/users/{id}", (int id, UserService userService) =>
{
    var user = userService.GetById(id);
    return Results.Ok(user);
});

app.MapPost("/users/", (User user, UserService userService) =>
{
    userService.Create(user);
    return Results.Created();
});

app.MapPut("/users/{id}", (int id, User user, UserService userService) =>
{
    userService.Update(id, user);
    return Results.Ok();
});

app.MapDelete("/users/{id}", (int id, UserService userService) =>
{
    userService.Delete(id);
    return Results.Ok();
});

app.Run();
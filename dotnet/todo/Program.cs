var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.UseHttpsRedirection();

var todos = new List<Todo>
{
    new() {
        Id = Guid.NewGuid(),
        CreatedAt = DateTime.UtcNow,
        Name = "Learn ASP.NET Core",
        Status = false
    },
    new() {
        Id = Guid.NewGuid(),
        CreatedAt = DateTime.UtcNow,
        Name = "Build a REST API",
        Status = false
    },
    new() {
        Id = Guid.NewGuid(),
        CreatedAt = DateTime.UtcNow,
        Name = "Deploy the app",
        Status = false
    }
};

// Create
app.MapPost("/todos", (TodoInputDto todoInputDto) =>
{
    var todo = new Todo
    {
        Id = Guid.NewGuid(),
        CreatedAt = DateTime.UtcNow,
        Name = todoInputDto.Name,
        Status = todoInputDto.Status
    };
    todos.Add(todo);
    return Results.Created($"/todos/{todo.Id}", todo);
});

// Read (Get All)
app.MapGet("/todos", () =>
{
    return Results.Ok(todos);
});

// Read (Get By Id)
app.MapGet("/todos/{id}", (Guid id) =>
{
    var todo = todos.FirstOrDefault(t => t.Id == id);
    return todo is null ? Results.NotFound() : Results.Ok(todo);
});

// Update
app.MapPut("/todos/{id}", (Guid id, TodoInputDto updatedTodoDto) =>
{
    var todo = todos.FirstOrDefault(t => t.Id == id);
    if (todo is null) return Results.NotFound();

    todo.Name = updatedTodoDto.Name;
    todo.Status = updatedTodoDto.Status;

    return Results.Ok(todo);
});

// Delete
app.MapDelete("/todos/{id}", (Guid id) =>
{
    var todo = todos.FirstOrDefault(t => t.Id == id);
    if (todo is null) return Results.NotFound();

    todos.Remove(todo);
    return Results.Ok();
});

app.Run();

public class Todo
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool Status { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class TodoInputDto
{
    public string Name { get; set; } = string.Empty;
    public bool Status { get; set; } = false;
}

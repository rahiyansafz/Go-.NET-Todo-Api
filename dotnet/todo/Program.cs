var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseHttpsRedirection();

var todos = new List<Todo>
{
    new() { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, Name = "Learn ASP.NET Core", Status = false },
    new() { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, Name = "Build a REST API", Status = false },
    new() { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, Name = "Deploy the app", Status = false }
};

app.MapPost("/todos", CreateTodo);
app.MapGet("/todos", GetTodos);
app.MapGet("/todos/{id}", GetTodoById);
app.MapPut("/todos/{id}", UpdateTodo);
app.MapDelete("/todos/{id}", DeleteTodo);

app.Run();

static IResult CreateTodo(TodoInputDto todoInputDto, List<Todo> todos)
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
}

static IResult GetTodos(List<Todo> todos)
{
    return Results.Ok(todos);
}

static IResult GetTodoById(Guid id, List<Todo> todos)
{
    var todo = todos.FirstOrDefault(t => t.Id == id);
    return todo is null ? Results.NotFound() : Results.Ok(todo);
}

static IResult UpdateTodo(Guid id, TodoInputDto updatedTodoDto, List<Todo> todos)
{
    var todo = todos.FirstOrDefault(t => t.Id == id);
    if (todo is null) return Results.NotFound();

    todo.Name = updatedTodoDto.Name;
    todo.Status = updatedTodoDto.Status;

    return Results.Ok(todo);
}

static IResult DeleteTodo(Guid id, List<Todo> todos)
{
    var todo = todos.FirstOrDefault(t => t.Id == id);
    if (todo is null) return Results.NotFound();

    todos.Remove(todo);
    return Results.Ok();
}

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

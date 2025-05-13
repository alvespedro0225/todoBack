using todoBack.Models;

namespace todoBack.Endpoints;

public static class Todos
{
    private static readonly List<Todo> TodosList = [];
    private static int _id = 1;

    public static void MapTodosEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("todos");
        group.MapGet("", GetTodos);
        group.MapPost("", CreateTodo);
        group.MapGet("{todoId:int}", GetTodo);
        group.MapDelete("{todoId:int}", DeleteTodo);
        group.MapPut("{todoId:int}", EditTodo);
    }

    public static IResult GetTodos(HttpContext context)
    {
        return Results.Ok(TodosList);
    }

    public static IResult GetTodo(int todoId)
    {
        Console.WriteLine("teste");
        var todo = TodosList.FirstOrDefault(listTodo => listTodo.Id == todoId);
        return todo is null ? Results.NotFound() : Results.Ok(todo);
    }

    public static IResult EditTodo(int todoId, Todo updatedTodo)
    {
        var oldTodoIndex = TodosList.FindIndex(listTodo => listTodo.Id == todoId);
        
        if (oldTodoIndex == -1)
            return Results.NotFound();

        TodosList[oldTodoIndex] = updatedTodo;
        return Results.Ok(updatedTodo);
    }

    public static IResult CreateTodo(HttpContext context, CreateTodoRequest todoData)
    {
        if (TodosList.Exists(listTodo => todoData.Name == listTodo.Name))
            return Results.BadRequest("Name already in use. Choose another.");

        var now = DateTime.Now;
        Todo newTodo = new()
        {
            Name = todoData.Name,
            Description = todoData.Description,
            Status = todoData.Status,
            CreatedAt = now,
            UpdatedAt = now,
            Id = _id++
        };
        TodosList.Add(newTodo);
        return Results.Created($"todo/{newTodo.Id}", newTodo);
    }

    public static IResult DeleteTodo(int todoId)
    {
        var deletedTodo = TodosList.FirstOrDefault(todo => todo.Id == todoId);

        if (deletedTodo is null)
            return Results.NotFound();

        TodosList.Remove(deletedTodo);
        return Results.Ok(deletedTodo);
    }
}
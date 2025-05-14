using API.Data.Models;
using Microsoft.Extensions.Primitives;

namespace API.Endpoints;

public static class Todos
{
    private static List<Todo> TodosList { get; } = [];
    private static int _id = 1;
    private const string Api = "todos";
    public static void MapTodosEndpoints(this WebApplication app)
    {
        var group = app.MapGroup(Api);
        group.MapGet("", GetTodos);
        group.MapPost("", CreateTodo);
        group.MapGet("{todoId:int}", GetTodo);
        group.MapDelete("{todoId:int}", DeleteTodo);
        group.MapPut("{todoId:int}", EditTodo);
    }

    public static IResult GetTodos(HttpRequest request, HttpResponse response)
    {

        if (response.Headers.ContainsKey("If-None-Match") &&
            response.Headers.IfNoneMatch == TodosList.GetHashCode().ToString())
            return Results.StatusCode(StatusCodes.Status304NotModified);
        
        response.Headers.CacheControl = new StringValues("max-age=2592000, private");
        response.Headers.ETag = new StringValues(TodosList.GetHashCode().ToString());
        return Results.Ok(TodosList);
    }

    public static IResult GetTodo(HttpRequest request, HttpResponse response, int todoId)
    {
        var todo = TodosList.FirstOrDefault(listTodo => listTodo.Id == todoId);

        if (todo is null)
            return Results.NotFound();
        
        if (request.Headers.ContainsKey("If-None-Match") &&
            request.Headers.IfNoneMatch == todo.GetHashCode().ToString())
            return Results.StatusCode(StatusCodes.Status304NotModified);
        
        response.Headers.CacheControl = new StringValues("max-age=2592000, private");
        response.Headers.ETag = new StringValues(todo.GetHashCode().ToString());
        
        return Results.Ok(todo);
    }

    public static IResult EditTodo(HttpResponse response, int todoId, Todo updatedTodo)
    {
        // looks for an item with the same name and different id
        if (TodosList.Exists(todo =>
                string.Equals(todo.Name, updatedTodo.Name, StringComparison.CurrentCultureIgnoreCase) &&
                todo.Id != updatedTodo.Id))
            return Results.BadRequest("Name already in use. Choose another.");
        
        var oldTodoIndex = TodosList.FindIndex(listTodo => listTodo.Id == todoId);
        
        if (oldTodoIndex == -1)
            return Results.NotFound();
        
        updatedTodo.UpdatedAt = DateTime.Now;
        var headers = response.Headers;
        headers.AccessControlExposeHeaders = new StringValues("Location");
        headers.Location = new StringValues($"/${Api}/{updatedTodo.Id}");
        TodosList[oldTodoIndex] = updatedTodo;
        return Results.Ok(updatedTodo);
    }

    public static IResult CreateTodo(HttpResponse response, CreateTodoRequest todoData)
    {
        if (TodosList.Exists(listTodo => todoData.Name == listTodo.Name))
            return Results.BadRequest("Name already in use. Choose another.");
	
        response.Headers.AccessControlExposeHeaders = new StringValues("Location");
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
        return Results.Created($"/{Api}/{newTodo.Id}", newTodo);
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
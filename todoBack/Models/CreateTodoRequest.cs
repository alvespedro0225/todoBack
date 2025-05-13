namespace todoBack.Models;

public class CreateTodoRequest
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required Status Status { get; set; }
}
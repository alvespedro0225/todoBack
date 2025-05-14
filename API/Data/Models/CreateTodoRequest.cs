using API.Data.Enums;

namespace API.Data.Models;

public sealed class CreateTodoRequest
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required Status Status { get; set; }
}
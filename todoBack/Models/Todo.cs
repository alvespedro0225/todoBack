namespace todoBack.Models;

public class Todo
{
    public required string Name { get; set; }
    public required Status Status { get; set; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; set; }
    public required int Id { get; set; }
    public required string Description { get; set; }
}
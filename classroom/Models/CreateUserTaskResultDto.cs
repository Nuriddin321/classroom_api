using classroom.Entities;

namespace classroom.Models;

public class CreateUserTaskResultDto
{
    public string? Description { get; set; }
    public EUserTaskStatus Status { get; set; }
}
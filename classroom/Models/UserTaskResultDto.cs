using classroom.Entities;

namespace classroom.Models;

public class UserTaskResultDto : TaskDto
{
    public UserTaskResult? UserResult { get; set; }
}

public class UserTaskResult
{
    public string? Description { get; set; }
    public EUserTaskStatus Status { get; set; }
}
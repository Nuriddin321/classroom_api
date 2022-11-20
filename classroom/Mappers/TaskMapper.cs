using classroom.Models;

namespace classroom.Mappers;

public static class TaskMapper
{
    public static void SetValues(this classroom.Entities.Task task, UpdateTaskDto updateTaskDto)
    {
        task.Title = updateTaskDto.Title;
        task.Description = updateTaskDto.Description;
        task.StartDate = updateTaskDto.StartDate;
        task.EndDate = updateTaskDto.EndDate;
        task.MaxScore = updateTaskDto.MaxScore;
        task.Status = updateTaskDto.Status;
    }
}
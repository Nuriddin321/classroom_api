using classroom.Mappers;
using classroom.Models;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace classroom.Controllers;

public partial class CoursesController
{
    [HttpPost("{courseId}/tasks")]
    public async Task<IActionResult> AddTask(Guid courseId, [FromBody] CreateTaskDto createTaskDto)
    {   //kiritilgan inputlani tekshirvotti
        if (!ModelState.IsValid)
            return BadRequest();
        //berilgan id bo'yicha bazadan courseni chaqirvotti
        var course = _context.Courses.FirstOrDefault(c => c.Id == courseId);
        if (course is null)
            return NotFound();
        //request tshavotkan userni obyektini ovotti
        var user = await _userManager.GetUserAsync(User);
        //user bu kursda bo'lmasa va admin  bo'masa task qo'shomiydi 
        if (course.Users?.Any(uc => uc.UserId == user.Id && uc.IsAdmin) != true)
            return BadRequest();

        var task = createTaskDto.Adapt<classroom.Entities.Task>();
        task.CreatedDate = DateTime.Now;
        task.CourseId = courseId;

        await _context.Tasks.AddAsync(task);
        await _context.SaveChangesAsync();

        return Ok(task.Adapt<TaskDto>());
    }

    [HttpGet("{courseId}/tasks")]
    public async Task<IActionResult> GetTasks(Guid courseId)
    {
        var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseId);
        if (course is null)
            return BadRequest();

        var tasks = course.Tasks?.Select(t => t.Adapt<TaskDto>()).ToList();

        return Ok(tasks ?? new List<TaskDto>());
    }

    [HttpGet("{courseId}/tasks/{taskId}")]
    public async Task<IActionResult> GetTaskById(Guid courseId, Guid taskId)
    {
        var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == taskId && t.CourseId == courseId);

        if (task is null)
            return NotFound();
        return Ok(task.Adapt<TaskDto>());
    }

    public async Task<IActionResult> UpdateTask(Guid courseId, Guid taskId, [FromBody] UpdateTaskDto updateTaskDto)
    {
        //todo user kursni azosi va admini ekanligini tekshirish kerak

        var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == taskId && t.CourseId == courseId);
        if (task is null)
            return NotFound();

        task.SetValues(updateTaskDto);

        await _context.SaveChangesAsync();

        return Ok(task.Adapt<TaskDto>());
    }

    [HttpDelete("{courseId}/tasks/{taskId}")]
    public async Task<IActionResult> DeleteTask(Guid courseId, Guid taskId)
    {
        //todo user kursni azosi va admini ekanligini tekshirish kerak

        var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == taskId && t.CourseId == courseId);
        if (task is null)
            return NotFound();

        _context.Remove(task);
        await _context.SaveChangesAsync();

        return Ok();
    }


}
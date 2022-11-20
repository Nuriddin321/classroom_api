using classroom.Context;
using classroom.Entities;
using classroom.Mappers;
using classroom.Models;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Classroom.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly AppDbContext _context;

    public ProfileController(UserManager<User> userManager, AppDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    [HttpGet("courses")]
    public async Task<IActionResult> GetCourses()
    {
        var user = await _userManager.GetUserAsync(User);
        var coursesDto = user.Courses?.Select(userCourse => userCourse.Course?.ToDto()).ToList();
        return Ok(coursesDto);
    }

    [HttpGet("courses/{courseId}/tasks")]
    public async Task<IActionResult> GetUserTasks(Guid courseId)
    {
        //todo user kursni azosi ekanligini tekshirish kerak
        var user = await _userManager.GetUserAsync(User);
        //bazadan berilgan id bo'yicha course obyektini aniqlavotti
        var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseId);
        if (course?.Tasks is null)
            return NotFound();
        //cursni tasklarini ovotti
        var tasks = course.Tasks;
        var usertasks = new List<UserTaskResultDto>();

        foreach (var task in tasks)
        {
            var result = task.Adapt<UserTaskResultDto>(); // user tasklarini TaskTesultDto ovotti
            var userResultEntity = task.UserTasks?.FirstOrDefault(ut => ut.UserId == user.Id);

            result.UserResult = userResultEntity == null ? null : new UserTaskResult()
            {
                Status = userResultEntity.Status,
                Description = userResultEntity.Description
            };

            usertasks.Add(result);
        }

        return Ok(usertasks);
    }


    [HttpPost("courses/{courseId}/tasks/{taskId}")]
    public async Task<IActionResult> AddUserTaskResult(Guid courseId, Guid taskId, [FromBody] CreateUserTaskResultDto resultDto)
    {
        //todo user kursni azosi ekanligini tekshirish kerak

        var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == taskId && t.CourseId == courseId);
        if (task is null)
            return NotFound();

        var user = await _userManager.GetUserAsync(User);

        var userTaskResult = await _context.UserTasks
            .FirstOrDefaultAsync(ut => ut.UserId == user.Id && ut.TaskId == taskId);

        if (userTaskResult is null)
        {
            userTaskResult = new UserTask()
            {
                UserId = user.Id,
                TaskId = taskId
            };

            await _context.UserTasks.AddAsync(userTaskResult);
        }

        userTaskResult.Description = resultDto.Description;
        userTaskResult.Status = resultDto.Status;

        await _context.SaveChangesAsync();

        return Ok();
    }
}
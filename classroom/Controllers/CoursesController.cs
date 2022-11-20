using classroom.Context;
using classroom.Entities;
using classroom.Mappers;
using classroom.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace classroom.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public partial class CoursesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly UserManager<User> _userManager;

    public CoursesController(AppDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> GetCourses()
    {
        var courses = await _context.Courses.ToListAsync();
        List<CourseDto> coursesDto = courses.Select(c => c.ToDto()).ToList();
        return Ok(coursesDto);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCourseById(Guid id)
    {
        var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);

        if (course is null)
            return NotFound();

        return Ok(course?.ToDto());
    }

    [HttpPost]
    public async Task<IActionResult> CreateCourse([FromBody] CreateCourseDto createCourseDto)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        var user = await _userManager.GetUserAsync(User);

        var course = new Course()
        {
            Name = createCourseDto.Name,
            Key = Guid.NewGuid().ToString("N"),
            Users = new List<UserCourse>()
            {
                new UserCourse()
                {
                    UserId = user.Id,
                    IsAdmin = true
                }
            }
        };

        await _context.Courses.AddAsync(course);
        await _context.SaveChangesAsync();

        course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == course.Id);

        return Ok(course?.ToDto());
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCourse(Guid id, [FromBody] UpdateCourseDto updateCourseDto)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        if (!await _context.Courses.AnyAsync(c => c.Id == id))
            return NotFound();

        var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);
        if (course is null)
            return NotFound();

        var user = await _userManager.GetUserAsync(User);

        if (course.Users?.Any(uc => uc.UserId == user.Id && uc.IsAdmin) != true)
            return BadRequest();

        course.Name = updateCourseDto.Name;
        await _context.SaveChangesAsync();

        return Ok(course?.ToDto());
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCourse(Guid id)
    {
        var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);
        if (course is null)
            return NotFound();

        var user = await _userManager.GetUserAsync(User);
        if (course.Users?.Any(uc => uc.UserId == user.Id && uc.IsAdmin) != true)
            return Forbid();

        _context.Courses.Remove(course);
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpPost("{id}/join")]
    public async Task<IActionResult> JoinCourse(Guid id, [FromBody] JoinCourseDto joinCourseDto)
    {
        var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);

        if (course is null)
            return NotFound();

        var user = await _userManager.GetUserAsync(User);

        if (course.Users?.Any(uc => uc.UserId == user.Id) == true)
            return BadRequest();

        _context.UserCourses.Add(new UserCourse()
        {
            UserId = user.Id,
            CourseId = course.Id,
            IsAdmin = false
        });

        await _context.SaveChangesAsync();

        return Ok();
    }

}
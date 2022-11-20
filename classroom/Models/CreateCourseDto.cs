using System.ComponentModel.DataAnnotations;

namespace classroom.Models;

public class CreateCourseDto
{
    [Required]
    public string? Name { get; set; }
}
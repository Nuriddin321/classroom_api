using System.ComponentModel.DataAnnotations;

namespace classroom.Models;

public class UpdateCourseDto
{
    [Required]
    public string? Name { get; set; }
}
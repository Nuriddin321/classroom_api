﻿using System.ComponentModel.DataAnnotations;
using classroom.Entities;

namespace classroom.Models;

public class CreateTaskDto
{
    [Required]
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int MaxScore { get; set; }
    public ETaskStatus Status { get; set; }
}
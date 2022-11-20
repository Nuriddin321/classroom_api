using classroom.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace classroom.Context;
public class AppDbContext : IdentityDbContext<User, Role, Guid>
{

    public DbSet<Course> Courses { get; set; }
    public DbSet<UserCourse> UserCourses { get; set; }
    public DbSet<Entities.Task> Tasks { get; set; }
    public DbSet<UserTask> UserTasks { get; set; }
    
    public AppDbContext(DbContextOptions options) : base(options) { }
}

using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudentProject.Entities;

namespace StudentProject
{
    public class AppDBContext: IdentityDbContext
    {
        public AppDBContext([NotNullAttribute] DbContextOptions options):base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StudentCourse>()
                .HasKey(x => new { x.StudentId, x.CourseId });
            modelBuilder.Entity<StudentCategory>()
                .HasKey(x => new { x.StudentId, x.CategoryId });
            base.OnModelCreating(modelBuilder);
        }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<StudentCourse> StudentsCourses { get; set; }
        public DbSet<StudentCategory> StudentCategories { get; set; }
    }
}

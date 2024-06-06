using System.ComponentModel.DataAnnotations;

namespace StudentProject.Entities
{
    public class Student
    {
        public int Id { get; set; }
        [StringLength(maximumLength:75)]
        [Required]
        public string Name { get; set; }
        public string RollNo { get; set; }
        public Boolean IsActive { get; set; }
        public Boolean IsTopper { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Image { get; set; }
        public List<StudentCourse> StudentCourses { get; set; }
        public List<StudentCategory> StudentCategories { get; set; }
    }
}

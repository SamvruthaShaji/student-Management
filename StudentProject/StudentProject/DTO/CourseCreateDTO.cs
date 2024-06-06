using StudentProject.Validations;
using System.ComponentModel.DataAnnotations;

namespace StudentProject.DTO
{
    public class CourseCreateDTO
    {
        [Required(ErrorMessage = "Name should not be blank")]
        [StringLength(50)]
        [BeginUppercase]
        public string Name { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using StudentProject.Validations;
namespace StudentProject.Entities
{
    public class Course
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Name should not be blank")]
        [StringLength(50)]
        [BeginUppercase]
        public string Name { get; set; }

    }
}  
using System.ComponentModel.DataAnnotations;
using StudentProject.Validations;
namespace StudentProject.DTO
{
    public class CategoryCreateDTO
    {
        [Required]
        [StringLength(100)]
        [BeginUppercase]
        public string Name { get; set; }
    }
}

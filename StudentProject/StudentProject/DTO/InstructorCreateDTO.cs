using System.ComponentModel.DataAnnotations;

namespace StudentProject.DTO
{
    public class InstructorCreateDTO
    {
        [Required]
        [StringLength(120)]
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public IFormFile Image { get; set; }
        public string Career { get; set; }
    }
}

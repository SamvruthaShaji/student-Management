using System.ComponentModel.DataAnnotations;

namespace StudentProject.Entities
{
    public class Instructor
    {
        public int Id { get; set; }
        [Required]
        [StringLength(120)]
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Image {  get; set; }
        public string Career { get; set; }
    }
}

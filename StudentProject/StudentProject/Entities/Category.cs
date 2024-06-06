using System.ComponentModel.DataAnnotations;
using StudentProject.Validations;

namespace StudentProject.Entities
{
    public class Category
    {
     public int Id {  get; set; }
        [Required]
        [StringLength(100)]
        [BeginUppercase]

        public string Name { get; set; }
    }
}

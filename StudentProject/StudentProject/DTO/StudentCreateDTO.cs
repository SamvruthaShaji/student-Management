using Microsoft.AspNetCore.Mvc;
using StudentProject.Helpers;

namespace StudentProject.DTO
{
    public class StudentCreateDTO
    {
        public string Name { get; set; }
        public string RollNo { get; set; }
        public Boolean IsActive { get; set; }
        public Boolean IsTopper { get; set; }
        public DateTime DateOfBirth { get; set; }
        public IFormFile Image { get; set; }

        [ModelBinder(BinderType = typeof(TypeBinder<List<int>>))]
        public List<int> CourseIds { get; set; }

        [ModelBinder(BinderType = typeof(TypeBinder<List<int>>))]
        public List<int> CategoryIds { get; set; }
    }
}

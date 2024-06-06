namespace StudentProject.DTO
{
    public class StudentDTO
    {
        public int Id { get; set; }
        
        public string Name { get; set; }
        public string RollNo { get; set; }
        public Boolean IsActive { get; set; }
        public Boolean IsTopper { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Image { get; set; }
        public List<CourseDTO> Courses { get; set; }
        public List<CategoryDTO> Categories { get; set; }
    }
}

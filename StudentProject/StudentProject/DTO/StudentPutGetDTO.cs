namespace StudentProject.DTO
{
    public class StudentPutGetDTO
    {
        public StudentDTO Student { get; set; }
        public List<CourseDTO> SelectedCourses { get; set; }
        public List<CourseDTO> UnSelectedCourses { get; set; }
        public List<CategoryDTO> SelectedCategories { get; set; }
        public List<CategoryDTO> UnSelectedCategories { get; set; }
    }
}

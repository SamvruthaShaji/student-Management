namespace StudentProject.Entities
{
    public class StudentCategory
    {
        public int CategoryId { get; set; }
        public int StudentId { get; set; }
        public Category Category { get; set; }
        public Student Student { get; set; }
    }
}

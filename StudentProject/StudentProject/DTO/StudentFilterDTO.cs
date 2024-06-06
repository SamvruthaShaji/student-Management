namespace StudentProject.DTO
{
    public class StudentFilterDTO
    {
        public int Page { get; set; }
        public int RecordsPerPage { get; set; }
        public PaginationDTO PaginationDTO
        {
            get { return new PaginationDTO() { Page = Page, RecordsPerPage = RecordsPerPage }; }

        }
        public string Name { get; set; }
        public int CourseId { get; set; }
        public bool ActiveList { get; set; }
        public bool ToppersList { get; set; }
    }
}

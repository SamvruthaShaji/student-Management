using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentProject.DTO;
using StudentProject.Entities;
using StudentProject.Helpers;

namespace StudentProject.Controllers
{
    [Route("api/student")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy ="IsAdmin")]
    public class StudentsController : ControllerBase
    {
        private readonly AppDBContext dBContext;
        private readonly IMapper mapper;
        private readonly IFileStorageServices fileStorageServices;
        private readonly string containerName = "students";

        public StudentsController(AppDBContext dBContext, IMapper mapper, IFileStorageServices fileStorageServices)
        {
            this.dBContext = dBContext;
            this.mapper = mapper;
            this.fileStorageServices = fileStorageServices;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<MainPageDTO>> Get()
        {
            var studentList = await dBContext.Students
                .OrderBy(x => x.Name)
                .ToListAsync();

            var toppersList = await dBContext.Students
                .Where(x => x.IsTopper == true)
                .OrderBy(x => x.Name)
                .ToListAsync();
            var mainPageDTO = new MainPageDTO();
            mainPageDTO.StudentList = mapper.Map<List<StudentDTO>>(studentList);
            mainPageDTO.ToppersList = mapper.Map<List<StudentDTO>>(toppersList);
            return mainPageDTO;
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<StudentDTO>> Get(int id)
        {
            var student = await dBContext.Students
                .Include(x=> x.StudentCourses).ThenInclude(x=>x.Course)
                .Include(x=>x.StudentCategories).ThenInclude(x=>x.Category)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (student == null)
            {
                return NotFound();
            }
            return mapper.Map<StudentDTO>(student);
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<StudentDTO>>> Search([FromQuery] StudentFilterDTO studentFilterDTO)
        {
            var studentQueryable = dBContext.Students.AsQueryable();
            if (!string.IsNullOrEmpty(studentFilterDTO.Name))

                studentQueryable = studentQueryable.Where(x => x.Name.Contains(studentFilterDTO.Name));

            if (studentFilterDTO.ActiveList)

                studentQueryable = studentQueryable.Where(x => x.IsActive);

            if (studentFilterDTO.ToppersList)

                studentQueryable = studentQueryable.Where(x => x.IsTopper);

            if (studentFilterDTO.CourseId != 0)
            {
                studentQueryable = studentQueryable.Where(x => x.StudentCourses.Select(y => y.CourseId)
                .Contains(studentFilterDTO.CourseId));
            }
            await HttpContext.InsertParameterPaginationHeader(studentQueryable);
            var students = await studentQueryable.OrderBy(x => x.Name).Paginate(studentFilterDTO.PaginationDTO)
                .ToListAsync();

            return mapper.Map<List<StudentDTO>>(students);

        }

        [HttpGet("PostGet")]
        public async Task<ActionResult<StudentPostGetDTO>> PostGet()
        {
            var courses = await dBContext.Courses.OrderBy(x => x.Name).ToListAsync();
            var categories = await dBContext.Categories.OrderBy(x => x.Name).ToListAsync();

            var coursesDTO = mapper.Map<List<CourseDTO>>(courses);
            var categoryDTO = mapper.Map<List<CategoryDTO>>(categories);

            return new StudentPostGetDTO() { Courses = coursesDTO, Categories = categoryDTO };
        }

        [HttpPost]
        public async Task<ActionResult<int>> Post([FromForm] StudentCreateDTO studentCreationDTO)
        {
            var student = mapper.Map<Student>(studentCreationDTO);
            if (studentCreationDTO.Image != null)
            {
                student.Image = await fileStorageServices.SaveFile(containerName, studentCreationDTO.Image);
            }
            dBContext.Students.Add(student);
            await dBContext.SaveChangesAsync();
            return student.Id;
        }

        [HttpGet("putget/{id:int}")]
        public async Task<ActionResult<StudentPutGetDTO>> PutGet(int id)
        {
            var studentActionResult = await Get(id);
            if (studentActionResult.Result is NotFoundResult) { return NotFound(); }

            var student = studentActionResult.Value;

            var coursesSelectedIds = student.Courses.Select(x => x.Id).ToList();
            var unSelectedCourses = await dBContext.Courses.Where(x => !coursesSelectedIds.Contains(x.Id)).ToListAsync();

            var categorySelectedIds = student.Categories.Select(x => x.Id).ToList();
            var unSelectedCategories = await dBContext.Categories.Where(x => !categorySelectedIds.Contains(x.Id)).ToListAsync();

            var unSelectedCoursesDTO = mapper.Map<List<CourseDTO>>(unSelectedCourses);
            var unSelectedCategoriesDTO = mapper.Map<List<CategoryDTO>>(unSelectedCategories);

            var response = new StudentPutGetDTO();
            response.Student = student;
            response.SelectedCourses = student.Courses;
            response.UnSelectedCourses = unSelectedCoursesDTO;
            response.SelectedCategories = student.Categories;
            response.UnSelectedCategories = unSelectedCategoriesDTO;
            return response;
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromForm] StudentCreateDTO studentCreationDTO)
        {
            var student = await dBContext.Students.Include(x => x.StudentCourses)
                .Include(x => x.StudentCategories)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (student == null) { return NotFound(); }


            student = mapper.Map(studentCreationDTO, student);
            if (studentCreationDTO.Image != null)
            {
                student.Image = await fileStorageServices.EditFile(containerName, studentCreationDTO.Image, student.Image);
            }
            await dBContext.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var student = await dBContext.Students.FirstOrDefaultAsync(x => x.Id == id);
            if (student == null)
            {
                return NotFound();
            }
            dBContext.Remove(student);
            await dBContext.SaveChangesAsync();
            await fileStorageServices.DeleteFile(containerName, student.Image);
            return NoContent();
        }
    }
}
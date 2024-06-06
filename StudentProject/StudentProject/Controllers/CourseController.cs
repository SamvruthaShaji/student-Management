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
    [Route("api/course")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
    public class CourseController : Controller
    {
        
        private readonly ILogger<CourseController> logger;
        private readonly AppDBContext dBContext;
        private readonly IMapper mapper;
        
        

        public CourseController( ILogger<CourseController> logger, AppDBContext dBContext, IMapper mapper)
            
        {
           
            this.logger = logger;
            this.dBContext = dBContext;
            this.mapper = mapper;
           
        }

        [HttpGet]
        
        
        public async Task<ActionResult<List<CourseDTO>>> Get([FromQuery]PaginationDTO paginationDTO)
        {
            var queryable = dBContext.Courses.AsQueryable();
            await HttpContext.InsertParameterPaginationHeader(queryable);
            var courses = await queryable.OrderBy(x=>x.Name).Paginate(paginationDTO).ToListAsync();
  
            return mapper.Map<List<CourseDTO>>(courses);    
        }
        [HttpGet("all")]
        [AllowAnonymous]
        public async Task<ActionResult<List<CourseDTO>>> Get()
        {
            var courses = await dBContext.Courses.OrderBy(x =>x.Name).ToListAsync();
            return mapper.Map<List<CourseDTO>>(courses);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CourseDTO>> Get(int id)
        {
            var course = await dBContext.Courses.FirstOrDefaultAsync(x => x.Id == id);
            if(course == null)
            {
                return NotFound();
            }
            return mapper.Map<CourseDTO>(course);  

        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] CourseCreateDTO courseCreationDTO)
        {
            var course = mapper.Map<Course>(courseCreationDTO); 
            dBContext.Courses.Add(course);
            await dBContext.SaveChangesAsync();
            return NoContent();
        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id,[FromBody] CourseCreateDTO  courseCreationDTO)
        {

            var course = await dBContext.Courses.FirstOrDefaultAsync(x => x.Id == id);
            if (course == null)
            {
                return NotFound();
            }
            course= mapper.Map(courseCreationDTO,course);
            await dBContext.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var dataExists = await dBContext.Courses.AnyAsync(x => x.Id == id);
            if (!dataExists)
            {
                return NotFound();
            }
            dBContext.Remove(new Course() { Id = id });
            await dBContext.SaveChangesAsync();
            return NoContent();
        }
    }
}

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
    [Route("api/instructor")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
    public class InstructorController : Controller
    {
        private readonly AppDBContext dBContext;
        private readonly IMapper mapper;
        private readonly IFileStorageServices fileStorageServices;
        private readonly string containerName = "instructors";
        public InstructorController(AppDBContext dBContext, IMapper mapper, IFileStorageServices fileStorageServices)
        {

            this.dBContext = dBContext;
            this.mapper = mapper;
            this.fileStorageServices = fileStorageServices;
        }
        [HttpGet]
        public async Task<ActionResult<List<InstructorDTO>>> Get([FromQuery] PaginationDTO paginationDTO)
        {
            var queryable =dBContext.Instructors.AsQueryable();
            await HttpContext.InsertParameterPaginationHeader(queryable);
            var instructors = await queryable.OrderBy(x => x.Name).Paginate(paginationDTO).ToListAsync();
            return mapper.Map<List<InstructorDTO>>(instructors);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<InstructorDTO>> Get(int id)
        {
            var instructor = await dBContext.Instructors.FirstOrDefaultAsync(x => x.Id == id);
            if (instructor == null)
            {
                return NotFound();
            }
            return mapper.Map<InstructorDTO>(instructor);
        }


        [HttpPost]
        public async Task<ActionResult> Post([FromForm] InstructorCreateDTO instructorCreationDTO)
        {
            var instructor = mapper.Map<Instructor>(instructorCreationDTO);
            if (instructorCreationDTO.Image != null)
            {
                instructor.Image = await fileStorageServices.SaveFile(containerName, instructorCreationDTO.Image);
            }
            dBContext.Instructors.Add(instructor);
            await dBContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromForm] InstructorCreateDTO instructorCreationDTO)
        {
            var instructor = await dBContext.Instructors.FirstOrDefaultAsync(x => x.Id == id);
            if (instructor == null)
            {
                return NotFound();
            }
            instructor = mapper.Map(instructorCreationDTO, instructor);
            if (instructorCreationDTO.Image != null)
            {
                instructor.Image = await fileStorageServices.EditFile(containerName, instructorCreationDTO.Image, instructor.Image);
            }
            await dBContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var instructor = await dBContext.Instructors.FirstOrDefaultAsync(x => x.Id == id);
            if (instructor == null)
            {
                return NotFound();
            }
            dBContext.Remove(instructor);
            await dBContext.SaveChangesAsync();
            await fileStorageServices.DeleteFile(containerName, instructor.Image);
            return NoContent();
        }

    }

}

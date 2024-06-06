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
    [Route("api/category")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
    public class CategoryController : Controller
    {
        private readonly ILogger<CategoryController> logger;
        private readonly AppDBContext dBContext;
        private readonly IMapper mapper;

        public CategoryController(ILogger<CategoryController> logger, AppDBContext dBContext, IMapper mapper)
        {

            this.logger = logger;
            this.dBContext = dBContext;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<CategoryDTO>>> Get([FromQuery] PaginationDTO paginationDTO)
        {
            var queryable = dBContext.Categories.AsQueryable();
            await HttpContext.InsertParameterPaginationHeader(queryable);

            var categories = await queryable.OrderBy(x => x.Name).Paginate(paginationDTO).ToListAsync();
            return mapper.Map<List<CategoryDTO>>(categories);
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<CategoryDTO>> Get(int id)
        {
            var category = await dBContext.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return mapper.Map<CategoryDTO>(category);
        }
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] CategoryCreateDTO categoryCreationDTO)
        {
            var category = mapper.Map<Category>(categoryCreationDTO);
            dBContext.Categories.Add(category);
            await dBContext.SaveChangesAsync();
            return NoContent();
        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromBody] CategoryCreateDTO categoryCreationDTO)
        {
            var category = await dBContext.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            category = mapper.Map(categoryCreationDTO, category);
            await dBContext.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var dataExists = await dBContext.Categories.AnyAsync(x => x.Id == id);
            if (!dataExists)
            {
                return NotFound();
            }
            dBContext.Remove(new Category() { Id = id });
            await dBContext.SaveChangesAsync();
            return NoContent();
        }
    }
}

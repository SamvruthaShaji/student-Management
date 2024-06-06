using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StudentProject.DTO;
using StudentProject.Helpers;

namespace StudentProject.Controllers
{
    [ApiController]
    [Route("api/accounts")]

    public class AccountsController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IConfiguration configuration;
        private readonly AppDBContext dBContext;
        private readonly IMapper mapper;

        public AccountsController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IConfiguration configuration,AppDBContext dBContext, IMapper mapper)
            
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
            this.dBContext = dBContext;
            this.mapper = mapper;
            

        }
        [HttpGet("userlist")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
        public async Task<ActionResult<List<UserDTO>>> GetUsersList([FromQuery] PaginationDTO paginationDTO)
        {
            var queryable =dBContext.Users.AsQueryable();
            await HttpContext.InsertParameterPaginationHeader(queryable);
            var users = await queryable.OrderBy(x => x.Email).Paginate(paginationDTO).ToListAsync();
            return mapper.Map<List<UserDTO>>(users);
        }

        [HttpPost("setadmin")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy ="IsAdmin")]
        public async Task<ActionResult> SetAdmin([FromBody] string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            await userManager.AddClaimAsync(user, new Claim("role", "admin"));
            return NoContent();
        }

        [HttpPost("unsetadmin")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
        public async Task<ActionResult> UnSetAdmin([FromBody] string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            await userManager.RemoveClaimAsync(user, new Claim("role", "admin"));
            return NoContent();
        }
        [HttpPost("create")]
        public async Task<ActionResult<AuthenticationResponse>> Create([FromBody] UserCredentials userCredentials)
        {
            var user = new IdentityUser { UserName = userCredentials.Email, Email = userCredentials.Email };
            var result = await userManager.CreateAsync(user, userCredentials.Password);
            if (result.Succeeded)
            {
                return BuildToken(userCredentials);
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthenticationResponse>> Login([FromBody] UserCredentials userCredentials)
        {
            var result = await signInManager.PasswordSignInAsync(userCredentials.Email, userCredentials.Password, isPersistent: false, lockoutOnFailure: false);
            
            if (result.Succeeded)
            {
                return BuildToken(userCredentials);
            }
            else
            {
                return BadRequest("Invalid Login");
            }
        }

        private AuthenticationResponse BuildToken(UserCredentials userCredentials)
        {
            //payload data
            var claims = new List<Claim>()
            {
                new Claim("email", userCredentials.Email)
            };
            var tokenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWTKEY"]));
            var credentials = new SigningCredentials(tokenKey, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddYears(1);
            var token = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: expiration,signingCredentials:credentials);
            
            return new AuthenticationResponse()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration
            };
        }
    }
}

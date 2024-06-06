
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Unicode;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using StudentProject;
using StudentProject.APIBehaviour;
using StudentProject.Filters;
using StudentProject.Helpers;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.Filters.Add(typeof(StudentExceptionFilter));
    options.Filters.Add(typeof(ParseBadRequest));
}
).ConfigureApiBehaviorOptions(BadRequestsBehaviour.Parse);

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("IsAdmin", policy => policy.RequireClaim("role", "admin"));
});

builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    var webURL = builder.Configuration.GetValue<string>("Web_URL");
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins(webURL).AllowAnyHeader().AllowAnyMethod()
        .WithExposedHeaders(new string[] { "recordCount" });
    });
});

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddScoped<IFileStorageServices,ApplicationStorageService>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<AppDBContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDBContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWTKEY"])),
            ClockSkew = TimeSpan.Zero
        };
    });
var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseCors();

app.UseResponseCaching();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    app.MapControllers();
});

app.Run();
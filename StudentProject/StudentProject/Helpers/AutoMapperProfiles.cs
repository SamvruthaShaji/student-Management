using AutoMapper;
using Microsoft.AspNetCore.Identity;
using StudentProject.DTO;
using StudentProject.Entities;

namespace StudentProject.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<CourseDTO, Course>().ReverseMap();
            CreateMap<CourseCreateDTO, Course>();

            CreateMap<InstructorDTO, Instructor>().ReverseMap();
            CreateMap<InstructorCreateDTO, Instructor>()
              .ForMember(x => x.Image, options => options.Ignore());

            CreateMap<CategoryDTO, Category>().ReverseMap();
            CreateMap<CategoryCreateDTO, Category>();

            CreateMap<StudentCreateDTO, Student>()
                 .ForMember(x => x.Image, options => { options.Ignore(); })
                 .ForMember(x => x.StudentCourses, options => options.MapFrom(MapStudentCourses))
                 .ForMember(x => x.StudentCategories, options => options.MapFrom(MapStudentCategories));

              CreateMap<Student, StudentDTO>()
                  .ForMember(x => x.Courses, options => options.MapFrom(MapStudentCourses))
                  .ForMember(x => x.Categories, options => options.MapFrom(MapStudentCategories));
            CreateMap<IdentityUser, UserDTO>();

        }

        private List<StudentCourse> MapStudentCourses(StudentCreateDTO studentCreateDTO, Student student)
        {
            var result = new List<StudentCourse>();
            if (studentCreateDTO.CourseIds == null) { return result; }
            foreach (var id in studentCreateDTO.CourseIds)
            {
                result.Add(new StudentCourse { CourseId = id });
            }
            return result;
        }

        private List<StudentCategory> MapStudentCategories(StudentCreateDTO studentCreateDTO, Student student)
        {
            var result = new List<StudentCategory>();
            if (studentCreateDTO.CategoryIds == null) { return result; }
            foreach (var id in studentCreateDTO.CategoryIds)
            {
                result.Add(new StudentCategory { CategoryId = id });
            }
            return result;

        }
        private List<CourseDTO> MapStudentCourses(Student student, StudentDTO studentDTO)
        {
            var result = new List<CourseDTO>();
            if (student.StudentCourses != null)
            {
                foreach (var course in student.StudentCourses)
                {
                    result.Add(new CourseDTO { Id = course.CourseId, Name = course.Course.Name });
                }
            }
            return result;
        }
        private List<CategoryDTO> MapStudentCategories(Student student, StudentDTO studentDTO)
        {
            var result = new List<CategoryDTO>();
            if (student.StudentCategories != null)
            {
                foreach (var category in student.StudentCategories)
                {
                    result.Add(new CategoryDTO { Id = category.CategoryId, Name = category.Category.Name });
                }
            }
            return result;
        }
    }
}


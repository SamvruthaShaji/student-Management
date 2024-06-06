
using Microsoft.AspNetCore.Mvc.Filters;

namespace StudentProject.Filters
{
    public class StudentExceptionFilter : ExceptionFilterAttribute
    {
        private readonly ILogger<StudentExceptionFilter> logger;

        public StudentExceptionFilter(ILogger<StudentExceptionFilter> logger)
        {
            this.logger = logger;
        }
        public override void OnException(ExceptionContext context)
        {
            logger.LogError(context.Exception, context.Exception.Message);
            base.OnException(context);
        }
    }
}

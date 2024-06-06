using Microsoft.EntityFrameworkCore;

namespace StudentProject.Helpers
{
    public static class HTTPContextExtensions
    {
        public async static Task InsertParameterPaginationHeader<T>(this HttpContext httpcontext, IQueryable<T> queryable)
        {
            if (httpcontext == null) { throw new ArgumentNullException(nameof(httpcontext)); }
            double count = await queryable.CountAsync();
            httpcontext.Response.Headers.Add("recordCount",count.ToString());
        }
    }
}

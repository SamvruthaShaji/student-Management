namespace StudentProject.Helpers
{
    public class ApplicationStorageService : IFileStorageServices
    {
        private readonly IWebHostEnvironment env;
        private readonly IHttpContextAccessor httpContextAccessor;

        public ApplicationStorageService(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            this.env = env;
            this.httpContextAccessor = httpContextAccessor;
        }
        public Task DeleteFile(string containerName, string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return Task.CompletedTask;
            }
            var fileName = Path.GetFileName(filePath);
            var fileDirectory = Path.Combine(containerName, fileName);
            if (File.Exists(fileDirectory))
            {
                File.Delete(fileDirectory);
            }
            return Task.CompletedTask;
        }

        public async Task<string> EditFile(string containerName, IFormFile file, string filePath)
        {
            await DeleteFile(containerName, filePath);
            return await SaveFile(containerName, file);
        }
        public async Task<string> SaveFile(string containerName, IFormFile file)
        {
            var extension = Path.GetExtension(file.Name);
            var fileName = $"{Guid.NewGuid()}{extension}";
            string folder = Path.Combine(env.WebRootPath, containerName);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            string filePath = Path.Combine(folder, fileName);
            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                var content = ms.ToArray();
                await File.WriteAllBytesAsync(filePath, content);
            }
            var url = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}";
            var dbFilePath = Path.Combine(url, containerName, fileName).Replace("\\", "/");
            return dbFilePath;
        }
    }
}



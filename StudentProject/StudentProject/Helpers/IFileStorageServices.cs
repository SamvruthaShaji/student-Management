namespace StudentProject.Helpers
{
    public interface IFileStorageServices
    {
        Task DeleteFile(string containerName, string fileRoute);
        Task<string> SaveFile(string containerName, IFormFile file);
        Task<string> EditFile(string containerName, IFormFile file, string fileRoute);
        
    }
}

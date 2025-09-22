using FullStackAssignment.Application.IReposetories;


namespace FullStackAssignment.IntegrationTests
{
    public class TestFileRepo : IFileRepo
        {
            private readonly string _folderPath;

            public TestFileRepo(string folderPath) => _folderPath = folderPath;

            public async Task<string> SaveProductImageAsync(byte[] imageBytes, string extension)
            {
                var path = Path.Combine(_folderPath, $"{Guid.NewGuid()}{extension}");
                await File.WriteAllBytesAsync(path, imageBytes);
                return path;
            }

            public Task DeleteOldImageAsync(string filePath)
            {
                if (File.Exists(filePath))
                    File.Delete(filePath);
                return Task.CompletedTask;
            }
        }
}


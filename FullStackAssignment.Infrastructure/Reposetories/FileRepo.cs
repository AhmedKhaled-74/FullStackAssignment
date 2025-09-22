using FullStackAssignment.Application.IReposetories;
using FullStackAssignment.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullStackAssignment.Infrastructure.Reposetories
{
    public class FileRepo : IFileRepo
    {
        private readonly string _wwwRootPath;

        public FileRepo(string wwwRootPath)
        {
            _wwwRootPath = wwwRootPath;
        }

        public async Task<string> SaveProductImageAsync(byte[] content, string extension)
        {
            var folder = Path.Combine(_wwwRootPath, "images", "products");
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(folder, fileName);

            await File.WriteAllBytesAsync(filePath, content);
            return $"/images/products/{fileName}";
        }

        public async Task DeleteOldImageAsync(string imageUrl)
        {
            if (!string.IsNullOrEmpty(imageUrl))
            {
                var filePath = Path.Combine(_wwwRootPath, imageUrl.TrimStart('/'));
                if (File.Exists(filePath))
                {
                    await Task.Run(() => File.Delete(filePath));
                }
            }
        }



    }

}

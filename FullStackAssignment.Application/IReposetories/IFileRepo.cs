using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullStackAssignment.Application.IReposetories
{
    public interface IFileRepo
    {
        Task<string> SaveProductImageAsync(byte[] content, string extension);  
        Task DeleteOldImageAsync(string imageUrl);  
    }
}

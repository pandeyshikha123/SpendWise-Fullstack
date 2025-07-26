using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace SpendWiseAPI.Services.Interfaces
{
    public interface IImageService
    {
        Task<string?> UploadImageAsync(IFormFile file);
    }
}

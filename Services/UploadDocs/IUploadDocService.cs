namespace RestauranteAPI.Services
{
   public interface IUploadDocService
   {
        Task<string> UploadImageAsync(IFormFile file);
        Task<string> UploadPDFAsync(IFormFile file);
        Task DeleteImageAsync(string publicId);
        Task DeleteDocAsync(string publicId);
   }
}

using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace SaviaVetAPI.Services
{
    public class CloudinaryImageService : IImageService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryImageService(IConfiguration configuration)
        {
            var cloudName = configuration["CloudinarySettings:CloudName"];
            var apiKey = configuration["CloudinarySettings:ApiKey"];
            var apiSecret = configuration["CloudinarySettings:ApiSecret"];
            var account = new Account(cloudName, apiKey, apiSecret);
            _cloudinary = new Cloudinary(account);
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Transformation = new Transformation().Width(800).Height(800).Crop("limit")
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.Error != null)
            {
                throw new Exception($"Cloudinary upload error: {uploadResult.Error.Message}");
            }

            return uploadResult.SecureUrl?.ToString() ?? string.Empty;
        }
    }
}

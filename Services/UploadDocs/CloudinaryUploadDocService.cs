
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace RestauranteAPI.Services
{
    public class CloudinaryUploadDocService: IUploadDocService
    {
        private readonly Cloudinary _cloudinary;

       public CloudinaryUploadDocService(IConfiguration configuration)
       {
           var cloudName = configuration["CloudinarySettings:CloudName"];
           var apiKey = configuration["CloudinarySettings:ApiKey"];
           var apiSecret = configuration["CloudinarySettings:ApiSecret"];
           var account = new Account(cloudName, apiKey, apiSecret);
           _cloudinary = new Cloudinary(account);
       }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Archivo de imagen inválido.");

            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
           {
               File = new FileDescription(file.FileName, stream),
               Transformation = new Transformation().Width(400).Height(400).Crop("fill")
               //Transformation = new Transformation().Width(300).Crop("scale").Chain().Effect("cartoonify")
               //Effect(vignette",20), Effect("remove_background", 0.5), Effect("sharpen", 50)  
           };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return uploadResult.SecureUrl?.ToString();
        }

          public async Task<string> UploadPDFAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Archivo PDF inválido.");

            using var stream = file.OpenReadStream();
            var uploadParams = new RawUploadParams
           {
               File = new FileDescription(file.FileName, stream)
           };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return uploadResult.SecureUrl?.ToString();
        }

        public async Task DeleteImageAsync(string publicId)
        {
            if (string.IsNullOrWhiteSpace(publicId)) return;
            await _cloudinary.DestroyAsync(new DeletionParams(publicId));
        }

        public async Task DeleteDocAsync(string publicId)
        {
            if (string.IsNullOrWhiteSpace(publicId)) return;
            await _cloudinary.DestroyAsync(new DeletionParams(publicId)
            {
                ResourceType = ResourceType.Raw
            });
        }
    }
}

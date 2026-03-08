using System;
using System.ComponentModel.DataAnnotations;

namespace SaviaVetAPI.DTOs
{
    public class GetPetDTO
    {
        public int Pet_id { get; set; }
        public string Name { get; set; } = "";
        public string Species { get; set; } = "";
        public string? Breed { get; set; }
        public DateTime? Birth_date { get; set; }
        public string? Photo_url { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; } = ""; 
        public int? Owner_id { get; set; }
    }

    public class AddPetDTO
    {
        [Required]
        public required string Name { get; set; }
        [Required]
        public required string Species { get; set; }
        public string? Breed { get; set; }
        public DateTime? Birth_date { get; set; }

        [Required(ErrorMessage = "Debe proporcionar una imagen.")]
        public IFormFile Imagen { get; set; }

        // Se rellena en backend tras subir la imagen a Cloudinary.
        public string? Photo_url { get; set; }
        public string? Description { get; set; }
        [Required]
        public required string Status { get; set; }
        public int? Owner_id { get; set; }
    }

    public class UpdatePetDTO
    {
        [Range(1, int.MaxValue)]
        public required int Pet_id { get; set; }
        public string? Name { get; set; }
        public string? Species { get; set; }
        public string? Breed { get; set; }
        public DateTime? Birth_date { get; set; }
        public string? Photo_url { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public int? Owner_id { get; set; } 
    }
}

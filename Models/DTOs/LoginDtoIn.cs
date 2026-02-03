using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SaviaVetAPI.DTOs;

public class LoginDtoIn
    {
        [Required]
        public string Email { get; set; }
        [Required]
        [StringLength(15, ErrorMessage = "La contraseña debe ser de 15 caracteres")]
        public string Password { get; set; }
}


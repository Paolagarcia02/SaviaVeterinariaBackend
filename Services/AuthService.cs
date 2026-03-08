using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SaviaVetAPI.DTOs;
using SaviaVetAPI.Repositories;

namespace SaviaVetAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _repository;

        public AuthService(IConfiguration configuration, IUserRepository repository)
        {
            _configuration = configuration;
            _repository = repository;
        }

        // 1. LOGIN
        public async Task<LoginResponseDTO?> Login(LoginDtoIn loginDtoIn) 
        {
            var user = await _repository.GetUserByCredentialsAsync(loginDtoIn);
            
            if (user == null) 
            {
                return null; 
            }

            var token = GenerateToken(user);

            return new LoginResponseDTO 
            {
                Token = token,
                Role = user.Role,
                UserId = user.UserId,
                FranchiseId = user.FranchiseId
            };
        }

        // 2. REGISTRO
        public async Task<string> Register(UserDtoIn userDtoIn) 
        {
            var user = await _repository.RegisterUserAsync(userDtoIn);
            
            return GenerateToken(user);
        }

        // 3. GENERAR TOKEN
        public string GenerateToken(UserDtoOut userDtoOut) 
        {
            // IMPORTANTE: Asegúrate de que en tu appsettings.json tienes "Jwt:Key"
            // Si usas "JWT:SecretKey" cambia el string de abajo para que coincida.
            var keyString = _configuration["JWT:SecretKey"]; 
            var key = Encoding.UTF8.GetBytes(keyString); 

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userDtoOut.UserId.ToString()),
                new Claim(ClaimTypes.Name, userDtoOut.UserName),
                new Claim(ClaimTypes.Role, userDtoOut.Role),
                new Claim(ClaimTypes.Email, userDtoOut.Email)
            };

            if (userDtoOut.FranchiseId.HasValue)
            {
                claims.Add(new Claim("franchise_id", userDtoOut.FranchiseId.Value.ToString()));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _configuration["JWT:ValidIssuer"],
                // Para quién es el token (SaviaVetFrontend)
                Audience = _configuration["JWT:ValidAudience"],
                Subject = new ClaimsIdentity(claims),
                
                // Duración del token (ej: 24 horas)
                Expires = DateTime.UtcNow.AddHours(24), 
                
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), 
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        } 

        // 4. VERIFICAR ACCESO (Seguridad)
        public bool HasAccessToResource(int requestedUserID, ClaimsPrincipal user) 
        {
            var userIdClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (userIdClaim is null || !int.TryParse(userIdClaim.Value, out int currentUserId)) 
            { 
                return false; 
            }

            // REGLA 1: ¿Es el dueño del recurso? (Ej: Yo consulto mi propio perfil)
            var isOwnResource = (currentUserId == requestedUserID);

            // REGLA 2: ¿Es Administrador?
            var roleClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
            bool isAdmin = false;
            
            if (roleClaim != null)
            {
                // Compara esto con cómo guardas el rol en BDD ("Admin", "Administrator", etc)
                isAdmin = roleClaim.Value == "Admin"; 
            }
            
            return isOwnResource || isAdmin;
        }
    }
}

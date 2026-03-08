using MySql.Data.MySqlClient;
using SaviaVetAPI.DTOs;
using SaviaVetAPI.Exceptions;
using Microsoft.Extensions.Configuration;
using System.Data.Common;


namespace SaviaVetAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SaviaVetDB") ?? "";
        }

        public async Task<List<GetUserDTO>> GetUsersAsync()
        {
            List<GetUserDTO> list = new List<GetUserDTO>();
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT User_id, Full_name, Email, Role, Franchise_id FROM User";
                using (var command = new MySqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var item = new GetUserDTO
                            {
                                User_id = reader.GetInt32(0),
                                Full_name = reader.GetString(1),
                                Email = reader.GetString(2),
                                Role = reader.GetString(3),
                                Franchise_id = reader.IsDBNull(4) ? null : reader.GetInt32(4)
                            };
                            list.Add(item);
                        }
                    }
                }
            }
            return list;
        }

        public async Task<GetUserDTO> GetOneUserAsync(int id)
        {
            GetUserDTO item = null;
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT User_id, Full_name, Email, Role, Franchise_id FROM User WHERE User_id = @Id";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            item = new GetUserDTO
                            {
                                User_id = reader.GetInt32(0),
                                Full_name = reader.GetString(1),
                                Email = reader.GetString(2),
                                Role = reader.GetString(3),
                                Franchise_id = reader.IsDBNull(4) ? null : reader.GetInt32(4)
                            };
                        }
                    }
                }
            }
            return item;
        }



        public async Task<bool> AddUserAsync(AddUserDTO dto)
        {
            bool bRet = true;
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "INSERT INTO User (Full_name, Email, Password_hash, Role, Franchise_id) VALUES (@Full_name, @Email, @Password, @Role, @Franchise_id)";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Full_name", dto.Full_name);
                    command.Parameters.AddWithValue("@Email", dto.Email);
                    command.Parameters.AddWithValue("@Password", dto.Password);
                    command.Parameters.AddWithValue("@Role", dto.Role);
                    command.Parameters.AddWithValue("@Franchise_id", (object)dto.Franchise_id ?? DBNull.Value);

                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    if (rowsAffected != 1)
                    {
                        bRet = false;
                        if (rowsAffected > 1) throw new MoreThanOneRowException();
                    }
                }
            }
            return bRet;
        }

        public async Task<bool> UpdateUserAsync(UpdateUserDTO dto)
        {
            bool bRet = true;
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "UPDATE User SET ";

                if (!string.IsNullOrEmpty(dto.Full_name)) query += "Full_name = @Full_name , ";
                if (!string.IsNullOrEmpty(dto.Email)) query += "Email = @Email , ";
                if (!string.IsNullOrEmpty(dto.Role)) query += "Role = @Role , ";
                if (dto.Franchise_id != null) query += "Franchise_id = @Franchise_id , ";

                query += " User_id = User_id WHERE User_id = @Id ;";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", dto.User_id);
                    if (!string.IsNullOrEmpty(dto.Full_name)) command.Parameters.AddWithValue("@Full_name", dto.Full_name);
                    if (!string.IsNullOrEmpty(dto.Email)) command.Parameters.AddWithValue("@Email", dto.Email);
                    if (!string.IsNullOrEmpty(dto.Role)) command.Parameters.AddWithValue("@Role", dto.Role);
                    if (dto.Franchise_id != null) command.Parameters.AddWithValue("@Franchise_id", dto.Franchise_id);

                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    if (rowsAffected != 1)
                    {
                        bRet = false;
                        if (rowsAffected > 1) throw new MoreThanOneRowException();
                    }
                }
            }
            return bRet;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            bool bRet = true;
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "DELETE FROM User WHERE User_id = @Id";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    if (rowsAffected != 1)
                    {
                        bRet = false;
                        if (rowsAffected > 1) throw new MoreThanOneRowException();
                    }
                }
            }
            return bRet;
        }

        public async Task<UserDtoOut> GetUserByCredentialsAsync(LoginDtoIn loginDto)
        {
            UserDtoOut user = null;

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                // NOTA: Asumo que en BDD tienes 'Password_hash'. 
                // En un caso real, aquí deberías comparar hashes, no texto plano.
                string query = "SELECT User_id, Full_name, Email, Role, Franchise_id FROM User WHERE Email = @Email AND Password_hash = @Password";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", loginDto.Email);
                    command.Parameters.AddWithValue("@Password", loginDto.Password);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            user = new UserDtoOut
                            {
                                UserId = reader.GetInt32(0),
                                UserName = reader.GetString(1),
                                Email = reader.GetString(2),
                                Role = reader.GetString(3),
                                FranchiseId = reader.IsDBNull(4) ? null : reader.GetInt32(4)
                            };
                        }
                    }
                }
            }
            return user; 
        }

        public async Task<UserDtoOut> RegisterUserAsync(UserDtoIn userDto)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = @"INSERT INTO User (Full_name, Email, Password_hash, Role) 
                         VALUES (@Name, @Email, @Pass, 'User');
                         SELECT LAST_INSERT_ID();"; 

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", userDto.UserName);
                    command.Parameters.AddWithValue("@Email", userDto.Email);
                    command.Parameters.AddWithValue("@Pass", userDto.Password);

                    var newId = Convert.ToInt32(await command.ExecuteScalarAsync());

                    return new UserDtoOut
                    {
                        UserId = newId,
                        UserName = userDto.UserName,
                        Email = userDto.Email,
                        Role = "User"
                    };
                }
            }
        }
    }


}

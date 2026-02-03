using MySql.Data.MySqlClient;
using SaviaVetAPI.DTOs;
using SaviaVetAPI.Exceptions;
using Microsoft.Extensions.Configuration;

namespace SaviaVetAPI.Repositories
{
    public class AdoptionApplicationRepository : IAdoptionApplicationRepository
    {
        private readonly string _connectionString;

        public AdoptionApplicationRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SaviaVetdb") ?? "";
        }

        public async Task<List<GetAdoptionApplicationDTO>> GetAdoptionApplicationsAsync()
        {
            List<GetAdoptionApplicationDTO> list = new List<GetAdoptionApplicationDTO>();
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT Application_id, User_id, Pet_id, Message, Status, Application_date FROM Adoption_application";
                using (var command = new MySqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var item = new GetAdoptionApplicationDTO
                            {
                                Application_id = reader.GetInt32(0),
                                User_id = reader.GetInt32(1),
                                Pet_id = reader.GetInt32(2),
                                Message = reader.IsDBNull(3) ? null : reader.GetString(3),
                                Status = reader.GetString(4),
                                Application_date = reader.GetDateTime(5)
                            };
                            list.Add(item);
                        }
                    }
                }
            }
            return list;
        }

        public async Task<GetAdoptionApplicationDTO> GetOneAdoptionApplicationAsync(int id)
        {
            GetAdoptionApplicationDTO item = null;
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT Application_id, User_id, Pet_id, Message, Status, Application_date FROM Adoption_application WHERE Application_id = @Id";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            item = new GetAdoptionApplicationDTO
                            {
                                Application_id = reader.GetInt32(0),
                                User_id = reader.GetInt32(1),
                                Pet_id = reader.GetInt32(2),
                                Message = reader.IsDBNull(3) ? null : reader.GetString(3),
                                Status = reader.GetString(4),
                                Application_date = reader.GetDateTime(5)
                            };
                        }
                    }
                }
            }
            return item;
        }

        public async Task<bool> AddAdoptionApplicationAsync(AddAdoptionApplicationDTO dto)
        {
            bool bRet = true;
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "INSERT INTO Adoption_application (User_id, Pet_id, Message) VALUES (@User_id, @Pet_id, @Message)";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@User_id", dto.User_id);
                    command.Parameters.AddWithValue("@Pet_id", dto.Pet_id);
                    command.Parameters.AddWithValue("@Message", (object)dto.Message ?? DBNull.Value);
                    
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

        public async Task<bool> UpdateAdoptionApplicationAsync(UpdateAdoptionApplicationDTO dto)
        {
            bool bRet = true;
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "UPDATE Adoption_application SET "; 

                if (!string.IsNullOrEmpty(dto.Status)) query += "Status = @Status , ";

                query += " Application_id = Application_id WHERE Application_id = @Id ;";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", dto.Application_id);
                    if (!string.IsNullOrEmpty(dto.Status)) command.Parameters.AddWithValue("@Status", dto.Status);

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

        public async Task<bool> DeleteAdoptionApplicationAsync(int id)
        {
            bool bRet = true;
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "DELETE FROM Adoption_application WHERE Application_id = @Id";
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
    }
}
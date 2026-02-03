using MySql.Data.MySqlClient;
using SaviaVetAPI.DTOs;
using SaviaVetAPI.Exceptions;
using Microsoft.Extensions.Configuration;

namespace SaviaVetAPI.Repositories
{
    public class PetRepository : IPetRepository
    {
        private readonly string _connectionString;

        public PetRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SaviaVetdb") ?? "";
        }

        public async Task<List<GetPetDTO>> GetPetsAsync()
        {
            List<GetPetDTO> list = new List<GetPetDTO>();
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT Pet_id, Name, Species, Breed, Birth_date, Photo_url, Description, Status, Owner_id FROM Pet";
                using (var command = new MySqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var item = new GetPetDTO
                            {
                                Pet_id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Species = reader.GetString(2),
                                Breed = reader.IsDBNull(3) ? null : reader.GetString(3),
                                Birth_date = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
                                Photo_url = reader.IsDBNull(5) ? null : reader.GetString(5),
                                Description = reader.IsDBNull(6) ? null : reader.GetString(6),
                                Status = reader.GetString(7),
                                Owner_id = reader.IsDBNull(8) ? null : reader.GetInt32(8)
                            };
                            list.Add(item);
                        }
                    }
                }
            }
            return list;
        }

        public async Task<GetPetDTO> GetOnePetAsync(int id)
        {
            GetPetDTO item = null;
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT Pet_id, Name, Species, Breed, Birth_date, Photo_url, Description, Status, Owner_id FROM Pet WHERE Pet_id = @Id";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            item = new GetPetDTO
                            {
                                Pet_id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Species = reader.GetString(2),
                                Breed = reader.IsDBNull(3) ? null : reader.GetString(3),
                                Birth_date = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
                                Photo_url = reader.IsDBNull(5) ? null : reader.GetString(5),
                                Description = reader.IsDBNull(6) ? null : reader.GetString(6),
                                Status = reader.GetString(7),
                                Owner_id = reader.IsDBNull(8) ? null : reader.GetInt32(8)
                            };
                        }
                    }
                }
            }
            return item;
        }

        public async Task<bool> AddPetAsync(AddPetDTO dto)
        {
            bool bRet = true;
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "INSERT INTO Pet (Name, Species, Breed, Birth_date, Photo_url, Description, Status, Owner_id) VALUES (@Name, @Species, @Breed, @Birth_date, @Photo_url, @Description, @Status, @Owner_id)";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", dto.Name);
                    command.Parameters.AddWithValue("@Species", dto.Species);
                    command.Parameters.AddWithValue("@Breed", (object)dto.Breed ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Birth_date", (object)dto.Birth_date ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Photo_url", (object)dto.Photo_url ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Description", (object)dto.Description ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Status", dto.Status);
                    command.Parameters.AddWithValue("@Owner_id", (object)dto.Owner_id ?? DBNull.Value);
                    
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

        public async Task<bool> UpdatePetAsync(UpdatePetDTO dto)
        {
            bool bRet = true;
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "UPDATE Pet SET "; 

                if (!string.IsNullOrEmpty(dto.Name)) query += "Name = @Name , ";
                if (!string.IsNullOrEmpty(dto.Species)) query += "Species = @Species , ";
                if (!string.IsNullOrEmpty(dto.Breed)) query += "Breed = @Breed , ";
                if (dto.Birth_date != null) query += "Birth_date = @Birth_date , ";
                if (!string.IsNullOrEmpty(dto.Photo_url)) query += "Photo_url = @Photo_url , ";
                if (!string.IsNullOrEmpty(dto.Description)) query += "Description = @Description , ";
                if (!string.IsNullOrEmpty(dto.Status)) query += "Status = @Status , ";
                if (dto.Owner_id != null) query += "Owner_id = @Owner_id , ";

                query += " Pet_id = Pet_id WHERE Pet_id = @Id ;";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", dto.Pet_id);
                    if (!string.IsNullOrEmpty(dto.Name)) command.Parameters.AddWithValue("@Name", dto.Name);
                    if (!string.IsNullOrEmpty(dto.Species)) command.Parameters.AddWithValue("@Species", dto.Species);
                    if (!string.IsNullOrEmpty(dto.Breed)) command.Parameters.AddWithValue("@Breed", dto.Breed);
                    if (dto.Birth_date != null) command.Parameters.AddWithValue("@Birth_date", dto.Birth_date);
                    if (!string.IsNullOrEmpty(dto.Photo_url)) command.Parameters.AddWithValue("@Photo_url", dto.Photo_url);
                    if (!string.IsNullOrEmpty(dto.Description)) command.Parameters.AddWithValue("@Description", dto.Description);
                    if (!string.IsNullOrEmpty(dto.Status)) command.Parameters.AddWithValue("@Status", dto.Status);
                    if (dto.Owner_id != null) command.Parameters.AddWithValue("@Owner_id", dto.Owner_id);

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

        public async Task<bool> DeletePetAsync(int id)
        {
            bool bRet = true;
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "DELETE FROM Pet WHERE Pet_id = @Id";
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
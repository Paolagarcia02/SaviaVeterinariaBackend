using MySql.Data.MySqlClient;
using SaviaVetAPI.DTOs;
using SaviaVetAPI.Exceptions;
using Microsoft.Extensions.Configuration;

namespace SaviaVetAPI.Repositories
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly string _connectionString;

       public ServiceRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SaviaVetDB") ?? "";
        }

        public async Task<List<GetServiceDTO>> GetServicesAsync()
        {
            List<GetServiceDTO> list = new List<GetServiceDTO>();
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT Service_id, Name, Description, Icon FROM Service";
                using (var command = new MySqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var item = new GetServiceDTO
                            {
                                Service_id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Description = reader.GetString(2),
                                Icon = reader.IsDBNull(3) ? null : reader.GetString(3)
                            };
                            list.Add(item);
                        }
                    }
                }
            }
            return list;
        }

        public async Task<GetServiceDTO> GetOneServiceAsync(int id)
        {
            GetServiceDTO item = null;
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT Service_id, Name, Description, Icon FROM Service WHERE Service_id = @Id";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            item = new GetServiceDTO
                            {
                                Service_id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Description = reader.GetString(2),
                                Icon = reader.IsDBNull(3) ? null : reader.GetString(3)
                            };
                        }
                    }
                }
            }
            return item;
        }

        public async Task<bool> AddServiceAsync(AddServiceDTO dto)
        {
            bool bRet = true;
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "INSERT INTO Service (Name, Description, Icon) VALUES (@Name, @Description, @Icon)";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", dto.Name);
                    command.Parameters.AddWithValue("@Description", dto.Description);
                    command.Parameters.AddWithValue("@Icon", (object)dto.Icon ?? DBNull.Value);
                    
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

        public async Task<bool> UpdateServiceAsync(UpdateServiceDTO dto)
        {
            bool bRet = true;
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "UPDATE Service SET "; 

                if (!string.IsNullOrEmpty(dto.Name)) query += "Name = @Name , ";
                if (!string.IsNullOrEmpty(dto.Description)) query += "Description = @Description , ";
                if (!string.IsNullOrEmpty(dto.Icon)) query += "Icon = @Icon , ";

                query += " Service_id = Service_id WHERE Service_id = @Id ;";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", dto.Service_id);
                    if (!string.IsNullOrEmpty(dto.Name)) command.Parameters.AddWithValue("@Name", dto.Name);
                    if (!string.IsNullOrEmpty(dto.Description)) command.Parameters.AddWithValue("@Description", dto.Description);
                    if (!string.IsNullOrEmpty(dto.Icon)) command.Parameters.AddWithValue("@Icon", dto.Icon);

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

        public async Task<bool> DeleteServiceAsync(int id)
        {
            bool bRet = true;
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "DELETE FROM Service WHERE Service_id = @Id";
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
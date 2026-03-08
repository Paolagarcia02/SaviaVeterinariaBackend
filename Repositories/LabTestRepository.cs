using MySql.Data.MySqlClient;
using SaviaVetAPI.DTOs;
using SaviaVetAPI.Exceptions;
using Microsoft.Extensions.Configuration;

namespace SaviaVetAPI.Repositories
{
    public class LabTestRepository : ILabTestRepository
    {
        private readonly string _connectionString;

        public LabTestRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SaviaVetdb") ?? "";
        }

        public async Task<List<GetLabTestDTO>> GetLabTestsAsync()
        {
            List<GetLabTestDTO> list = new List<GetLabTestDTO>();
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT Test_id, Appointment_id, Test_type, Result_data, Comments, Status, Requested_at, Completed_at FROM Lab_test";
                using (var command = new MySqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var item = new GetLabTestDTO
                            {
                                Test_id = reader.GetInt32(0),
                                Appointment_id = reader.GetInt32(1),
                                Test_type = reader.GetString(2),
                                Result_data = reader.IsDBNull(3) ? null : reader.GetString(3),
                                Comments = reader.IsDBNull(4) ? null : reader.GetString(4),
                                Status = reader.GetString(5),
                                Requested_at = reader.GetDateTime(6),
                                Completed_at = reader.IsDBNull(7) ? null : reader.GetDateTime(7)
                            };
                            list.Add(item);
                        }
                    }
                }
            }
            return list;
        }

        public async Task<List<GetLabTestDTO>> GetLabTestsByOwnerIdAsync(int ownerId)
        {
            List<GetLabTestDTO> list = new List<GetLabTestDTO>();
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = @"SELECT lt.Test_id, lt.Appointment_id, lt.Test_type, lt.Result_data, lt.Comments, lt.Status, lt.Requested_at, lt.Completed_at
                                 FROM Lab_test lt
                                 INNER JOIN Appointment a ON a.Appointment_id = lt.Appointment_id
                                 INNER JOIN Pet p ON p.Pet_id = a.Pet_id
                                 WHERE p.Owner_id = @Owner_id";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Owner_id", ownerId);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var item = new GetLabTestDTO
                            {
                                Test_id = reader.GetInt32(0),
                                Appointment_id = reader.GetInt32(1),
                                Test_type = reader.GetString(2),
                                Result_data = reader.IsDBNull(3) ? null : reader.GetString(3),
                                Comments = reader.IsDBNull(4) ? null : reader.GetString(4),
                                Status = reader.GetString(5),
                                Requested_at = reader.GetDateTime(6),
                                Completed_at = reader.IsDBNull(7) ? null : reader.GetDateTime(7)
                            };
                            list.Add(item);
                        }
                    }
                }
            }
            return list;
        }

        public async Task<GetLabTestDTO> GetOneLabTestAsync(int id)
        {
            GetLabTestDTO item = null;
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT Test_id, Appointment_id, Test_type, Result_data, Comments, Status, Requested_at, Completed_at FROM Lab_test WHERE Test_id = @Id";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            item = new GetLabTestDTO
                            {
                                Test_id = reader.GetInt32(0),
                                Appointment_id = reader.GetInt32(1),
                                Test_type = reader.GetString(2),
                                Result_data = reader.IsDBNull(3) ? null : reader.GetString(3),
                                Comments = reader.IsDBNull(4) ? null : reader.GetString(4),
                                Status = reader.GetString(5),
                                Requested_at = reader.GetDateTime(6),
                                Completed_at = reader.IsDBNull(7) ? null : reader.GetDateTime(7)
                            };
                        }
                    }
                }
            }
            return item;
        }

        public async Task<bool> AddLabTestAsync(AddLabTestDTO dto)
        {
            bool bRet = true;
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "INSERT INTO Lab_test (Appointment_id, Test_type) VALUES (@Appointment_id, @Test_type)";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Appointment_id", dto.Appointment_id);
                    command.Parameters.AddWithValue("@Test_type", dto.Test_type);
                    
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

        public async Task<bool> UpdateLabTestAsync(UpdateLabTestDTO dto)
        {
            bool bRet = true;
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "UPDATE Lab_test SET "; 

                if (!string.IsNullOrEmpty(dto.Result_data)) query += "Result_data = @Result_data , ";
                if (!string.IsNullOrEmpty(dto.Comments)) query += "Comments = @Comments , ";
                if (!string.IsNullOrEmpty(dto.Status)) query += "Status = @Status , ";
                if (dto.Completed_at != null) query += "Completed_at = @Completed_at , ";

                query += " Test_id = Test_id WHERE Test_id = @Id ;";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", dto.Test_id);
                    if (!string.IsNullOrEmpty(dto.Result_data)) command.Parameters.AddWithValue("@Result_data", dto.Result_data);
                    if (!string.IsNullOrEmpty(dto.Comments)) command.Parameters.AddWithValue("@Comments", dto.Comments);
                    if (!string.IsNullOrEmpty(dto.Status)) command.Parameters.AddWithValue("@Status", dto.Status);
                    if (dto.Completed_at != null) command.Parameters.AddWithValue("@Completed_at", dto.Completed_at);

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

        public async Task<bool> DeleteLabTestAsync(int id)
        {
            bool bRet = true;
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "DELETE FROM Lab_test WHERE Test_id = @Id";
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

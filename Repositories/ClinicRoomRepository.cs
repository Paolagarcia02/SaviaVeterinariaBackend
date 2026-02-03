using MySql.Data.MySqlClient;
using SaviaVetAPI.DTOs;
using SaviaVetAPI.Exceptions;
using Microsoft.Extensions.Configuration;

namespace SaviaVetAPI.Repositories
{
    public class ClinicRoomRepository : IClinicRoomRepository
    {
        private readonly string _connectionString;

        public ClinicRoomRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SaviaVetdb") ?? "";
        }

        public async Task<List<GetClinicRoomDTO>> GetClinicRoomsAsync()
        {
            List<GetClinicRoomDTO> list = new List<GetClinicRoomDTO>();
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT Room_id, Franchise_id, Name, Room_type, Is_active FROM Clinic_room";
                using (var command = new MySqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var item = new GetClinicRoomDTO
                            {
                                Room_id = reader.GetInt32(0),
                                Franchise_id = reader.GetInt32(1),
                                Name = reader.GetString(2),
                                Room_type = reader.GetString(3),
                                Is_active = reader.GetBoolean(4)
                            };
                            list.Add(item);
                        }
                    }
                }
            }
            return list;
        }

        public async Task<GetClinicRoomDTO> GetOneClinicRoomAsync(int id)
        {
            GetClinicRoomDTO item = null;
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT Room_id, Franchise_id, Name, Room_type, Is_active FROM Clinic_room WHERE Room_id = @Id";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            item = new GetClinicRoomDTO
                            {
                                Room_id = reader.GetInt32(0),
                                Franchise_id = reader.GetInt32(1),
                                Name = reader.GetString(2),
                                Room_type = reader.GetString(3),
                                Is_active = reader.GetBoolean(4)
                            };
                        }
                    }
                }
            }
            return item;
        }

        public async Task<bool> AddClinicRoomAsync(AddClinicRoomDTO dto)
        {
            bool bRet = true;
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "INSERT INTO Clinic_room (Franchise_id, Name, Room_type, Is_active) VALUES (@Franchise_id, @Name, @Room_type, 1)";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Franchise_id", dto.Franchise_id);
                    command.Parameters.AddWithValue("@Name", dto.Name);
                    command.Parameters.AddWithValue("@Room_type", dto.Room_type);
                    
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

        public async Task<bool> UpdateClinicRoomAsync(UpdateClinicRoomDTO dto)
        {
            bool bRet = true;
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "UPDATE Clinic_room SET "; 

                if (!string.IsNullOrEmpty(dto.Name)) query += "Name = @Name , ";
                if (!string.IsNullOrEmpty(dto.Room_type)) query += "Room_type = @Room_type , ";
                if (dto.Is_active != null) query += "Is_active = @Is_active , ";

                query += " Room_id = Room_id WHERE Room_id = @Id ;";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", dto.Room_id);
                    if (!string.IsNullOrEmpty(dto.Name)) command.Parameters.AddWithValue("@Name", dto.Name);
                    if (!string.IsNullOrEmpty(dto.Room_type)) command.Parameters.AddWithValue("@Room_type", dto.Room_type);
                    if (dto.Is_active != null) command.Parameters.AddWithValue("@Is_active", dto.Is_active);

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

        public async Task<bool> DeleteClinicRoomAsync(int id)
        {
            bool bRet = true;
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "DELETE FROM Clinic_room WHERE Room_id = @Id";
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
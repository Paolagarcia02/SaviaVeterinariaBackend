using MySql.Data.MySqlClient;
using SaviaVetAPI.DTOs;
using SaviaVetAPI.Exceptions;
using Microsoft.Extensions.Configuration;

namespace SaviaVetAPI.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly string _connectionString;

        public AppointmentRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SaviaVetdb") ?? "";
        }

        public async Task<List<GetAppointmentDTO>> GetAppointmentsAsync()
        {
            List<GetAppointmentDTO> list = new List<GetAppointmentDTO>();
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT Appointment_id, Date_time, Duration_minutes, Reason, Status, Notes, Pet_id, Vet_id, Franchise_id, Room_id FROM Appointment";
                using (var command = new MySqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var item = new GetAppointmentDTO
                            {
                                Appointment_id = reader.GetInt32(0),
                                Date_time = reader.IsDBNull(1) ? null : reader.GetDateTime(1),
                                Duration_minutes = reader.GetInt32(2),
                                Reason = reader.GetString(3),
                                Status = reader.GetString(4),
                                Notes = reader.IsDBNull(5) ? null : reader.GetString(5),
                                Pet_id = reader.GetInt32(6),
                                Vet_id = reader.IsDBNull(7) ? null : reader.GetInt32(7),
                                Franchise_id = reader.GetInt32(8),
                                Room_id = reader.IsDBNull(9) ? null : reader.GetInt32(9)
                            };
                            list.Add(item);
                        }
                    }
                }
            }
            return list;
        }

        public async Task<GetAppointmentDTO> GetOneAppointmentAsync(int id)
        {
            GetAppointmentDTO item = null;
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT Appointment_id, Date_time, Duration_minutes, Reason, Status, Notes, Pet_id, Vet_id, Franchise_id, Room_id FROM Appointment WHERE Appointment_id = @Id";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            item = new GetAppointmentDTO
                            {
                                Appointment_id = reader.GetInt32(0),
                                Date_time = reader.IsDBNull(1) ? null : reader.GetDateTime(1),
                                Duration_minutes = reader.GetInt32(2),
                                Reason = reader.GetString(3),
                                Status = reader.GetString(4),
                                Notes = reader.IsDBNull(5) ? null : reader.GetString(5),
                                Pet_id = reader.GetInt32(6),
                                Vet_id = reader.IsDBNull(7) ? null : reader.GetInt32(7),
                                Franchise_id = reader.GetInt32(8),
                                Room_id = reader.IsDBNull(9) ? null : reader.GetInt32(9)
                            };
                        }
                    }
                }
            }
            return item;
        }

        public async Task<bool> AddAppointmentAsync(AddAppointmentDTO dto)
        {
            bool bRet = true;
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "INSERT INTO Appointment (Date_time, Duration_minutes, Reason, Status, Pet_id, Vet_id, Franchise_id, Room_id) VALUES (@Date_time, @Duration_minutes, @Reason, @Status, @Pet_id, @Vet_id, @Franchise_id, @Room_id)";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Date_time", (object?)dto.Date_time ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Duration_minutes", dto.Duration_minutes);
                    command.Parameters.AddWithValue("@Reason", dto.Reason);
                    command.Parameters.AddWithValue("@Status", string.IsNullOrEmpty(dto.Status) ? "Pendiente" : dto.Status);
                    command.Parameters.AddWithValue("@Pet_id", dto.Pet_id);
                    command.Parameters.AddWithValue("@Vet_id", (object?)dto.Vet_id ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Franchise_id", dto.Franchise_id!.Value);
                    command.Parameters.AddWithValue("@Room_id", (object?)dto.Room_id ?? DBNull.Value);
                    
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

        public async Task<bool> UpdateAppointmentAsync(UpdateAppointmentDTO dto)
        {
            bool bRet = true;
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "UPDATE Appointment SET "; 

                if (dto.Date_time != null) query += "Date_time = @Date_time , ";
                if (!string.IsNullOrEmpty(dto.Reason)) query += "Reason = @Reason , ";
                if (!string.IsNullOrEmpty(dto.Status)) query += "Status = @Status , ";
                if (!string.IsNullOrEmpty(dto.Notes)) query += "Notes = @Notes , ";
                if (dto.Vet_id != null) query += "Vet_id = @Vet_id , ";
                if (dto.Room_id != null) query += "Room_id = @Room_id , ";

                query += " Appointment_id = Appointment_id WHERE Appointment_id = @Id ;";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", dto.Appointment_id);
                    if (dto.Date_time != null) command.Parameters.AddWithValue("@Date_time", dto.Date_time);
                    if (!string.IsNullOrEmpty(dto.Reason)) command.Parameters.AddWithValue("@Reason", dto.Reason);
                    if (!string.IsNullOrEmpty(dto.Status)) command.Parameters.AddWithValue("@Status", dto.Status);
                    if (!string.IsNullOrEmpty(dto.Notes)) command.Parameters.AddWithValue("@Notes", dto.Notes);
                    if (dto.Vet_id != null) command.Parameters.AddWithValue("@Vet_id", dto.Vet_id);
                    if (dto.Room_id != null) command.Parameters.AddWithValue("@Room_id", dto.Room_id);

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

        public async Task<bool> DeleteAppointmentAsync(int id)
        {
            bool bRet = true;
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "DELETE FROM Appointment WHERE Appointment_id = @Id";
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

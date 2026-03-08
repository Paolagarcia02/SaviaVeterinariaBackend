using MySql.Data.MySqlClient;
using SaviaVetAPI.DTOs;
using SaviaVetAPI.Exceptions;

namespace SaviaVetAPI.Repositories
{
    public class ClinicScheduleRepository : IClinicScheduleRepository
    {
        private readonly string _connectionString;

        public ClinicScheduleRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SaviaVetdb") ?? "";
        }

        public async Task<List<GetClinicScheduleDTO>> GetClinicSchedulesAsync()
        {
            var list = new List<GetClinicScheduleDTO>();
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = @"SELECT Schedule_id, Franchise_id, Room_id, Day_of_week, Open_time, Close_time, Is_open
                                   FROM Clinic_schedule";
            using var command = new MySqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new GetClinicScheduleDTO
                {
                    Schedule_id = reader.GetInt32(0),
                    Franchise_id = reader.GetInt32(1),
                    Room_id = reader.IsDBNull(2) ? null : reader.GetInt32(2),
                    Day_of_week = reader.GetInt32(3),
                    Open_time = reader.GetFieldValue<TimeSpan>(4),
                    Close_time = reader.GetFieldValue<TimeSpan>(5),
                    Is_open = reader.GetBoolean(6)
                });
            }

            return list;
        }

        public async Task<GetClinicScheduleDTO> GetOneClinicScheduleAsync(int id)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = @"SELECT Schedule_id, Franchise_id, Room_id, Day_of_week, Open_time, Close_time, Is_open
                                   FROM Clinic_schedule
                                   WHERE Schedule_id = @Id";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new GetClinicScheduleDTO
                {
                    Schedule_id = reader.GetInt32(0),
                    Franchise_id = reader.GetInt32(1),
                    Room_id = reader.IsDBNull(2) ? null : reader.GetInt32(2),
                    Day_of_week = reader.GetInt32(3),
                    Open_time = reader.GetFieldValue<TimeSpan>(4),
                    Close_time = reader.GetFieldValue<TimeSpan>(5),
                    Is_open = reader.GetBoolean(6)
                };
            }

            return null;
        }

        public async Task<bool> AddClinicScheduleAsync(AddClinicScheduleDTO dto)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = @"INSERT INTO Clinic_schedule (Franchise_id, Room_id, Day_of_week, Open_time, Close_time, Is_open)
                                   VALUES (@Franchise_id, @Room_id, @Day_of_week, @Open_time, @Close_time, @Is_open)";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Franchise_id", dto.Franchise_id);
            command.Parameters.AddWithValue("@Room_id", (object?)dto.Room_id ?? DBNull.Value);
            command.Parameters.AddWithValue("@Day_of_week", dto.Day_of_week);
            command.Parameters.AddWithValue("@Open_time", dto.Open_time);
            command.Parameters.AddWithValue("@Close_time", dto.Close_time);
            command.Parameters.AddWithValue("@Is_open", dto.Is_open);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            if (rowsAffected > 1) throw new MoreThanOneRowException();
            return rowsAffected == 1;
        }

        public async Task<bool> UpdateClinicScheduleAsync(UpdateClinicScheduleDTO dto)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "UPDATE Clinic_schedule SET ";
            if (dto.Day_of_week != null) query += "Day_of_week = @Day_of_week, ";
            if (dto.Open_time != null) query += "Open_time = @Open_time, ";
            if (dto.Close_time != null) query += "Close_time = @Close_time, ";
            if (dto.Is_open != null) query += "Is_open = @Is_open, ";
            query += "Schedule_id = Schedule_id WHERE Schedule_id = @Id";

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", dto.Schedule_id);
            if (dto.Day_of_week != null) command.Parameters.AddWithValue("@Day_of_week", dto.Day_of_week);
            if (dto.Open_time != null) command.Parameters.AddWithValue("@Open_time", dto.Open_time);
            if (dto.Close_time != null) command.Parameters.AddWithValue("@Close_time", dto.Close_time);
            if (dto.Is_open != null) command.Parameters.AddWithValue("@Is_open", dto.Is_open);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            if (rowsAffected > 1) throw new MoreThanOneRowException();
            return rowsAffected == 1;
        }

        public async Task<bool> DeleteClinicScheduleAsync(int id)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = "DELETE FROM Clinic_schedule WHERE Schedule_id = @Id";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            if (rowsAffected > 1) throw new MoreThanOneRowException();
            return rowsAffected == 1;
        }
    }
}

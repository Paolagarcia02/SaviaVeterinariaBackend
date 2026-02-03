using MySql.Data.MySqlClient;
using SaviaVetAPI.DTOs;
using SaviaVetAPI.Exceptions;
using Microsoft.Extensions.Configuration;

namespace SaviaVetAPI.Repositories
{
    public class FranchiseRepository : IFranchiseRepository
    {
        private readonly string _connectionString;

        public FranchiseRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SaviaVetdb") ?? "";
        }

        public async Task<List<GetFranchiseDTO>> GetFranchisesAsync()
        {
            List<GetFranchiseDTO> list = new List<GetFranchiseDTO>();
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT Franchise_id, Name, Address, Phone FROM Franchise";
                using (var command = new MySqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var item = new GetFranchiseDTO
                            {
                                Franchise_id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Address = reader.GetString(2),
                                Phone = reader.IsDBNull(3) ? null : reader.GetString(3)
                            };
                            list.Add(item);
                        }
                    }
                }
            }
            return list;
        }

        public async Task<GetFranchiseDTO> GetOneFranchiseAsync(int id)
        {
            GetFranchiseDTO item = null;
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT Franchise_id, Name, Address, Phone FROM Franchise WHERE Franchise_id = @Id";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            item = new GetFranchiseDTO
                            {
                                Franchise_id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Address = reader.GetString(2),
                                Phone = reader.IsDBNull(3) ? null : reader.GetString(3)
                            };
                        }
                    }
                }
            }
            return item;
        }

        public async Task<bool> AddFranchiseAsync(AddFranchiseDTO dto)
        {
            bool bRet = true;
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "INSERT INTO Franchise (Name, Address, Phone) VALUES (@Name, @Address, @Phone)";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", dto.Name);
                    command.Parameters.AddWithValue("@Address", dto.Address);
                    command.Parameters.AddWithValue("@Phone", (object)dto.Phone ?? DBNull.Value);
                    
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

        public async Task<bool> UpdateFranchiseAsync(UpdateFranchiseDTO dto)
        {
            bool bRet = true;
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "UPDATE Franchise SET "; 

                if (!string.IsNullOrEmpty(dto.Name)) query += "Name = @Name , ";
                if (!string.IsNullOrEmpty(dto.Address)) query += "Address = @Address , ";
                if (!string.IsNullOrEmpty(dto.Phone)) query += "Phone = @Phone , ";

                query += " Franchise_id = Franchise_id WHERE Franchise_id = @Id ;";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", dto.Franchise_id);
                    if (!string.IsNullOrEmpty(dto.Name)) command.Parameters.AddWithValue("@Name", dto.Name);
                    if (!string.IsNullOrEmpty(dto.Address)) command.Parameters.AddWithValue("@Address", dto.Address);
                    if (!string.IsNullOrEmpty(dto.Phone)) command.Parameters.AddWithValue("@Phone", dto.Phone);

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

        public async Task<bool> DeleteFranchiseAsync(int id)
        {
            bool bRet = true;
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "DELETE FROM Franchise WHERE Franchise_id = @Id";
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
using NUnit.Framework;
using Microsoft.Data.SqlClient;

namespace LDSTest.Shared
{
    public class Database
    {
        private readonly string _connectionString = null!;
        private readonly string _sqlScriptsPath = null!;

        public Database()
        {
            // Database parameters
            _connectionString = TestContext.Parameters["ConnectionString"]
                               ?? throw new InvalidOperationException("ConnectionString parameter is required in .runsettings");
            _sqlScriptsPath = TestContext.Parameters["SqlScriptsPath"]
                             ?? throw new InvalidOperationException("SqlScriptsPath parameter is required in .runsettings");
        }

        public async Task ResetDatabase()
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var deleteScript = await File.ReadAllTextAsync(Path.Combine(_sqlScriptsPath, "Booking_delete_all.sql"));
                using (var deleteCmd = new SqlCommand(deleteScript, connection))
                {
                    await deleteCmd.ExecuteNonQueryAsync();
                }

                var insertCustomersScript = await File.ReadAllTextAsync(Path.Combine(_sqlScriptsPath, "Insert_Customers.sql"));
                using (var insertCustomersCmd = new SqlCommand(insertCustomersScript, connection))
                {
                    await insertCustomersCmd.ExecuteNonQueryAsync();
                }

                var insertBookingsScript = await File.ReadAllTextAsync(Path.Combine(_sqlScriptsPath, "Insert_Bookings.sql"));
                using (var insertBookingsCmd = new SqlCommand(insertBookingsScript, connection))
                {
                    await insertBookingsCmd.ExecuteNonQueryAsync();
                }

                TestContext.WriteLine("Database reset successfully");
            }
            catch (Exception ex)
            {
                Assert.Fail($"Failed to reset database: {ex.Message}");
            }
        }
    }
}

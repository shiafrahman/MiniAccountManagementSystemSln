using Microsoft.Data.SqlClient;
using MiniAccountManagementSystem.Data.Repositories.Interfaces;
using MiniAccountManagementSystem.Models;
using System.Data;

namespace MiniAccountManagementSystem.Data.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly string _connectionString;

        public AccountRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<List<Account>> GetAllAccountsAsync()
        {
            var accounts = new List<Account>();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("sp_ManageChartOfAccounts", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Action", "SELECT");

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            accounts.Add(new Account
                            {
                                AccountId = reader.GetInt32("AccountId"),
                                AccountCode = reader.GetString("AccountCode"),
                                AccountName = reader.GetString("AccountName"),
                                ParentAccountId = reader.IsDBNull("ParentAccountId") ? null : reader.GetInt32("ParentAccountId"),
                                AccountType = reader.GetString("AccountType")
                            });
                        }
                    }
                }
            }
            return accounts;
        }

        public async Task<Account> GetAccountByIdAsync(int id)
        {
            Account account = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("sp_ManageChartOfAccounts", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Action", "SELECT");
                    command.Parameters.AddWithValue("@AccountId", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            account = new Account
                            {
                                AccountId = reader.GetInt32("AccountId"),
                                AccountCode = reader.GetString("AccountCode"),
                                AccountName = reader.GetString("AccountName"),
                                ParentAccountId = reader.IsDBNull("ParentAccountId") ? null : reader.GetInt32("ParentAccountId"),
                                AccountType = reader.GetString("AccountType")
                            };
                        }
                    }
                }
            }
            return account;
        }

        public async Task<int> CreateAccountAsync(Account account)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("sp_ManageChartOfAccounts", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Action", "INSERT");
                    command.Parameters.AddWithValue("@AccountCode", account.AccountCode);
                    command.Parameters.AddWithValue("@AccountName", account.AccountName);
                    command.Parameters.AddWithValue("@ParentAccountId", (object)account.ParentAccountId ?? DBNull.Value);
                    command.Parameters.AddWithValue("@AccountType", account.AccountType);

                    var accountId = await command.ExecuteScalarAsync();
                    return Convert.ToInt32(accountId);
                }
            }
        }

        public async Task UpdateAccountAsync(Account account)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("sp_ManageChartOfAccounts", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Action", "UPDATE");
                    command.Parameters.AddWithValue("@AccountId", account.AccountId);
                    command.Parameters.AddWithValue("@AccountCode", account.AccountCode);
                    command.Parameters.AddWithValue("@AccountName", account.AccountName);
                    command.Parameters.AddWithValue("@ParentAccountId", (object)account.ParentAccountId ?? DBNull.Value);
                    command.Parameters.AddWithValue("@AccountType", account.AccountType);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeleteAccountAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("sp_ManageChartOfAccounts", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Action", "DELETE");
                    command.Parameters.AddWithValue("@AccountId", id);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}

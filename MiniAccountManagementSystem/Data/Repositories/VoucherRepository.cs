using Microsoft.Data.SqlClient;
using MiniAccountManagementSystem.Data.Repositories.Interfaces;
using MiniAccountManagementSystem.Models;
using System.Data;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace MiniAccountManagementSystem.Data.Repositories
{
    public class VoucherRepository: IVoucherRepository
    {
        private readonly string _connectionString;

        public VoucherRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<int> SaveVoucherAsync(Voucher voucher)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("sp_SaveVoucher", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@VoucherType", voucher.VoucherType);
                    command.Parameters.AddWithValue("@VoucherDate", voucher.VoucherDate);
                    command.Parameters.AddWithValue("@ReferenceNo", voucher.ReferenceNo);
                    command.Parameters.AddWithValue("@CreatedBy", voucher.CreatedBy);
                    command.Parameters.AddWithValue("@VoucherEntries", JsonSerializer.Serialize(voucher.Entries));

                    var voucherId = await command.ExecuteScalarAsync();
                    return Convert.ToInt32(voucherId);
                }
            }
        }

        public async Task<List<Voucher>> GetAllVouchersAsync()
        {
            var vouchers = new List<Voucher>();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("SELECT * FROM Vouchers", connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            vouchers.Add(new Voucher
                            {
                                VoucherId = reader.GetInt32("VoucherId"),
                                VoucherType = reader.GetString("VoucherType"),
                                VoucherDate = reader.GetDateTime("VoucherDate"),
                                ReferenceNo = reader.GetString("ReferenceNo"),
                                CreatedBy = reader.GetString("CreatedBy")
                            });
                        }
                    }
                }
            }
            return vouchers;
        }

    }
}

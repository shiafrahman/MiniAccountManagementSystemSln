using MiniAccountManagementSystem.Models;

namespace MiniAccountManagementSystem.Data.Repositories.Interfaces
{
    public interface IVoucherRepository
    {
        Task<int> SaveVoucherAsync(Voucher voucher);
        Task<List<Voucher>> GetAllVouchersAsync();
    }
}

namespace MiniAccountManagementSystem.Models
{
    public class VoucherEntry
    {
        public int VoucherEntryId { get; set; }
        public int VoucherId { get; set; }
        public int AccountId { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public string Description { get; set; }
    }
}

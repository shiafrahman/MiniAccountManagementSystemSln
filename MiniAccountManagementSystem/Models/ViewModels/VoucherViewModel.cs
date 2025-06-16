namespace MiniAccountManagementSystem.Models.ViewModels
{
    public class VoucherViewModel
    {
        public int VoucherId { get; set; }
        public string VoucherType { get; set; }
        public DateTime VoucherDate { get; set; }
        public string ReferenceNo { get; set; }
        public List<VoucherEntryViewModel> Entries { get; set; } = new List<VoucherEntryViewModel>();
    }

    public class VoucherEntryViewModel
    {
        public int AccountId { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public string Description { get; set; }
    }
}

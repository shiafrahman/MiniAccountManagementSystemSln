namespace MiniAccountManagementSystem.Models
{
    public class Account
    {
        public int AccountId { get; set; }
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
        public int? ParentAccountId { get; set; }
        public string AccountType { get; set; }
    }
    public class AccountNode
    {
        public Account Account { get; set; }
        public List<AccountNode> Children { get; set; } = new List<AccountNode>();
    }
}

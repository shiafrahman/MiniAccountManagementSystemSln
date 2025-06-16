using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniAccountManagementSystem.Data.Repositories.Interfaces;
using MiniAccountManagementSystem.Models;
using OfficeOpenXml;

namespace MiniAccountManagementSystem.Controllers
{
    [Authorize(Roles = "Admin, Accountant")]
    public class AccountsController : Controller
    {
        private readonly IAccountRepository _accountRepository;

        public AccountsController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }
        public async Task<IActionResult> Index()
        {
            var accounts = await _accountRepository.GetAllAccountsAsync();
            var accountTree = BuildAccountTree(accounts);
            return View(accountTree);
        }

        public IActionResult Create()
        {
            ViewBag.Accounts = _accountRepository.GetAllAccountsAsync().Result;
            return View(new Account());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Account account)
        {
            if (ModelState.IsValid)
            {
                await _accountRepository.CreateAccountAsync(account);
                TempData["Success"] = "Account created successfully!";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Accounts = await _accountRepository.GetAllAccountsAsync();
            TempData["Error"] = "Failed to create account. Please check the input.";
            return View(account);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var account = await _accountRepository.GetAccountByIdAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            ViewBag.Accounts = await _accountRepository.GetAllAccountsAsync();
            return View(account);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Account account)
        {
            if (ModelState.IsValid)
            {
                await _accountRepository.UpdateAccountAsync(account);
                TempData["Success"] = "Account updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Accounts = await _accountRepository.GetAllAccountsAsync();
            TempData["Error"] = "Failed to update account. Please check the input.";
            return View(account);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var account = await _accountRepository.GetAccountByIdAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            return View(account);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _accountRepository.DeleteAccountAsync(id);
            TempData["Success"] = "Account deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ExportToExcel()
        {
            var accounts = await _accountRepository.GetAllAccountsAsync();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("ChartOfAccounts");
                worksheet.Cells[1, 1].Value = "Account Code";
                worksheet.Cells[1, 2].Value = "Account Name";
                worksheet.Cells[1, 3].Value = "Parent Account";
                worksheet.Cells[1, 4].Value = "Account Type";

                for (int i = 0; i < accounts.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = accounts[i].AccountCode;
                    worksheet.Cells[i + 2, 2].Value = accounts[i].AccountName;
                    worksheet.Cells[i + 2, 3].Value = accounts.FirstOrDefault(a => a.AccountId == accounts[i].ParentAccountId)?.AccountName ?? "";
                    worksheet.Cells[i + 2, 4].Value = accounts[i].AccountType;
                }

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ChartOfAccounts.xlsx");
            }
        }

        private List<AccountNode> BuildAccountTree(List<Account> accounts)
        {
            var tree = new List<AccountNode>();
            var lookup = accounts.ToDictionary(a => a.AccountId, a => new AccountNode { Account = a });

            foreach (var account in accounts)
            {
                if (account.ParentAccountId.HasValue && lookup.ContainsKey(account.ParentAccountId.Value))
                {
                    lookup[account.ParentAccountId.Value].Children.Add(lookup[account.AccountId]);
                }
                else
                {
                    tree.Add(lookup[account.AccountId]);
                }
            }

            return tree;
        }

    }

    
}

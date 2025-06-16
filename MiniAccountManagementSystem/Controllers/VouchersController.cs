using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniAccountManagementSystem.Data.Repositories.Interfaces;
using MiniAccountManagementSystem.Models.ViewModels;
using MiniAccountManagementSystem.Models;
using System.Security.Claims;

namespace MiniAccountManagementSystem.Controllers
{
    [Authorize(Roles = "Admin, Accountant")]
    public class VouchersController : Controller
    {
        private readonly IVoucherRepository _voucherRepository;
        private readonly IAccountRepository _accountRepository;

        public VouchersController(IVoucherRepository voucherRepository, IAccountRepository accountRepository)
        {
            _voucherRepository = voucherRepository;
            _accountRepository = accountRepository;
        }

        public async Task<IActionResult> Index()
        {
            var vouchers = await _voucherRepository.GetAllVouchersAsync();
            return View(vouchers);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Accounts = await _accountRepository.GetAllAccountsAsync();
            var model = new VoucherViewModel { VoucherDate = DateTime.Today };
            model.Entries.Add(new VoucherEntryViewModel());
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VoucherViewModel model)
        {
            if (ModelState.IsValid)
            {
                var voucher = new Voucher
                {
                    VoucherType = model.VoucherType,
                    VoucherDate = model.VoucherDate,
                    ReferenceNo = model.ReferenceNo,
                    CreatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    Entries = model.Entries.Select(e => new VoucherEntry
                    {
                        AccountId = e.AccountId,
                        Debit = e.Debit,
                        Credit = e.Credit,
                        Description = e.Description
                    }).ToList()
                };

                await _voucherRepository.SaveVoucherAsync(voucher);
                TempData["Success"] = "Voucher created successfully!";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Accounts = await _accountRepository.GetAllAccountsAsync();
            TempData["Error"] = "Failed to create voucher. Please check the input.";
            return View(model);
        }
    }
}

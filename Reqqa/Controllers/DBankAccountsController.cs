using Core.Interfaces;
using Core.TableDb;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Salony.Models.ControllerDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static Salony.Enums.AllEnums;
using static Salony.Helper.Helper;

namespace Salony.Controllers
{
    [AuthorizeRoles(Roles.Admin, Roles.BankAccounts)]
    public class DBankAccountsController : Controller
    {
        private readonly UserManager<ApplicationDbUser> _userManager;
        private readonly IUnitOfWork<ApplicationDbUser> _users;
        private readonly IUnitOfWork<BankAccounts> _bankAccounts;
        public DBankAccountsController(UserManager<ApplicationDbUser> userManager, IUnitOfWork<ApplicationDbUser> users, IUnitOfWork<BankAccounts> bankAccounts)
        {
            this._userManager = userManager;
            this._users = users;
            this._bankAccounts = bankAccounts;
        }
        public async Task<IActionResult> Index()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userFk_BranchID = await _users.Entity.GetAsync(predicate: b => b.Id == userId);

            return View(await _bankAccounts.Entity.GetAllAsync(predicate: x => x.FK_BranchID == userFk_BranchID.FK_BranchID));
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(BankAccountDTO model)
        {
            if (ModelState.IsValid)
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userFk_BranchID = await _users.Entity.GetAsync(predicate: b => b.Id == userId);

                BankAccounts bankAccounts = new BankAccounts()
                {
                    bankNameAr = model.bankNameAr,
                    bankNameEn = model.bankNameEn,
                    bankAccountNumber = model.bankAccountNumber,
                    OwnerNameAr = model.OwnerName,
                    FK_BranchID = userFk_BranchID.FK_BranchID,
                    isActive = true
                };

                await _bankAccounts.Entity.InsertAsync(bankAccounts);
                await _bankAccounts.Save();

                return RedirectToAction("Index");
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var bankAccountsDB = await _bankAccounts.Entity.FindByIdAsync(id);

            BankAccountDTO model = new BankAccountDTO()
            {
                bankNameAr = bankAccountsDB.bankNameAr,
                bankNameEn = bankAccountsDB.bankNameEn,
                bankAccountNumber = bankAccountsDB.bankAccountNumber,
                OwnerName = bankAccountsDB.OwnerNameAr,

                isActive = bankAccountsDB.isActive
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BankAccountDTO model)
        {
            if (model.ID == 0)
            {
                return NotFound();
            }
            var bankAccountsDb = await _bankAccounts.Entity.FindByIdAsync(model.ID);
            if (ModelState.IsValid)
            {
                bankAccountsDb.bankNameAr = model.bankNameAr;
                bankAccountsDb.bankNameEn = model.bankNameEn;
                bankAccountsDb.bankAccountNumber = model.bankAccountNumber;
                bankAccountsDb.OwnerNameAr = model.OwnerName;

                _bankAccounts.Entity.Update(bankAccountsDb);
                await _bankAccounts.Save();

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [HttpPost]
        public async Task<bool> ChangeStatus(int id)
        {
            var Data = await _bankAccounts.Entity.FindByIdAsync(id);

            Data.isActive = !Data.isActive;
            await _bankAccounts.Save();

            return Data.isActive;
        }

        //[HttpPost]
        //public async Task<IActionResult> ChangeBranchAsync(int branch_id)
        //{
        //    var UserId = _userManager.GetUserId(User);
        //    var UserDB = await _users.Entity.FindByIdAsync(UserId);

        //    if (branch_id != 0)
        //    {
        //        UserDB.FK_BranchID = branch_id;
        //    }
        //    else
        //    {
        //        UserDB.FK_BranchID = 0;
        //    }

        //    await _users.Save();

        //    return Json(new { key = 1 });
        //}

    }
}

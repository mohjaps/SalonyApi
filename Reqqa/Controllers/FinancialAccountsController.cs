using Core.Interfaces;
using Core.TableDb;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static Salony.Enums.AllEnums;
using static Salony.Helper.Helper;

namespace Salony.Controllers
{
    [AuthorizeRoles(Roles.Admin)]
    public class FinancialAccountsController : Controller
    {
        private readonly IUnitOfWork<ApplicationDbUser> _users;
        private readonly IUnitOfWork<FinancialAccount> _financialAccount;

        public FinancialAccountsController(IUnitOfWork<ApplicationDbUser> users, IUnitOfWork<FinancialAccount> financialAccount)
        {
            this._users = users;
            this._financialAccount = financialAccount;
        }
        public async Task<IActionResult> Index()
        {
            string userIdnew = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userFk_BranchID = await _users.Entity.GetAsync(predicate: b => b.Id == userIdnew);

            var settlementRequest = await _financialAccount.Entity.GetAllAsync(predicate: x => (x.FKProvider.FK_BranchID == userFk_BranchID.FK_BranchID),
                                                      include: source => source.Include(i => i.FKProvider).ThenInclude(x=>x.ProviderAditionalData),
                                                      orderBy: o => o.OrderByDescending(x => !x.IsPaid).ThenByDescending(x=>x.Date));


            return View(settlementRequest);
        }



        public async Task<IActionResult> ChangeState(int id)
        {
            try
            {
                var payMent = await _financialAccount.Entity.FindByIdAsync(id);
                if (payMent.IsPaid == false)
                {
                    payMent.IsPaid = true;


                    var provider = await _users.Entity.GetAsync(predicate: x => x.Id == payMent.FkProviderId);
                    

                    //if(provider.FK_BranchID == (int)BranchName.Eleklil)
                    //{
                    //    provider.userWallet = 0;
                    //    _users.Entity.Update(provider);
                    //    await _users.Save();
                    //}

                    var providerWallet = await _users.Entity.GetAsync(predicate: u => u.Id == payMent.FkProviderId);
                    providerWallet.stableWallet = 0;
                    _users.Entity.Update(providerWallet);
                    await _users.Save();
                    await _financialAccount.Save();
                }

                return Json(new { key = 1, data = payMent.IsPaid });
            }
            catch (Exception ex)
            {
                return Json(new { key = 0, msg = ex.Message });
            }
        }
    }
}

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
    [AuthorizeRoles(Roles.Admin,Roles.Copons)]
    public class DCoponsController : Controller
    {
        private readonly UserManager<ApplicationDbUser> _userManager;
        private readonly IUnitOfWork<ApplicationDbUser> _users;
        private readonly IUnitOfWork<Copons> _copons;
        public DCoponsController(UserManager<ApplicationDbUser> userManager, IUnitOfWork<ApplicationDbUser> users, IUnitOfWork<Copons> copons)
        {
            this._userManager = userManager;
            this._users = users;
            this._copons = copons;
        }
        public async Task<IActionResult> Index()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userFk_BranchID = await _users.Entity.GetAsync(predicate: b => b.Id == userId);

            return View(await _copons.Entity.GetAllAsync(predicate:x=>x.FK_Branch == userFk_BranchID.FK_BranchID));
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CoponsDTO model)
        {
            if (ModelState.IsValid)
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userFk_BranchID = await _users.Entity.GetAsync(predicate: b => b.Id == userId);

                var foundedCoupon = await _copons.Entity.GetAsync(c => c.code == model.code && c.FK_Branch == userFk_BranchID.FK_BranchID);
                if (foundedCoupon != null)
                {
                    ModelState.AddModelError("", "هذا الكود موجود من قبل");
                    return View(model);
                }
                if (model.discPercentage > 100)
                {
                    ModelState.AddModelError(nameof(model.discPercentage), "نسبة الخصم خطأ");
                    return View(model);
                }

                Copons Copons = new Copons()
                {
                    code = model.code,
                    discPercentage = model.discPercentage,
                    FK_Branch = userFk_BranchID.FK_BranchID,
                    isActive = true
                };

                await _copons.Entity.InsertAsync(Copons);
                await _copons.Save();

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
            var branchDB = await _copons.Entity.FindByIdAsync(id);

            CoponsDTO model = new CoponsDTO()
            {
                code = branchDB.code,
                discPercentage = branchDB.discPercentage,
                isActive = branchDB.isActive
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CoponsDTO model)
        {
            if (model.ID == 0)
            {
                return NotFound();
            }
            var couponDB = await _copons.Entity.FindByIdAsync(model.ID);
            if (ModelState.IsValid)
            {

                var foundedCoupon = await _copons.Entity.GetAsync(c => c.code == model.code && c.FK_Branch == couponDB.FK_Branch && c.ID != model.ID);
                if (foundedCoupon != null)
                {
                    ModelState.AddModelError("", "هذا الكود موجود من قبل");
                    return View(model);
                }
                if (model.discPercentage > 100)
                {
                    ModelState.AddModelError(nameof(model.discPercentage), "نسبة الخصم خطأ");
                    return View(model);
                }


                couponDB.code = model.code;
                couponDB.discPercentage = model.discPercentage;

                _copons.Entity.Update(couponDB);
                await _copons.Save();

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [HttpPost]
        public async Task<bool> ChangeStatus(int id)
        {
            var Data = await _copons.Entity.FindByIdAsync(id);

            Data.isActive = !Data.isActive;
            await _copons.Save();

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

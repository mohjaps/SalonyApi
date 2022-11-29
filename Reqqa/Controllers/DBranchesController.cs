using Core.Interfaces;
using Core.TableDb;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Salony.Models.ControllerDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Salony.Enums.AllEnums;
using static Salony.Helper.Helper;

namespace Salony.Controllers
{
    [AuthorizeRoles(Roles.SuperAdmin)]
    public class DBranchesController : Controller
    {
        private readonly UserManager<ApplicationDbUser> _userManager;
        private readonly IUnitOfWork<ApplicationDbUser> _users;
        private readonly IUnitOfWork<Branches> _branches;
        public DBranchesController(UserManager<ApplicationDbUser> userManager, IUnitOfWork<ApplicationDbUser> users, IUnitOfWork<Branches> branches)
        {
            this._userManager = userManager;
            this._users = users;
            this._branches = branches;
        }
        //public async Task<IActionResult> Index()
        //{
        //    return View(await _branches.Entity.GetAllAsync());
        //}

        //public IActionResult Create()
        //{
        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Create(BranchDTO model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        Branches branches = new Branches()
        //        {
        //            nameAr = model.nameAr,
        //            nameEn = model.nameEn,
        //            date = GetCurrentDate(),
        //            isActive = true
        //        };

        //        await _branches.Entity.InsertAsync(branches);
        //        await _branches.Save();

        //        return RedirectToAction("Index");
        //    }
        //    return View(model);
        //}

        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }
        //    var branchDB = await _branches.Entity.FindByIdAsync(id);

        //    BranchDTO model = new BranchDTO()
        //    {
        //        nameAr = branchDB.nameAr,
        //        nameEn = branchDB.nameEn,
        //        isActive = branchDB.isActive
        //    };

        //    return View(model);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(BranchDTO model)
        //{
        //    if (model.ID == 0)
        //    {
        //        return NotFound();
        //    }
        //    var branchDB = await _branches.Entity.FindByIdAsync(model.ID);
        //    if (ModelState.IsValid)
        //    {
        //        branchDB.nameAr = model.nameAr;
        //        branchDB.nameEn = model.nameEn;
        //        branchDB.isActive = model.isActive;

        //        _branches.Entity.Update(branchDB);
        //        await _branches.Save();

        //        return RedirectToAction(nameof(Index));
        //    }

        //    return View(model);
        //}

        //[HttpPost]
        //public async Task<bool> ChangeStatus(int id)
        //{
        //    var Data = await _branches.Entity.FindByIdAsync(id);

        //    Data.isActive = !Data.isActive;
        //    await _branches.Save();

        //    return Data.isActive;
        //}

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

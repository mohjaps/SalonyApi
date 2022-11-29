using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Core.Interfaces;
using Core.TableDb;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Salony.Models.ControllerDTO;

namespace Salony.Controllers
{
    public class DMainServicesController : Controller
    {
        private readonly UserManager<ApplicationDbUser> _userManager;
        private readonly IUnitOfWork<ApplicationDbUser> _users;
        private readonly IUnitOfWork<MainServices> _mainServices;

        public DMainServicesController(UserManager<ApplicationDbUser> userManager,  IUnitOfWork<ApplicationDbUser> users, IUnitOfWork<MainServices> mainServices)
        {
            this._userManager = userManager;
            this._users = users;
            this._mainServices = mainServices;
        }
        public async Task<IActionResult> Index(int Id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userFk_BranchID = await _users.Entity.GetAsync(predicate: b => b.Id == userId);
            ViewBag.FkCategoryId = Id;
            return View(await _mainServices.Entity.GetAllAsync(predicate: s => s.FK_BranchID == userFk_BranchID.FK_BranchID && s.FK_CategoryID==Id));
        }
        public IActionResult Create(int Id)
        {
            ViewBag.FkCategoryId = Id;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(MainServicesDTO model)
        {
            if (ModelState.IsValid)
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userFk_BranchID = await _users.Entity.GetAsync(predicate: b => b.Id == userId);

                MainServices mainServices = new MainServices()
                {
                    nameAr = model.nameAr,
                    nameEn = model.nameEn,
                    isActive = true,
                    FK_CategoryID=model.FK_CategoryID,
                    
                    FK_BranchID = userFk_BranchID.FK_BranchID
                };

                await _mainServices.Entity.InsertAsync(mainServices);
                await _mainServices.Save();

                return RedirectToAction("Index" ,new { id= mainServices.FK_CategoryID});
            }
            return View(model);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var mainServices = await _mainServices.Entity.FindByIdAsync(id);

            MainServicesEditDTO model = new MainServicesEditDTO()
            {
                nameAr = mainServices.nameAr,
                nameEn = mainServices.nameEn,
                isActive = mainServices.isActive,
                FK_CategoryID= mainServices.FK_CategoryID,
                FK_BranchID=mainServices.FK_BranchID
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MainServicesEditDTO model)
        {
            if (model.ID == 0)
            {
                return NotFound();
            }
            var mainServicesDB = await _mainServices.Entity.FindByIdAsync(model.ID);
            if (ModelState.IsValid)
            {


                mainServicesDB.nameAr = model.nameAr;
                mainServicesDB.nameEn = model.nameEn;
                mainServicesDB.isActive = model.isActive;

                _mainServices.Entity.Update(mainServicesDB);
                await _mainServices.Save();

                return RedirectToAction(nameof(Index), new { id = mainServicesDB.FK_CategoryID });
            }

            return View(model);
        }


        [HttpPost]
        public async Task<bool> ChangeStatus(int id)
        {
            var Data = await _mainServices.Entity.FindByIdAsync(id);

            Data.isActive = !Data.isActive;
            await _mainServices.Save();

            return Data.isActive;
        }
    }
}

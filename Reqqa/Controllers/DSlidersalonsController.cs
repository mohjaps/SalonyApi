using Core.Interfaces;
using Core.TableDb;
using Microsoft.AspNetCore.Hosting;
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
    [AuthorizeRoles(Roles.Admin,Roles.Sliders)]
    public class DSlidersalonsController : Controller
    {

        private readonly UserManager<ApplicationDbUser> _userManager;
        private readonly IUnitOfWork<ApplicationDbUser> _users;
        private readonly IUnitOfWork<Sliders> _sliders;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public DSlidersalonsController(UserManager<ApplicationDbUser> userManager, IWebHostEnvironment hostingEnvironment, IUnitOfWork<ApplicationDbUser> users, IUnitOfWork<Sliders> sliders)
        {
            this._userManager = userManager;
            this._users = users;
            this._sliders = sliders;
            this._hostingEnvironment = hostingEnvironment;
        }


        public async Task<IActionResult> Index()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userFk_BranchID = await _users.Entity.GetAsync(predicate: b => b.Id == userId);

            //var Sliders = await _sliders.Entity
            return View(await _sliders.Entity.GetAllAsync(predicate: s => s.type == (int)SliderType.salons && s.FK_BranchID == userFk_BranchID.FK_BranchID));
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(SlidersDTO model)
        {
            if (ModelState.IsValid)
            {

                if (model.img == null)
                {
                    ModelState.AddModelError("Img", "من فضلك ادخل صورة القسم");
                    return View(model);
                }

                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userFk_BranchID = await _users.Entity.GetAsync(predicate: b => b.Id == userId);

                string Img = ProcessUploadedFile(_hostingEnvironment, model.img, "SliderImages");

                Sliders sliders = new Sliders()
                {
                    img = Img,
                    isActive = true,
                    type = (int)SliderType.salons,
                    FK_BranchID = userFk_BranchID.FK_BranchID
                };

                await _sliders.Entity.InsertAsync(sliders);
                await _sliders.Save();

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
            var sliderDB = await _sliders.Entity.FindByIdAsync(id);

            SlidersEditDTO model = new SlidersEditDTO()
            {
                DisplayImg = sliderDB.img,
                isActive = sliderDB.isActive
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SlidersEditDTO model)
        {
            if (model.ID == 0)
            {
                return NotFound();
            }

            var sliderDB = await _sliders.Entity.FindByIdAsync(model.ID);

            if (ModelState.IsValid)
            {

                if (model.img != null)
                {
                    string Img = ProcessUploadedFile(_hostingEnvironment, model.img, "SliderImages");
                    sliderDB.img = Img;
                }
                else
                {
                    sliderDB.img = sliderDB.img;
                }

                sliderDB.isActive = model.isActive;

                _sliders.Entity.Update(sliderDB);
                await _sliders.Save();

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }


        [HttpPost]
        public async Task<bool> ChangeStatus(int id)
        {
            var Data = await _sliders.Entity.FindByIdAsync(id);

            Data.isActive = !Data.isActive;
            await _sliders.Save();

            return Data.isActive;
        }


    }
}

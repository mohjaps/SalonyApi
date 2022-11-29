using Core.Interfaces;
using Core.TableDb;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    public class DSliderhomeController : Controller
    {

        private readonly UserManager<ApplicationDbUser> _userManager;
        private readonly IUnitOfWork<ApplicationDbUser> _users;
        private readonly IUnitOfWork<Sliders> _sliders;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public DSliderhomeController(UserManager<ApplicationDbUser> userManager, IWebHostEnvironment hostingEnvironment, IUnitOfWork<ApplicationDbUser> users, IUnitOfWork<Sliders> sliders)
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
            return View(await _sliders.Entity.GetAllAsync(predicate: s => s.type == (int)SliderType.home && s.FK_BranchID == userFk_BranchID.FK_BranchID));
        }

        public async Task<ActionResult> Create()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userFk_BranchID = await _users.Entity.GetAsync(predicate: b => b.Id == userId);
            ViewBag.Providers = (await _users.Entity.GetAllAsync(predicate: s => s.typeUser == (int)TypeUser.provider
                        && s.FK_BranchID == userFk_BranchID.FK_BranchID && s.isActive && s.activeCode,
                                    include: source => source.Include(i => i.ProviderAditionalData),
                                    orderBy: source => source.OrderByDescending(o => o.registerDate)))
                                    .Select(c =>
                                    new SelectListItem
                                    {
                                        Value = c.ProviderAditionalData.ID.ToString(),
                                        Text = c.fullName,
                                    }).ToList();
            ViewBag.branchId = userFk_BranchID.FK_BranchID;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(SlidersDTO model)
        {                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userFk_BranchID = await _users.Entity.GetAsync(predicate: b => b.Id == userId);

            if (ModelState.IsValid)
            {

                if (model.img == null)
                {
                    ModelState.AddModelError("Img", "من فضلك ادخل صورة القسم");
                    return View(model);
                }


                string Img = ProcessUploadedFile(_hostingEnvironment, model.img, "SliderImages");

                Sliders sliders = new Sliders()
                {
                    img = Img,
                    isActive = true,
                    type = (int)SliderType.home,
                    FK_BranchID = userFk_BranchID.FK_BranchID,
                    ProviderId = model.ProviderId
                };

                await _sliders.Entity.InsertAsync(sliders);
                await _sliders.Save();

                return RedirectToAction("Index");
            }

                ViewBag.Providers = (await _users.Entity.GetAllAsync(predicate: s => s.typeUser == (int)TypeUser.provider
                        && s.FK_BranchID == userFk_BranchID.FK_BranchID &&s.isActive && s.activeCode,
                        include: source => source.Include(i => i.ProviderAditionalData),
                        orderBy: source => source.OrderByDescending(o => o.registerDate)))
                        .Select(c =>
                        new SelectListItem
                        {
                            Value = c.ProviderAditionalData.ID.ToString(),
                            Text = c.fullName,
                        }).ToList();

            ViewBag.branchId = userFk_BranchID.FK_BranchID;

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
                isActive = sliderDB.isActive,
                ProviderId = sliderDB.ProviderId
                //FK_BranchID=sliderDB.FK_BranchID
            };

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userFk_BranchID = await _users.Entity.GetAsync(predicate: b => b.Id == userId);
            ViewBag.Providers = (await _users.Entity.GetAllAsync(predicate: s => s.typeUser == (int)TypeUser.provider
                        && s.FK_BranchID == userFk_BranchID.FK_BranchID && s.isActive && s.activeCode,
                                    include: source => source.Include(i => i.ProviderAditionalData),
                                    orderBy: source => source.OrderByDescending(o => o.registerDate)))
                                    .Select(c =>
                                    new SelectListItem
                                    {
                                        Value = c.ProviderAditionalData.ID.ToString(),
                                        Text = c.fullName,
                                        Selected = sliderDB.ProviderId == c.ProviderAditionalData.ID
                                    }).ToList();

            ViewBag.branchId = userFk_BranchID.FK_BranchID;

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
                sliderDB.ProviderId = model.ProviderId;

                _sliders.Entity.Update(sliderDB);
                await _sliders.Save();

                return RedirectToAction(nameof(Index));
            }
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userFk_BranchID = await _users.Entity.GetAsync(predicate: b => b.Id == userId);
            ViewBag.Providers = (await _users.Entity.GetAllAsync(predicate: s => s.typeUser == (int)TypeUser.provider
                        && s.FK_BranchID == userFk_BranchID.FK_BranchID && s.isActive && s.activeCode,
                                    include: source => source.Include(i => i.ProviderAditionalData),
                                    orderBy: source => source.OrderByDescending(o => o.registerDate)))
                                    .Select(c =>
                                    new SelectListItem
                                    {
                                        Value = c.ProviderAditionalData.ID.ToString(),
                                        Text = c.fullName,
                                        Selected = sliderDB.ProviderId == c.ProviderAditionalData.ID
                                    }).ToList();
            ViewBag.branchId = userFk_BranchID.FK_BranchID;

            return View(model);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var slider = await _sliders.Entity.FindByIdAsync(id);

             _sliders.Entity.Delete(slider);
            await _sliders.Save();


            return RedirectToAction(nameof(Index));
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

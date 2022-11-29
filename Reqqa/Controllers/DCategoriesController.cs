using Core.Interfaces;
using Core.TableDb;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
    [AuthorizeRoles(Roles.Admin,Roles.SuperAdmin,Roles.Categories)]
    public class DCategoriesController : Controller
    {
        private readonly UserManager<ApplicationDbUser> _userManager;
        private readonly IUnitOfWork<ApplicationDbUser> _users;
        private readonly IUnitOfWork<Categories> _categories;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public DCategoriesController(UserManager<ApplicationDbUser> userManager,IWebHostEnvironment hostEnvironment, IUnitOfWork<ApplicationDbUser> users, IUnitOfWork<Categories> categories)
        {
            this._userManager = userManager;
            this._users = users;
            this._categories = categories;
            this._hostingEnvironment = hostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userFk_BranchID = await _users.Entity.GetAsync(predicate: b => b.Id == userId);

            return View(await _categories.Entity.GetAllAsync(predicate: s => s.FK_BranchID == userFk_BranchID.FK_BranchID));
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CategoryDTO model)
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

                string Img = ProcessUploadedFile(_hostingEnvironment, model.img, "CategoriesImages");

                Categories category = new Categories()
                {
                    img = Img,
                    nameAr = model.nameAr,
                    nameEn = model.nameEn,
                    isActive = true,
                    FK_BranchID = userFk_BranchID.FK_BranchID
                };

                await _categories.Entity.InsertAsync(category);
                await _categories.Save();

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
            var sliderDB = await _categories.Entity.FindByIdAsync(id);

            CategoryEditDTO model = new CategoryEditDTO()
            {
                nameAr = sliderDB.nameAr,
                nameEn = sliderDB.nameEn,
                DisplayImg = sliderDB.img,
                isActive = sliderDB.isActive
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryEditDTO model)
        {
            if (model.ID == 0)
            {
                return NotFound();
            }
            var categoryDB = await _categories.Entity.FindByIdAsync(model.ID);
            if (ModelState.IsValid)
            {

                if (model.img != null)
                {
                    string Img = ProcessUploadedFile(_hostingEnvironment, model.img, "SliderImages");
                    categoryDB.img = Img;
                }
                else
                {
                    categoryDB.img = categoryDB.img;

                }

                categoryDB.nameAr = model.nameAr;
                categoryDB.nameEn = model.nameEn;
                categoryDB.isActive = model.isActive;

                _categories.Entity.Update(categoryDB);
                await _categories.Save();

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [HttpPost]
        public async Task<bool> ChangeStatus(int id)
        {
            var Data = await _categories.Entity.FindByIdAsync(id);

            Data.isActive = !Data.isActive;
            await _categories.Save();

            return Data.isActive;
        }
    }
}

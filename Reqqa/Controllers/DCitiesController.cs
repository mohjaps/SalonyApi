using Core.Interfaces;
using Core.TableDb;
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
    //Roles Cities Must Be Added
    [AuthorizeRoles(Roles.Cities, Roles.Admin)]
    public class DCitiesController : Controller
    {

        private readonly IUnitOfWork<ApplicationDbUser> _users;
        private readonly IUnitOfWork<Cities> _cities;
        private readonly IUnitOfWork<Districts> _districts;

        public DCitiesController(IUnitOfWork<ApplicationDbUser> users,IUnitOfWork<Cities> cities, IUnitOfWork<Districts> districts)
        {
            this._users = users;
            this._cities = cities;
            this._districts = districts;
        }
        public async Task<IActionResult> Index()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userFk_BranchID = await _users.Entity.GetAsync(predicate: b => b.Id == userId);

            var cities = (await _cities.Entity.GetAllAsync(predicate: s => s.FK_BranchID == userFk_BranchID.FK_BranchID));
            return View(cities);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CitiesDTO model)
        {
            if (ModelState.IsValid)
            {

                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userFk_BranchID = await _users.Entity.GetAsync(predicate: b => b.Id == userId);


                Cities newCity = new Cities()
                {
                    nameAr = model.nameAr,
                    nameEn = model.nameEn,
                    isActive = true,
                    FK_BranchID = userFk_BranchID.FK_BranchID
                };
                await _cities.Entity.InsertAsync(newCity);
                await _cities.Save();

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
            var CityDB = await _cities.Entity.FindByIdAsync(id);

            CitiesDTO model = new CitiesDTO()
            {
                ID = CityDB.ID,
                nameAr = CityDB.nameAr,
                nameEn = CityDB.nameEn,
                isActive = CityDB.isActive
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CitiesDTO model)
        {
            if (model.ID == 0)
            {
                return NotFound();
            }
            var citydata = await _cities.Entity.FindByIdAsync(model.ID);
            if (ModelState.IsValid)
            {
                try
                {
                    citydata.nameAr = model.nameAr;
                    citydata.nameEn = model.nameEn;
                    citydata.isActive = model.isActive;

                    _cities.Entity.Update(citydata);
                    await _cities.Save();


                }
                catch (DbUpdateConcurrencyException)
                {

                }
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        public async Task<IActionResult> Districs(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var city = (await _cities.Entity.FindByIdAsync(id));
            var districsDB = (await _districts.Entity.GetAllAsync(predicate: x => x.FK_CityID == id));
            ViewData["id"] = id;
            ViewData["city_name"] = city.nameAr;
            return View(districsDB);
        }


        public ActionResult CreateDistrict(int id)
        {
            ViewData["id"] = id;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateDistrict(DistrictsDTO model)
        {
            if (ModelState.IsValid)
            {
                Districts newDistrict = new Districts()
                {
                    nameAr = model.nameAr,
                    nameEn = model.nameEn,
                    FK_CityID = model.FK_CityID,
                    isActive = true
                };


                await _districts.Entity.InsertAsync(newDistrict);
                await _districts.Save();

                return RedirectToAction("Districs", new { id = newDistrict.FK_CityID});
            }

            return View(model);
        }


        public async Task<IActionResult> EditDistrict(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var DistrictDB = await _districts.Entity.FindByIdAsync(id);

            DistrictsDTO model = new DistrictsDTO()
            {
                ID = DistrictDB.ID,
                nameAr = DistrictDB.nameAr,
                nameEn = DistrictDB.nameEn,
                isActive = DistrictDB.isActive,
                FK_CityID = DistrictDB.FK_CityID
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDistrict(DistrictsDTO model)
        {
            if (model.ID == 0)
            {
                return NotFound();
            }
            var districtdata = (await _districts.Entity.FindByIdAsync(model.ID));
            if (ModelState.IsValid)
            {
                try
                {
                    districtdata.nameAr = model.nameAr;
                    districtdata.nameEn = model.nameEn;
                    districtdata.isActive = model.isActive;

                    _districts.Entity.Update(districtdata);
                    await _cities.Save();

                }
                catch (DbUpdateConcurrencyException)
                {

                }
                return RedirectToAction(nameof(Districs), new { id = districtdata.FK_CityID });
            }

            return View(model);
        }





        [HttpPost]
        public async Task<IActionResult> ChangeStatus(int? id)
        {
            var city = await _cities.Entity.FindByIdAsync(id);
            city.isActive = !city.isActive;
            await _cities.Save();

            return Json(new { key = 1, data = city.isActive });
        }

        [HttpPost]
        public async Task<IActionResult> ChangeDistricState(int? id)
        {
            var distric = (await _districts.Entity.FindByIdAsync(id));
            distric.isActive = !distric.isActive;
            await _districts.Save();

            return Json(new { key = 1, data = distric.isActive });
        }


    }
}

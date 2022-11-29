using Core.Interfaces;
using Core.TableDb;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Salony.Enums.AllEnums;
using static Salony.Helper.Helper;

namespace Salony.Controllers
{
    public class showController : Controller
    {
        private readonly IUnitOfWork<ProviderAditionalData> _providerAditionalData;
        private readonly IUnitOfWork<MainServices> _mainServices;

        public showController(IUnitOfWork<ProviderAditionalData> providerAditionalData, IUnitOfWork<MainServices> mainServices)
        {
            _providerAditionalData = providerAditionalData;
            _mainServices = mainServices;
        }
        public async Task<IActionResult> share(int id)
        {
            ViewBag.id = id;
            var main_services = await (_mainServices.Entity.GetCustomAll(predicate: m => m.SubServices.Where(ms => ms.FK_ProviderAdditionalDataID == id).Any() && m.isActive))
                     .Select(s => new ViewMainService
                     {
                         name = CreatMessage("ar", s.nameAr, s.nameEn),
                         sub_services = s.SubServices.Where(service => service.FK_ProviderAdditionalDataID == id).Select(service => new ViewSubService
                         {
                             id = service.ID,
                             name = CreatMessage("ar", service.nameAr, service.nameEn),
                             duration = service.duration,
                             price = service.price,
                         }).ToList()
                     }).ToListAsync();



            var Boutique = _providerAditionalData.Entity.GetCustomAll(
                predicate: x => x.ID == id && x.FK_User.isActive == true && x.FK_User.activeCode == true).Select(x => new ViewBoutique
                {
                    id = x.ID,
                    branchId = x.FK_User.FK_BranchID,
                    imgs = x.SalonImages.Select(x => BaseUrlHoste + x.img).ToList(),
                    name = CreatMessage("ar", x.nameAr, x.nameEn),
                    description = CreatMessage("ar", x.descriptionAr, x.descriptionEn),
                    address = x.address,
                    instagramProfile = x.socialMediaProfile ?? "",
                    lat = x.lat,
                    lng = x.lng,
                    rate = x.rate,
                    time_from = x.timeForm.ToString("hh:mm tt"),
                    time_to = x.timeTo.ToString("hh:mm tt"),
                    url = "https://www.google.com",


                    offers = x.Offers.Select(x => BaseUrlHoste + x.img).ToList(),
                    main_services = main_services,
                    Rates = x.Orders.Where(r => r.rate != 0).Select(o => new ViewRates
                    {
                        name = o.FK_User.fullName,
                        img = o.FK_User.img,
                        rate = o.rate
                    }).ToList(),
                }).FirstOrDefault();
            if (Boutique.branchId == 5)
            {
            return View("SalonyShare",Boutique);
            }
            if (Boutique.branchId == 6)
            {
                return View("EklilShare", Boutique);
            }
            else
            {
                throw new Exception();
            }
        }


        public class ViewBoutique
        {
            public int id { get; set; }
            public int branchId { get; set; }
            public List<string> imgs { get; set; }
            public string name { get; set; }
            public string description { get; set; }
            public string address { get; set; }
            public string instagramProfile { get; set; }
            public string lat { get; set; }
            public string lng { get; set; }
            public int rate { get; set; }
            public string time_from { get; set; }
            public string time_to { get; set; }
            public string url { get; set; }
            public List<string> offers { get; set; }
            public List<ViewRates> Rates { get; set; }

            public List<ViewMainService> main_services { get; set; }
        }

        public class ViewMainService 
        {
            public string name { get; set; }
            public List<ViewSubService> sub_services { get; set; }
        }
        public class ViewSubService 
        {
            public int id { get; set; }
            public string name { get; set; }
            public double duration { get; set; }
            public double price { get; set; }
        }

        public class ViewRates
        {
            public string name { get; set; }
            public string img { get; set; }
            public int rate { get; set; }
        }
    }
}

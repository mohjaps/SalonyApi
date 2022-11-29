using Core.Interfaces;
using Core.TableDb;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Salony.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Salony.ViewModels;
using Infrastructure;
using static Salony.Helper.Helper;
using static Salony.Helper.QrCode;
using static Salony.Enums.AllEnums;

namespace Salony.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork<ApplicationDbUser> _users;
        private readonly IUnitOfWork<Branches> _branches;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger,IUnitOfWork<ApplicationDbUser> users,IUnitOfWork<Branches> branches,ApplicationDbContext db)
        {
            _logger = logger;
            _users = users;
            _branches = branches;
            _db = db;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int branchId =( await _users.Entity.FindByIdAsync(userId))?.FK_BranchID ??0;

            DashboardHomeViewModel dashboardHomeViewModel = (from st in _db.Settings
                                                             let branches = _db.Branches.Where(x=>x.isActive).Count()
                                                             let categories = _db.Categories.Where(x => x.FK_BranchID == branchId).Count()
                                                             let clients = _db.Users.Where(x => x.FK_BranchID == branchId && x.typeUser == (int)Enums.AllEnums.TypeUser.client).Count()
                                                             let providers = _db.Users.Where(x => x.FK_BranchID == branchId && x.typeUser == (int)Enums.AllEnums.TypeUser.provider).Count()
                                                             let cities = _db.Cities.Where(x => x.FK_BranchID == branchId).Count()
                                                             let copons = _db.Copons.Where(x => x.FK_Branch == branchId).Count()
                                                             let homeSliders = _db.Sliders.Where(x => x.FK_BranchID == branchId && x.type == (int)Enums.AllEnums.SliderType.home).Count()
                                                             let categoriesSliders = _db.Sliders.Where(x => x.FK_BranchID == branchId && x.type == (int)Enums.AllEnums.SliderType.salons).Count()
                                                             select new DashboardHomeViewModel
                                                             {
                                                                 branches = branches,
                                                                 categories = categories,
                                                                 clients = clients,
                                                                 providers = providers,
                                                                 cities = cities,
                                                                 copons = copons,
                                                                 homeSliders = homeSliders,
                                                                 categoriesSliders = categoriesSliders
                                                             }).FirstOrDefault();


            List<BranchHomeViewModel> Branchs = (await _branches.Entity.GetAllAsync()).Select(x => new BranchHomeViewModel
            {
                id = x.ID,
                name = x.nameAr
            }).ToList();
                

            ViewBag.Branchs = Branchs;

            ViewBag.MyBranchId = branchId;
            return View(dashboardHomeViewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> CreateBill(int id)
        {
            var orderData = await _db.Orders.Where(o => o.ID == id).Select(order => new CreateBillViewModel
            {
                id = order.ID,
                providerName = order.FK_Provider.nameAr,
                date = order.date.ToString("dd/MM/yyyy"),
                time = order.date.ToString("HH:mm tt"),
                commercialRegister = order.FK_Provider.commercialRegister,
                price = order.price,
                priceBeforeDisc = order.priceBeforeDisc,
                tax = order.price * .05,
                services = order.OrderServices.Select(service => new CreateBillServiceViewModel
                {
                    name = service.SubServicNameAr,
                    price = service.price,
                    date = service.date.ToString("dd/MM/yyyy"),
                    time = service.date.ToString("HH:mm tt"),
                    address = service.address
                }).ToList()
            }).FirstOrDefaultAsync();
            return View(orderData);
        }


        //public async Task<IActionResult> CreateBillForLady(int id)
        //{
        //    double ApplicationRatio = (_db.Settings.FirstOrDefault(s => s.FK_BranchID == (int)BranchName.Lady).appPrecent)/100;
        //    double tax = (_db.Settings.FirstOrDefault(s => s.FK_BranchID == (int)BranchName.Lady).Tax) / 100;
        //    double TaxPlusApplicationRatio = ApplicationRatio + tax;

        //    var orderData = await _db.Orders.Where(o => o.ID == id).Select(order => new CreateBillViewModel
        //    {
        //        id = order.ID,
        //        providerName = order.FK_Provider.nameAr,
        //        date = order.date.ToString("dd/MM/yyyy"),
        //        time = order.date.ToString("HH:mm tt"),
        //        commercialRegister = order.FK_Provider.commercialRegister,
        //        price = order.OrderServices.Sum(d=>Math.Round(d.price,2)),
        //        tax = tax,
        //        appPercent = ApplicationRatio,
        //        totalCut = Math.Round(TaxPlusApplicationRatio * (order.OrderServices.Sum(d => Math.Round(d.price, 2))),2),
        //        typePay = order.typePay,
        //        totalPrice = order.OrderServices.Sum(d => Math.Round(d.price, 2)) + Math.Round(TaxPlusApplicationRatio * (order.OrderServices.Sum(d => Math.Round(d.price, 2))), 2), // orderData.totalPrice = orderData.price + orderData.totalCut;
        //        services = order.OrderServices.Select(service => new CreateBillServiceViewModel
        //        {
        //            name = service.SubServicNameAr,
        //            price = service.price,
        //            date = service.date.ToString("dd/MM/yyyy"),
        //            time = service.date.ToString("HH:mm tt"),
        //            address = service.address
        //        }).ToList(),
        //        dateTime = order.date
        //    }).FirstOrDefaultAsync();
           
        //    DateTime date = orderData.dateTime;
        //    double price = Math.Round(orderData.price, 2);
        //    double vat = 15;
        //    TLVCls tlv = new TLVCls("Ladies", orderData.tax.ToString(), date, price, vat);

        //    //var Text = tlv.ToString();
        //    string text = tlv.ToBase64();
        //    ViewBag.ReturnQrImg = GenerateQrcode(text);

        //    return View(orderData);
        //}

    }
}

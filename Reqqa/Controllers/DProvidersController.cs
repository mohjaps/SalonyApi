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
    [AuthorizeRoles(Roles.Admin, Roles.Users)]
    public class DProvidersController : Controller
    {
        private readonly IUnitOfWork<ApplicationDbUser> _users;
        private readonly IUnitOfWork<ProviderAditionalData> _providerAditionalData;
        private readonly IUnitOfWork<Settings> _settings;
        private readonly IUnitOfWork<Orders> _orders;
        private readonly IUnitOfWork<Notifications> _notifications;
        private readonly IUnitOfWork<DeviceIds> _deviceIds;
        private readonly IUnitOfWork<Branches> _branches;

        public DProvidersController(IUnitOfWork<ApplicationDbUser> users, IUnitOfWork<ProviderAditionalData> providerAditionalData, IUnitOfWork<Settings> settings,
            IUnitOfWork<Orders> orders, IUnitOfWork<Notifications> notifications, IUnitOfWork<DeviceIds> deviceIds, IUnitOfWork<Branches> branches)
        {
            this._users = users;
            this._providerAditionalData = providerAditionalData;
            this._settings = settings;
            this._orders = orders;
            this._notifications = notifications;
            this._deviceIds = deviceIds;
            this._branches = branches;

        }


        public async Task<IActionResult> Index()
        {

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userFk_BranchID = await _users.Entity.GetAsync(predicate: b => b.Id == userId);
            var providerAdditiondata = await _providerAditionalData.Entity.GetAsync(predicate: b => b.FK_UserID == userId);

            var settingApp = (await _settings.Entity.GetAsync(predicate: x => x.FK_BranchID == userFk_BranchID.FK_BranchID));
            List<ProviderAditionalDataDTO> UserDB = null;
            ViewBag.branchId = userFk_BranchID.FK_BranchID;

            //var Sliders = await _sliders.Entity
            if (providerAdditiondata != null)
            {
                UserDB = (await _users.Entity.GetAllAsync(predicate: s => s.typeUser == (int)TypeUser.provider && s.FK_BranchID == userFk_BranchID.FK_BranchID,
                                                       include: source => source.Include(i => i.ProviderAditionalData).ThenInclude(i => i.Orders).Include(i => i.ProviderAditionalData).ThenInclude(i => i.FK_District).ThenInclude(i => i.FK_City),
                                                       orderBy: source => source.OrderByDescending(o => o.registerDate))).Select(x => new ProviderAditionalDataDTO
                                                       {
                                                           id = x.Id,
                                                           Email = x.Email,
                                                           PhoneNumber = x.PhoneNumber,
                                                           fullName = x.fullName,
                                                           img = x.img,
                                                           registerDateToSort = x.registerDate != null ? x.registerDate : DateTime.Now,
                                                           registerDate = x.registerDate != null ? x.registerDate.ToString("dd/MM/yyyy") : "",
                                                           lang = x.lang,
                                                           address = x.address,
                                                           isActive = x.isActive,
                                                           isDeleted = x.isDeleted,
                                                           code = x.code,
                                                           password = x.showPassword,
                                                           UserIbanNumber = !string.IsNullOrEmpty(x.ibanNumber) ?
                                                           x.ibanNumber : "لايوجد رقم ايبان",
                                                           ProviderID = x.ProviderAditionalData.ID,
                                                           nameAr = x.ProviderAditionalData.nameAr,
                                                           nameEn = x.ProviderAditionalData.nameEn,
                                                           descriptionAr = x.ProviderAditionalData.descriptionAr,
                                                           rate = x.ProviderAditionalData.rate,
                                                           timeTo = x.ProviderAditionalData.timeTo,
                                                           timeForm = x.ProviderAditionalData.timeForm,

                                                           certificatePhoto = x.certificatePhoto,
                                                           bankAccount = x.ProviderAditionalData.bankAccount,

                                                           destrict = x.ProviderAditionalData.FK_District.nameAr,
                                                           city = x.ProviderAditionalData.FK_District.FK_City.nameAr,
                                                           neededCommission = x.ProviderAditionalData.Orders.Where(order => order.Applicationpercentagepaid == false).Count() * settingApp.appPrecent,
                                                           neededWalletCommission = x.ProviderAditionalData.Orders.Where(order => order.ApplicationProviderpercentagepaid == false).Select(x => x.Providerpercentage).Sum(),
                                                           wallet = x.wallet,
                                                           userWallet = x.userWallet,
                                                           //---------
                                                           allDdeposit = x.ProviderAditionalData.Orders.Where(order => order.ApplicationProviderpercentagepaid == false && (order.status == (int)Enums.AllEnums.OrderStates.accepted || order.status == (int)Enums.AllEnums.OrderStates.finished)).Select(x => x.Deposit).Sum(),
                                                           allCommission = (x.ProviderAditionalData.Orders.Where(order => order.ApplicationProviderpercentagepaid == false && (order.status == (int)Enums.AllEnums.OrderStates.accepted || order.status == (int)Enums.AllEnums.OrderStates.finished)).Select(x => x.Deposit).Sum() - x.ProviderAditionalData.Orders.Where(order => order.ApplicationProviderpercentagepaid == false && (order.status == (int)Enums.AllEnums.OrderStates.accepted || order.status == (int)Enums.AllEnums.OrderStates.finished)).Select(x => x.AppCommission).Sum()),

                                                           //-------
                                                           IdentityImage = Helper.Helper.BaseUrlHoste + x.ProviderAditionalData.IdentityImage,
                                                           commercialRegister = x.ProviderAditionalData.commercialRegister,
                                                           CommercialRegisterImage = Helper.Helper.BaseUrlHoste + x.ProviderAditionalData.CommercialRegisterImage,
                                                           HealthCardImage = Helper.Helper.BaseUrlHoste + x.ProviderAditionalData.HealthCardImage,
                                                           IbanNumber = x.ProviderAditionalData.IbanNumber,
                                                           IbanImage = Helper.Helper.BaseUrlHoste + x.ProviderAditionalData.IbanImage,
                                                           //-------

                                                       }).OrderByDescending(x => x.registerDateToSort).ToList();

            }
            else
            {
                UserDB = (await _users.Entity.GetAllAsync(predicate: s => s.typeUser == (int)TypeUser.provider && s.FK_BranchID == userFk_BranchID.FK_BranchID,
                                                   include: source => source.Include(i => i.ProviderAditionalData).ThenInclude(i => i.Orders).Include(i => i.ProviderAditionalData).ThenInclude(i => i.FK_District).ThenInclude(i => i.FK_City),
                                                   orderBy: source => source.OrderByDescending(o => o.registerDate))).Select(x => new ProviderAditionalDataDTO
                                                   {
                                                       id = x.Id,
                                                       Email = x.Email,
                                                       PhoneNumber = x.PhoneNumber,
                                                       fullName = x.fullName,
                                                       img = x.img,
                                                       registerDateToSort = x.registerDate != null ? x.registerDate : DateTime.Now,
                                                       registerDate = x.registerDate != null ? x.registerDate.ToString("dd/MM/yyyy") : "",
                                                       lang = x.lang,
                                                       address = x.address,
                                                       isActive = x.isActive,
                                                       isDeleted = x.isDeleted,
                                                       code = x.code,
                                                       password = x.showPassword,
                                                       UserIbanNumber = !string.IsNullOrEmpty(x.ibanNumber) ?
                                                       x.ibanNumber : "لايوجد رقم ايبان",

                                                       certificatePhoto = x.certificatePhoto,
                                                       wallet = x.wallet,
                                                       userWallet = x.userWallet

                                                   }).OrderByDescending(x => x.registerDateToSort).ToList();
            }


            return View(UserDB);

        }


        [HttpPost]
        public async Task<bool> ChangeStatus(string id)
        {
            var Data = await _users.Entity.FindByIdAsync(id);

            Data.isActive = !Data.isActive;

            await _users.Save();
            var projectName = await _branches.Entity.GetCustomAll(x => x.ID == Data.FK_BranchID).Select(x => x.nameAr + " | " + x.nameEn).FirstOrDefaultAsync();
            var devicesIds = await _deviceIds.Entity.GetCustomAll(predicate: x => x.FK_UserID == id).ToListAsync();

            var userIds = devicesIds.Select(x => x.deviceID).ToList();
            SendPushNotification(device_ids: userIds, msg: CreatMessage(Data.lang, "تم حظرك من قبل الادارة", "your account blocked from admin"), type: (int)Enums.AllEnums.FcmType.blockUser, branchId: Data.FK_BranchID, projectName: projectName);
            _deviceIds.Entity.DeleteRange(devicesIds);
            await _deviceIds.Save();


            return Data.isActive;
        }

        public async Task<IActionResult> deleteUser(string id)
        {
            try
            {
                var aditionalData = (await _providerAditionalData.Entity.GetAllAsync(predicate: x => x.FK_UserID == id)).FirstOrDefault();
                _providerAditionalData.Entity.Delete(aditionalData);
                var user = await _users.Entity.FindByIdAsync(id);
                _users.Entity.Delete(user);
                var orders = (await _orders.Entity.GetAllAsync(predicate: x => x.FK_Provider.FK_UserID == id || x.FK_UserID == id)).ToList();
                _orders.Entity.DeleteRange(orders);
                var notify = (await _notifications.Entity.GetAllAsync(predicate: x => x.FK_Order.FK_Provider.FK_UserID == id)).ToList();
                _notifications.Entity.DeleteRange(notify);

                await _users.Save();

            }
            catch (Exception e)
            {

                var xx = 0;
            }

            return Json(new { key = 1 });
        }




        [HttpPost]
        public async Task<IActionResult> payCommission(string id)
        {


            var orders = await _orders.Entity.GetAllAsync(predicate: x => x.Applicationpercentagepaid == false && (x.status == (int)Enums.AllEnums.OrderStates.accepted || x.status == (int)Enums.AllEnums.OrderStates.finished) && x.FK_Provider.FK_UserID == id);
            foreach (var item in orders)
            {
                item.Applicationpercentagepaid = true;
                _orders.Entity.Update(item);
            }

            await _orders.Save();
            return Json(new { key = 1 });

        }


        [HttpPost]
        public async Task<IActionResult> payWalletCommission(string id, double price)
        {


            var orders = await _orders.Entity.GetAllAsync(predicate: x => x.ApplicationProviderpercentagepaid == false && (x.status == (int)Enums.AllEnums.OrderStates.accepted || x.status == (int)Enums.AllEnums.OrderStates.finished) && x.FK_Provider.FK_UserID == id);
            foreach (var item in orders)
            {
                item.ApplicationProviderpercentagepaid = true;
                _orders.Entity.Update(item);
            }
            var user = await _users.Entity.GetAsync(predicate: x => x.Id == id);
            user.wallet += price;
            _users.Entity.Update(user);
            await _orders.Save();
            return Json(new { key = 1 });

        }

        [HttpPost]
        public async Task<IActionResult> emptyWallet(string id)
        {


            var user = await _users.Entity.GetAsync(predicate: x => x.Id == id);
            user.wallet = 0;
            _users.Entity.Update(user);
            await _users.Save();
            return Json(new { key = 1 });

        }


        [HttpPost]
        public async Task<IActionResult> getDetails(string id)
        {
            var settingAppPrecent = (await _settings.Entity.GetAsync(predicate: x => x.FK_BranchID == 6)).appPrecent;

            var boutiques = (await _providerAditionalData.Entity.GetAllAsync(
                predicate: x => x.FK_UserID == id,
                include: source => source.Include(i => i.FK_User).Include(i => i.SubServices)
                                         .ThenInclude(i => i.FK_MainService).ThenInclude(i => i.SubServices),
                disableTracking: false
                )).Select(x => new
                {
                    main_services = x.SubServices.Where(s => s.isActive == true).GroupBy(s => s.FK_MainServiceID).Select(s => s.FirstOrDefault())
                    .Where(s => s.FK_MainService.isActive == true)
                    .Select(s => new
                    {
                        name = s.FK_MainService.nameAr,
                        sub_services = s.FK_MainService.SubServices.Where(service => service.FK_ProviderAdditionalDataID == x.ID).Select(service => new
                        {
                            name_ar = CreatMessage("ar", service.nameAr, service.nameEn),
                            price = service.price - settingAppPrecent,
                        }).ToList()
                    }).ToList()
                }).FirstOrDefault();
            if (boutiques != null)
            {
                return Json(new { key = 1, data = boutiques.main_services });
            }

            return Json(new { key = 0 });
        }

    }
}

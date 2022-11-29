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
    [AuthorizeRoles(Roles.Users, Roles.Admin)]
    public class DClientsController : Controller
    {

        private readonly IUnitOfWork<ApplicationDbUser> _users;
        private readonly IUnitOfWork<DeviceIds> _deviceIds;
        private readonly IUnitOfWork<Branches> _branches;
        private readonly IUnitOfWork<Orders> _orders;
        private readonly IUnitOfWork<Notifications> _notifications;
        private readonly IUnitOfWork<Carts> _carts;

        public DClientsController(IUnitOfWork<ApplicationDbUser> users, IUnitOfWork<DeviceIds> deviceIds, IUnitOfWork<Branches> branches, IUnitOfWork<Orders> orders,
            IUnitOfWork<Notifications> notifications, IUnitOfWork<Carts> carts)
        {
            this._users = users;
            this._deviceIds = deviceIds;
            this._branches = branches;
            this._orders = orders;
            this._notifications = notifications;
            this._carts = carts;

        }


        public async Task<IActionResult> Index()
        {

            //var usersTemp = await _users.Entity.GetAllAsync(predicate: u => u.FK_BranchID == (int)BranchName.ToGo);

            //foreach (var item in usersTemp)
            //{
            //    string invCode = "";

            //    // generate code
            //    Random rnd = new Random();
            //    ApplicationDbUser res = new ApplicationDbUser();
            //    do
            //    {
            //        invCode = rnd.Next(100000, 999999).ToString();
            //        res = await _users.Entity.GetAsync(u => u.invitationCode == invCode);
            //    } while (res != null);

            //    item.invitationCode = invCode;


            //}
            //_users.Entity.UpdateRange(usersTemp as List<ApplicationDbUser>);
            //await _users.Save();



            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userFk_BranchID = await _users.Entity.GetAsync(predicate: b => b.Id == userId);
            ViewBag.branchId = userFk_BranchID.FK_BranchID;
            //var Sliders = await _sliders.Entity
            var UserDB = await _users.Entity.GetCustomAll(predicate: s => s.typeUser == (int)TypeUser.client && s.FK_BranchID == userFk_BranchID.FK_BranchID).Select(x => new UserDBDTO
            {
                id = x.Id,
                Email = x.Email,
                registerDate = x.registerDate.ToString("dd/MM/yyyy"),
                registerDateToSort = x.registerDate,
                PhoneNumber = x.PhoneNumber,
                fullName = x.fullName,
                img = x.img,
                lang = x.lang,
                address = x.address,
                code = x.code,
                userWallet = x.userWallet,
                isActive = x.isActive,
                isDeleted = x.isDeleted,
                cartCount = x.Carts.Count(),
                invitationCodeBallance = x.invitationCode
            }).OrderByDescending(x => x.registerDateToSort).ToListAsync();
            return View(UserDB);

        }

        public async Task<IActionResult> CartCountIndex()
        {

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userFk_BranchID = await _users.Entity.GetAsync(predicate: b => b.Id == userId);
            ViewBag.branchId = userFk_BranchID.FK_BranchID;
            //var Sliders = await _sliders.Entity
            var UserDB = await _users.Entity.GetCustomAll(predicate: s => s.typeUser == (int)TypeUser.client && s.FK_BranchID == userFk_BranchID.FK_BranchID && s.Carts.Count() != 0).Select(x => new UserDBDTO
            {
                id = x.Id,
                Email = x.Email,
                registerDate = x.Carts.Select(x => x.date).Max().ToString("dd/MM/yyyy"),
                registerDateToSort = x.Carts.Select(x => x.date).Max(),
                PhoneNumber = x.PhoneNumber,
                fullName = x.fullName,
                img = x.img,
                lang = x.lang,
                address = x.address,
                code = x.code,
                userWallet = x.userWallet,
                isActive = x.isActive,
                isDeleted = x.isDeleted,
                cartCount = x.Carts.Count()
            }).OrderByDescending(x => x.registerDateToSort).ToListAsync();
            return View(UserDB);

        }

        public async Task<IActionResult> CartDetails(string id)
        {
            ViewBag.Name = (await _users.Entity.FindByIdAsync(id)).fullName;

            var cart = await _carts.Entity.GetCustomAll(c => c.FK_UserID == id).Select(c => new CartDetailsForClientDto
            {
                serviceNameAr = c.FK_SubService.nameAr,
                serviceNamwEn = c.FK_SubService.nameEn,
                mainServiceNameAr = c.FK_SubService.FK_MainService.nameAr,
                mainServiceNamwEn = c.FK_SubService.FK_MainService.nameEn,
                date = c.date.ToString("dd/MM/yyyy"),
                price = c.FK_SubService.price,
                duration = c.FK_SubService.duration
            }).ToListAsync();
            return View(cart);
        }

        [HttpPost]
        public async Task<bool> ChangeStatus(string id)
        {
            var Data = await _users.Entity.FindByIdAsync(id);

            Data.isActive = !Data.isActive;
            await _users.Save();
            if (Data.isActive == false)
            {
                var projectName = await _branches.Entity.GetCustomAll(x => x.ID == Data.FK_BranchID).Select(x => x.nameAr + " | " + x.nameEn).FirstOrDefaultAsync();

                var devicesIds = await _deviceIds.Entity.GetCustomAll(predicate: x => x.FK_UserID == id).ToListAsync();

                var userIds = devicesIds.Select(x => x.deviceID).ToList();
                SendPushNotification(device_ids: userIds, msg: CreatMessage(Data.lang, "تم حظرك من قبل الادارة", "your account blocked from admin"), type: (int)Enums.AllEnums.FcmType.blockUser, branchId: Data.FK_BranchID, projectName: projectName);
                _deviceIds.Entity.DeleteRange(devicesIds);
                await _deviceIds.Save();
            }
            return Data.isActive;
        }

        [HttpPost]
        public async Task<IActionResult> IncreaseWallet(string id, double price)
        {
            var Data = await _users.Entity.FindByIdAsync(id);

            Data.userWallet = Data.userWallet + price;
            await _users.Save();

            var projectName = await _branches.Entity.GetCustomAll(x => x.ID == Data.FK_BranchID).Select(x => x.nameAr + " | " + x.nameEn).FirstOrDefaultAsync();

            var devicesIds = await _deviceIds.Entity.GetCustomAll(predicate: x => x.FK_UserID == id).ToListAsync();

            var userIds = devicesIds.Select(x => x.deviceID).ToList();
            SendPushNotification(device_ids: userIds, msg: CreatMessage(Data.lang, $"تم اضافة مبلغ {price} الى المحفظة من قبل الادارة", $"your wallet has been charged with {price} from admin"), type: (int)Enums.AllEnums.FcmType.dashboard, branchId: Data.FK_BranchID, projectName: projectName);
            return Json(new { key = 1 });
        }

        [HttpPost]
        public async Task<IActionResult> DicreaseWallet(string id, double price)
        {
            var Data = await _users.Entity.FindByIdAsync(id);
            if (Data.userWallet < price)
            {
                return Json(new { key = 0 });

            }
            Data.userWallet = Data.userWallet - price;
            await _users.Save();

            var projectName = await _branches.Entity.GetCustomAll(x => x.ID == Data.FK_BranchID).Select(x => x.nameAr + " | " + x.nameEn).FirstOrDefaultAsync();

            var devicesIds = await _deviceIds.Entity.GetCustomAll(predicate: x => x.FK_UserID == id).ToListAsync();

            var userIds = devicesIds.Select(x => x.deviceID).ToList();
            SendPushNotification(device_ids: userIds, msg: CreatMessage(Data.lang, $"تم خصم مبلغ {price} من المحفظة من قبل الادارة", $"your wallet has been decreased with {price} from admin"), type: (int)Enums.AllEnums.FcmType.dashboard, branchId: Data.FK_BranchID, projectName: projectName);
            return Json(new { key = 1 });
        }

        public async Task<IActionResult> deleteUser(string id)
        {
            try
            {
                var user = await _users.Entity.FindByIdAsync(id);
                _users.Entity.Delete(user);
                var orders = (await _orders.Entity.GetAllAsync(predicate: x => x.FK_UserID == id)).ToList();
                _orders.Entity.DeleteRange(orders);
                var carts = (await _carts.Entity.GetAllAsync(predicate: x => x.FK_UserID == id)).ToList();
                _carts.Entity.DeleteRange(carts);
                var notify = (await _notifications.Entity.GetAllAsync(predicate: x => x.FK_UserID == id || x.FK_Order.FK_UserID == id)).ToList();
                _notifications.Entity.DeleteRange(notify);

                await _users.Save();

            }
            catch (Exception e)
            {

                var xx = 0;
            }
            return Json(new { key = 1 });
        }


    }
}

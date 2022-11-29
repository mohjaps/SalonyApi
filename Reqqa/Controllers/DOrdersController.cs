using Core.Interfaces;
using Core.TableDb;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Salony.Models.ControllerDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static Salony.Enums.AllEnums;
using static Salony.Helper.Helper;
using static Salony.PathUrl.ApiRoutes;

namespace Salony.Controllers
{
    [AuthorizeRoles(Roles.Admin, Roles.Orders)]
    public class DOrdersController : Controller
    {

        private readonly IUnitOfWork<ApplicationDbUser> _users;
        private readonly IUnitOfWork<Orders> _orders;
        private readonly IUnitOfWork<Messages> _messages;
        private readonly IUnitOfWork<Settings> _settings;
        private readonly IUnitOfWork<ProviderAditionalData> _providerAditionalData;

        public DOrdersController(IUnitOfWork<ApplicationDbUser> users, IUnitOfWork<Cities> cities, IUnitOfWork<Orders> orders, IUnitOfWork<Messages> messages, IUnitOfWork<Settings> settings, IUnitOfWork<ProviderAditionalData> providerAditionalData)
        {
            this._users = users;
            this._orders = orders;
            this._messages = messages;
            _settings = settings;
            _providerAditionalData = providerAditionalData;
        }

        public async Task<IActionResult> Index(DateTime? startdate, DateTime? enddate, int? id, string userId = null,string provider = null, int status = 5)
        {
            //if (id == null)
            //{
            //    return NotFound();
            //}

            string userIdnew = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userFk_BranchID = await _users.Entity.GetAsync(predicate: b => b.Id == userIdnew);
            ProviderAditionalData providerAditionalData = await _providerAditionalData.Entity.GetAsync(dd => dd.FK_UserID == userIdnew);
            var settingAppPrecent = (await _settings.Entity.GetAsync(predicate: x => x.FK_BranchID == userFk_BranchID.FK_BranchID));
            ViewBag.settingAppPrecent = settingAppPrecent.appPrecent;

            if (providerAditionalData != null)
            {
                ViewBag.userName = !String.IsNullOrEmpty(provider) ? _users.Entity.GetAsync(predicate: u => u.Id == provider, include: source => source.Include(i => i.ProviderAditionalData)).GetAwaiter().GetResult().ProviderAditionalData.nameAr : "";



                ViewData["users"] = new SelectList(_users.Entity.GetAllAsync(u => (u.isActive && u.typeUser == (int)TypeUser.provider && u.FK_BranchID == userFk_BranchID.FK_BranchID), include: u => u.Include(i => i.ProviderAditionalData)).GetAwaiter().GetResult().Select(u => new
                {
                    Id = u.Id,
                    nameAr = u.ProviderAditionalData.nameAr
                }), "Id", "nameAr", provider);

            }
            else
            {
                ViewBag.userName = (await _users.Entity.FindByIdAsync(userIdnew)).fullName;



                ViewData["users"] = new SelectList(_users.Entity.GetAllAsync(u => (u.isActive && u.typeUser == (int)TypeUser.provider && u.FK_BranchID == userFk_BranchID.FK_BranchID)).GetAwaiter().GetResult().Select(u => new
                {
                    Id = u.Id,
                    nameAr = u.fullName
                }), "Id", "nameAr", provider);


            }


            var orders = await _orders.Entity.GetAllAsync(predicate: x => (id != null ? x.FK_ProviderID == id : true)
                                                         && (!string.IsNullOrEmpty(userId) ? (x.FK_UserID == userId) : true)
                                                         && (!string.IsNullOrEmpty(provider) ? (x.FK_Provider.FK_User.Id == provider) : true)

                                                         &&(startdate != null ? x.date.Date >= startdate.Value.Date : true)
                                                         && (enddate != null ? x.date.Date <= enddate.Value.Date : true)

                                                         && (status != 5 ? x.status == status : true)
                                                         && !x.IsDeleted
                                                         && (x.FK_User.FK_BranchID == userFk_BranchID.FK_BranchID),
                                                         include: source => source.Include(i => i.FK_User).Include(i => i.FK_Provider).ThenInclude(i => i.FK_User)
                                                                                    .Include(i => i.OrderServices),
                                                         orderBy: o => o.OrderByDescending(x => x.ID));

            ViewData["ordersCount"] = await _orders.Entity.GetCustomAll(predicate: x => (id != null ? x.FK_ProviderID == id : true)
                                                        && (!string.IsNullOrEmpty(userId) ? x.FK_UserID == userId : true)
                                                                                                                 && (startdate != null ? x.date.Date >= startdate.Value.Date : true)
                                                         && (enddate != null ? x.date.Date <= enddate.Value.Date : true)


                                                        && (x.FK_User.FK_BranchID == userFk_BranchID.FK_BranchID)
                                                        && (status != 5 ? x.status == status : true)).CountAsync();

            ViewData["id"] = id;
            ViewData["userId"] = userId;


            ViewData["startdate"] = startdate != null ?  startdate.Value.Date.ToString("yyyy-MM-dd") : "";
            ViewData["enddate"] = enddate != null ? enddate.Value.Date.ToString("yyyy-MM-dd") : "";
            ViewData["status"] = status ;

            return View(orders);
        }

        public async Task<IActionResult> SalonyNewOrders()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userFk_BranchID = await _users.Entity.GetAsync(predicate: b => b.Id == userId);

            return View(await _orders.Entity.GetAllAsync(predicate: x => !x.Applicationpercentagepaid && x.ApplicationpercentageImg != null && x.FK_User.FK_BranchID == userFk_BranchID.FK_BranchID,
                include: source => source.Include(i => i.FK_User).Include(i => i.FK_Provider).ThenInclude(i => i.FK_User),
                orderBy: o => o.OrderByDescending(x => x.ID)));
        }

        public async Task<IActionResult> OrderDetails(int id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userFk_BranchID = await _users.Entity.GetAsync(predicate: b => b.Id == userId);
            var order = await _orders.Entity.GetAsync(predicate: x => x.ID == id && x.FK_User.FK_BranchID == userFk_BranchID.FK_BranchID,
                include: source => source.Include(i => i.OrderServices));
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }


        [HttpPost]
        public async Task<IActionResult> acceptApplicationpercentageImg(int id)
        {


            var order = await _orders.Entity.GetAsync(predicate: x => x.ID == id, include: source => source.Include(i => i.FK_Provider));
            order.Applicationpercentagepaid = true;
            _orders.Entity.Update(order);
            await _messages.Entity.InsertAsync(new Messages()
            {
                Text = $"يمكنك التواصل على طلبك رقم {order.ID}",
                TypeMessage = (int)Enums.AllEnums.FileTypeChat.text,
                DateSend = GetCurrentDate(),
                FK_OrderId = order.ID,
                ReceiverId = order.FK_UserID,
                SenderId = order.FK_Provider.FK_UserID,
            });


            await _orders.Save();

            return Json(new { key = 1 });

        }

        [HttpPost]
        public async Task<IActionResult> RefuseApplicationpercentageImg(int id)
        {


            var order = await _orders.Entity.GetAsync(predicate: x => x.ID == id);
            order.Applicationpercentagepaid = false;
            order.ApplicationpercentageImg = null;
            _orders.Entity.Update(order);
            await _orders.Save();

            return Json(new { key = 1 });

        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind("date,status,typePay,copon,discountPercentage,priceBeforeDisc,price,paid,rate,refusedReason,FK_ProviderID")] OrderDTO model)
        {
            if (ModelState.IsValid)
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                model.FK_UserID = userId;

                Orders newOrder = new Orders()
                {
                    date = model.date,
                    status = model.status,
                    typePay = model.typePay,
                    copon = model.copon,
                    discountPercentage = model.discountPercentage,
                    priceBeforeDisc = model.priceBeforeDisc,
                    price = model.price,
                    paid = model.paid,
                    rate = model.rate,
                    refusedReason = model.refusedReason,
                    FK_ProviderID = model.FK_ProviderID,
                    FK_UserID = model.FK_UserID
                };


                await _orders.Entity.InsertAsync(newOrder);
                await _orders.Save();

                return RedirectToAction("Index", new { id = newOrder.FK_ProviderID });
            }

            return View(model);
        }


        // GET: DOrders2/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orders = await _orders.Entity.FindByIdAsync(id);
            if (orders == null)
            {
                return NotFound();
            }

            OrderDTO EditOrder = new OrderDTO();
            EditOrder.ID = orders.ID;
            EditOrder.date = orders.date;
            EditOrder.status = orders.status;
            EditOrder.typePay = orders.typePay;
            EditOrder.copon = orders.copon;
            EditOrder.discountPercentage = orders.discountPercentage;
            EditOrder.priceBeforeDisc = orders.priceBeforeDisc;
            EditOrder.price = orders.price;
            EditOrder.paid = orders.paid;
            EditOrder.rate = orders.rate;
            EditOrder.refusedReason = orders.refusedReason;
            EditOrder.FK_ProviderID = orders.FK_ProviderID;
            EditOrder.FK_UserID = orders.FK_UserID;

            return View(EditOrder);
        }

        // POST: DOrders2/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("date,status,typePay,returnMoney,copon,discountPercentage,priceBeforeDisc,price,paid,rate,refusedReason,FK_ProviderID")] OrderDTO model)
        {
            if (id != model.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                model.FK_UserID = userId;
                try
                {
                    var oldOrder = await _orders.Entity.FindByIdAsync(model.ID);

                    oldOrder.ID = id;
                    oldOrder.date = model.date;
                    oldOrder.status = model.status;
                    oldOrder.typePay = model.typePay;
                    oldOrder.copon = model.copon;
                    oldOrder.discountPercentage = model.discountPercentage;
                    oldOrder.priceBeforeDisc = model.priceBeforeDisc;
                    oldOrder.price = model.price;
                    oldOrder.paid = model.paid;
                    oldOrder.rate = model.rate;
                    oldOrder.refusedReason = model.refusedReason;
                    oldOrder.FK_ProviderID = model.FK_ProviderID;
                    oldOrder.FK_UserID = userId;

                    _orders.Entity.Update(oldOrder);
                    await _orders.Save();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await OrdersExists(model.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }



        private async Task<bool> OrdersExists(int id)
        {

            return await _orders.Entity.FindByIdAsync(id) == null;
        }


        [HttpPost]
        public async Task<int> ChangeStatus(int id)
        {
            var Data = await _orders.Entity.FindByIdAsync(id);

            if (Data.status == 1)
                Data.status = 0;
            else
                Data.status = 1;

            await _orders.Save();

            return Data.status;
        }

        public async Task<ActionResult> Conversation(int id)
        {
            var listMessages = await _messages.Entity.GetCustomAll(predicate: m => m.FK_OrderId == id).Select(m => new Salony.Models.ControllerDTO.ListMessageTwoUsersToAdminViewModel
            {
                Id = m.Id,
                SenderId = m.SenderId,
                SenderName = m.Sender.fullName,
                ReceiverId = m.ReceiverId,
                ReceiverName = m.Receiver.fullName,
                Message = m.Text,
                Date = m.DateSend.ToString("dd/MM/yyyy h:mm tt")
            }).ToListAsync();


            ViewBag.FirstMessage = listMessages.Select(x => x.SenderId).FirstOrDefault();
            return View(listMessages);
        }

        [HttpPost]
        public async Task<int> changeOrderStatus(int id,int status)
        {
            var Data = await _orders.Entity.FindByIdAsync(id);

            if (Data.status == 0)
                Data.status = status;

            await _orders.Save();

            return 1;
        }



    }
}

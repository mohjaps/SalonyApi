using Core.Interfaces;
using Core.TableDb;
using DinkToPdf;
using DinkToPdf.Contracts;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Salony.Models.ApiDTO.Provider;
using Salony.PathUrl;
using Salony.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static Salony.Enums.AllEnums;
using static Salony.Helper.Helper;


namespace Salony.Controllers.API
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiExplorerSettings(GroupName = "WebApi")]
    public class ProviderController : Controller
    {
        private readonly IWebHostEnvironment _HostingEnvironment;
        private readonly IUnitOfWork<Categories> _categories;
        private readonly IUnitOfWork<Cities> _cities;
        private readonly IUnitOfWork<ApplicationDbUser> _users;
        private readonly IUnitOfWork<SubServices> _subServices;
        private readonly IUnitOfWork<ProviderAditionalData> _providerAditionalData;
        private readonly IUnitOfWork<Offers> _offers;
        private readonly IUnitOfWork<Orders> _orders;
        private readonly IUnitOfWork<Settings> _settings;
        private readonly IUnitOfWork<Notifications> _notifications;
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork<DeviceIds> _deviceIds;
        private readonly IUnitOfWork<Branches> _branches;
        private readonly IUnitOfWork<Messages> _messages;
        private readonly IUnitOfWork<SalonImages> _salonImages;
        private readonly IUnitOfWork<FinancialAccount> _financialAccount;
        private readonly IUnitOfWork<Employees> _employees;

        private IConverter _converter;

        public ProviderController(IWebHostEnvironment HostingEnvironment, IUnitOfWork<Categories> categories, IUnitOfWork<Cities> cities, IUnitOfWork<ApplicationDbUser> users, IUnitOfWork<SubServices> subServices,
            IUnitOfWork<ProviderAditionalData> providerAditionalData, IUnitOfWork<Offers> offers, IUnitOfWork<Orders> orders, IUnitOfWork<Settings> settings,
            IUnitOfWork<Notifications> notifications, ApplicationDbContext context, IUnitOfWork<DeviceIds> deviceIds, IUnitOfWork<Branches> branches, IUnitOfWork<Messages> messages, IUnitOfWork<SalonImages> salonImages, IUnitOfWork<FinancialAccount> financialAccount,
            IUnitOfWork<Employees> employees, IConverter converter)
        {
            _HostingEnvironment = HostingEnvironment;
            _categories = categories;
            _cities = cities;
            _users = users;
            _subServices = subServices;
            _providerAditionalData = providerAditionalData;
            _offers = offers;
            _orders = orders;
            _settings = settings;
            _notifications = notifications;
            _context = context;
            _deviceIds = deviceIds;
            _messages = messages;
            _branches = branches;
            _salonImages = salonImages;
            _converter = converter;
            _financialAccount = financialAccount;
            _employees = employees;

        }

        [AllowAnonymous]
        [HttpPost(ApiRoutes.Provider.GetRegisterData)]
        public async Task<ActionResult> GetRegisterData([FromForm] GetRegisterDataDTO model)
        {
            string userId = User.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;

            var categories = (await _categories.Entity.GetAllAsync(
                predicate: x => x.isActive == true && x.FK_BranchID == model.branch_id
                )).Select(x => new
                {
                    id = x.ID,
                    name = CreatMessage(model.lang, x.nameAr, x.nameEn),
                    img = BaseUrlHoste + x.img,
                }).ToList();
            var cities = (await _cities.Entity.GetAllAsync(predicate: x => x.isActive == true && x.FK_BranchID == model.branch_id,
                include: source => source.Include(i => i.Districts))).Select(city => new
                {
                    id = city.ID,
                    name = CreatMessage(model.lang, city.nameAr, city.nameEn),
                    districts = city.Districts.Select(district => new
                    {
                        id = district.ID,
                        name = CreatMessage(model.lang, district.nameAr, district.nameEn),
                    })
                });
            Settings conditions = await _settings.Entity.GetAsync(x => x.FK_BranchID == model.branch_id);
            return Ok(new
            {
                key = 1,
                categories = categories,
                cities = cities,
                conditions = CreatMessage(model.lang, conditions.condtionsAr, conditions.condtionsEn)
            });
        }

        [HttpPost(ApiRoutes.Provider.ListProviderMainServices)]
        public async Task<ActionResult> ListProviderMainServices(ListProviderMainServicesDTO model)
        {
            string userId = User.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;
            int branchId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == "branch_id")?.Value ?? "0");

            //var categories = _categories.Entity.GetAllAsync(predicate: x=>x.FK_BranchID)

            var categories = (await _users.Entity.GetAllAsync(predicate: x => x.Id == userId, include: source => source.Include(i => i.ProviderAditionalData).ThenInclude(i => i.FK_Category).ThenInclude(x => x.MainServices)))
                .Select(x => x.ProviderAditionalData.FK_Category.MainServices.Select(s => new
                {
                    id = s.ID,
                    name = CreatMessage(model.lang, s.nameAr, s.nameEn)
                }).ToList()).FirstOrDefault();

            return Ok(new
            {
                key = 1,
                categories = categories,
            });
        }

        [HttpPost(ApiRoutes.Provider.AddSubService)]
        public async Task<ActionResult> AddSubService(AddSubServiceDTO model)
        {
            string userId = User.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;
            int branchId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == "branch_id")?.Value ?? "0");

            Boolean ifExists = (await _context.Workers.FirstOrDefaultAsync(x => x.ID != model.WorkerID)) != null;
            if (!ifExists)
                throw new Exception("worker doesn't exists");

            double settingAppPrecent = (await _settings.Entity.GetAsync(predicate: x => x.FK_BranchID == branchId)).appPrecent;
            ApplicationDbUser user = await _users.Entity.GetAsync(predicate: x => x.Id == userId, include: source => source.Include(i => i.ProviderAditionalData));

            SubServices subServices = new SubServices()
            {
                nameAr = model.name_ar,
                nameEn = model.name_en,
                duration = model.duration,
                price = model.price + settingAppPrecent,
                Image = model.image != null ? Helper.Helper.ProcessUploadedFile(_HostingEnvironment, model.image, "SubService") : null,
                DescriptionAr = model.descriptionAr,
                DescriptionEn = model.descriptionEn,
                FK_MainServiceID = model.main_service,
                FK_ProviderAdditionalDataID = user.ProviderAditionalData.ID,
                isActive = true,
                WokerID = model.WorkerID
            };
            await _subServices.Entity.InsertAsync(subServices);
            await _subServices.Save();

            return Ok(new
            {
                key = 1,
                msg = CreatMessage(model.lang, "تم اضافة الخدمة بنجاح", "service added successfully"),
            });

        }

        [HttpPost(ApiRoutes.Provider.EditSubService)]
        public async Task<ActionResult> EditSubService(EditSubServiceDTO model)
        {
            string userId = User.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;
            int branchId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == "branch_id")?.Value ?? "0");

            double settingAppPrecent = (await _settings.Entity.GetAsync(predicate: x => x.FK_BranchID == branchId)).appPrecent;

            SubServices service = await _subServices.Entity.FindByIdAsync(model.id);
            if (service != null)
            {
                service.nameAr = model.name_ar ?? service.nameAr;
                service.nameEn = model.name_en ?? service.nameEn;
                service.duration = model.duration != 0 ? model.duration : service.duration;
                service.price = (model.price != 0 ? model.price : service.price) + settingAppPrecent;
                service.Image = model.image != null ? Helper.Helper.ProcessUploadedFile(_HostingEnvironment, model.image, "SubService") : service.Image;
                service.DescriptionAr = model.descriptionAr ?? service.DescriptionAr;
                service.DescriptionEn = model.descriptionEn ?? service.DescriptionEn;
                _subServices.Entity.Update(service);
                await _subServices.Save();
                return Ok(new
                {
                    key = 1,
                    msg = CreatMessage(model.lang, "تم تعديل الخدمة بنجاح", "service Edited successfully"),
                });
            }

            return Ok(new
            {
                key = 0,
                msg = CreatMessage(model.lang, "الخدمة غير موجوده", "service not found"),
            });
        }

        [HttpPost(ApiRoutes.Provider.DeleteSubService)]
        public async Task<ActionResult> DeleteSubService(DeleteSubServiceDTO model)
        {
            string userId = User.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;
            int branchId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == "branch_id")?.Value ?? "0");

            SubServices service = await _subServices.Entity.FindByIdAsync(model.id);
            if (service != null)
            {
                _subServices.Entity.Delete(service);
                await _subServices.Save();
                return Ok(new
                {
                    key = 1,
                    msg = CreatMessage(model.lang, "تم حذف الخدمة بنجاح", "service deleted successfully"),
                });
            }

            return Ok(new
            {
                key = 0,
                msg = CreatMessage(model.lang, "الخدمة غير موجوده", "service not found"),
            });
        }

        [HttpPost(ApiRoutes.Provider.GetCurrentBoutiqueData)]
        public async Task<ActionResult> GetCurrentBoutiqueData(GetCurrentBoutiqueDataDTO model)
        {
            string userId = User.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;
            int branchId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == "branch_id")?.Value ?? "0");
            double settingAppPrecent = (await _settings.Entity.GetAsync(predicate: x => x.FK_BranchID == branchId)).appPrecent;

            var boutiques = (await _providerAditionalData.Entity.GetAllAsync(
                predicate: x => x.FK_UserID == userId && x.FK_User.isActive == true && x.FK_User.activeCode == true,
                include: source => source.Include(i => i.FK_User).Include(i => i.SalonImages).Include(x => x.Offers)
                                .Include(i => i.SubServices).ThenInclude(i => i.FK_MainService).ThenInclude(i => i.SubServices),
                disableTracking: false
                )).Select(x => new
                {
                    id = x.ID,
                    imgs = x.SalonImages.Select(x => BaseUrlHoste + x.img).ToList(),
                    name = CreatMessage(model.lang, x.nameAr, x.nameEn),
                    description = CreatMessage(model.lang, x.descriptionAr, x.descriptionEn),
                    address = x.address,
                    lat = x.lat,
                    lng = x.lng,
                    rate = x.rate,
                    time_from = x.timeForm.ToString("hh:mm tt"),
                    time_to = x.timeTo.ToString("hh:mm tt"),
                    offers = x.Offers.Select(x => BaseUrlHoste + x.img).ToList(),
                    main_services = x.SubServices.Where(s => s.isActive == true).GroupBy(s => s.FK_MainServiceID).Select(s => s.FirstOrDefault())
                    .Where(s => s.FK_MainService.isActive == true)
                    .Select(s => new
                    {
                        name = CreatMessage(model.lang, s.FK_MainService.nameAr, s.FK_MainService.nameEn),
                        sub_services = s.FK_MainService.SubServices.Where(service => service.FK_ProviderAdditionalDataID == x.ID).Select(service => new
                        {
                            id = service.ID,
                            name_ar = CreatMessage("ar", service.nameAr, service.nameEn),
                            name_en = CreatMessage("en", service.nameAr, service.nameEn),
                            duration = service.duration,
                            price = service.price - settingAppPrecent,
                            img = BaseUrlHoste + service.Image,
                            descriptionAr = service.DescriptionAr,
                            descriptionEn = service.DescriptionEn
                        }).ToList()
                    }).ToList()
                }).FirstOrDefault();


            return Ok(new
            {
                key = 1,
                data = boutiques
            });
        }

        [HttpPost(ApiRoutes.Provider.GetCurrentBoutiqueAppPrecent)]
        public async Task<ActionResult> GetCurrentBoutiqueAppPrecent(GetCurrentBoutiqueAppPrecentDTO model)
        {
            string userId = User.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;
            int branchId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == "branch_id")?.Value ?? "0");
            Settings settingApp = (await _settings.Entity.GetAsync(predicate: x => x.FK_BranchID == branchId));

            var boutiques = (await _providerAditionalData.Entity.GetAllAsync(
                predicate: x => x.FK_UserID == userId && x.FK_User.isActive == true && x.FK_User.activeCode == true,
                include: source => source.Include(i => i.FK_User).Include(i => i.SalonImages).Include(x => x.Offers)
                                .Include(i => i.SubServices).ThenInclude(i => i.FK_MainService).ThenInclude(i => i.SubServices),
                disableTracking: false
                )).Select(x => new
                {
                    wallet = branchId == 9 ? x.FK_User.stableWallet : x.FK_User.wallet,
                    payText = settingApp.payText
                }).FirstOrDefault();


            return Ok(new
            {
                key = 1,
                data = boutiques
            });
        }


        [HttpPost(ApiRoutes.Provider.ListProviderOffers)]
        public async Task<ActionResult> ListProviderOffers(ListProviderOffersDTO model)
        {
            string userId = User.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;

            var offers = (await _offers.Entity.GetAllAsync(
                predicate: x => x.FK_ProviderAdditionalData.FK_UserID == userId
                )).Select(x => new
                {
                    id = x.ID,
                    img = BaseUrlHoste + x.img ?? "",
                }).OrderByDescending(x => x.id).ToList();

            return Ok(new
            {
                key = 1,
                offers = offers
            });
        }

        [HttpPost(ApiRoutes.Provider.AddProviderOffer)]
        public async Task<ActionResult> AddProviderOffer(AddProviderOfferDTO model)
        {
            string userId = User.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;

            ProviderAditionalData user = await _providerAditionalData.Entity.GetAsync(x => x.FK_UserID == userId);

            if (model.img == null)
            {
                return Ok(new
                {
                    key = 0,
                    msg = CreatMessage(model.lang, "لاتوجد صور", "add image to offer")
                });
            }

            Offers offers = new Offers()
            {
                isActive = true,
                FK_ProviderAdditionalDataID = user.ID,
                img = ProcessUploadedFile(_HostingEnvironment, model.img, "OfferImages")
            };

            await _offers.Entity.InsertAsync(offers);
            await _offers.Save();
            return Ok(new
            {
                key = 1,
                msg = CreatMessage(model.lang, "تم الاضافة بنجاح", " added successfully")
            });
        }

        [HttpPost(ApiRoutes.Provider.DeleteProviderOffer)]
        public async Task<ActionResult> DeleteProviderOffer(DeleteProviderOfferDTO model)
        {
            string userId = User.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;

            Offers offer = await _offers.Entity.FindByIdAsync(model.id);
            if (offer != null)
            {
                _offers.Entity.Delete(offer);
                await _offers.Save();

                return Ok(new
                {
                    key = 1,
                    msg = CreatMessage(model.lang, "تم الحذف بنجاح", " deleted successfully")
                });

            }
            return Ok(new
            {
                key = 0,
                msg = CreatMessage(model.lang, " غير موجود", " not found")
            });
        }


        [HttpPost(ApiRoutes.Provider.ListProviderOrders)]
        public async Task<ActionResult> ListProviderOrders(ListProviderOrdersDTO model)
        {

            int branchId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == "branch_id")?.Value ?? "0");
            string userId = User.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;
            int notification_count = _notifications.Entity.GetCustomAll(predicate: x => x.FK_UserID == userId && x.show == false).Count();
            ProviderAditionalData user = await _providerAditionalData.Entity.GetAsync(predicate: x => x.FK_UserID == userId, include: source => source.Include(i => i.FK_User));
            var orders = (await _orders.Entity.GetAllAsync(predicate: order => order.FK_Provider.FK_UserID == userId
            && !order.IsDeleted
            && order.status == model.type
            && order.paid == true
            && (order.Applicationpercentagepaid || order.status == (int)OrderStates.waiting),
            include: source => source.Include(i => i.FK_User).Include(i => i.OrderServices).Include(i => i.FK_Provider)
            )).Select(order => new
            {
                id = order.ID,
                client_id = order.FK_UserID,
                client_name = order.FK_User.fullName,
                client_phone = order.FK_User.PhoneNumber,
                client_img = BaseUrlHoste + order.FK_User.img,
                count_services = order.OrderServices.Count(),
                type_pay_id = order.typePay,
                type_pay = CreatMessage(model.lang, order.typePay == (int)Enums.AllEnums.TypePay.cash ? "كاش" : "اونلاين", order.typePay == (int)Enums.AllEnums.TypePay.cash ? "cash" : "online"),
                price = order.price,
                pdf = order.pdf,
                address = !String.IsNullOrEmpty(order.address) ? order.address : order.FK_Provider.address,
                lat = !String.IsNullOrEmpty(order.lat) ? order.lat : order.FK_Provider.lat,
                lng = !String.IsNullOrEmpty(order.lng) ? order.lng : order.FK_Provider.lng,
                orderDate = order.orderDate.ToString("dd/MM/yyyy"),
                orderTime = order.orderDate.ToString("hh:mm tt"),
                type = !String.IsNullOrEmpty(order.address) ? Helper.Helper.CreatMessage(model.lang, "بالمنزل", "Home") : Helper.Helper.CreatMessage(model.lang, "بالصالون", "Salon"),
                shippingValue = order.shippingPrice,
                refusedReason = order.refusedReason,
                services = order.OrderServices.Select(serv => new
                {
                    id = serv.ID,
                    img = BaseUrlHoste + order.FK_User.img,
                    service_name = CreatMessage(model.lang, serv.SubServicNameAr, serv.SubServicNameEn),
                    main_service_name = CreatMessage(model.lang, serv.mainServiceNameAr, serv.mainServiceNameEn),
                    price = serv.price,
                    date = serv.date.ToString("dd/MM/yyyy"),
                    time = serv.date.ToString("HH:mm tt"),
                    address = String.IsNullOrEmpty(serv.address) ? CreatMessage(model.lang, "بالصالون", "at salon") : serv.address,
                    lat = String.IsNullOrEmpty(serv.address) ? order.FK_Provider.lat : serv.lat,
                    lng = String.IsNullOrEmpty(serv.address) ? order.FK_Provider.lng : serv.lng,
                    employeeName = CreatMessage(model.lang, serv.EmployeeNameAr, serv.EmployeeNameEn)

                }).ToList(),
                atHome = order.OrderServices.Where(x => !String.IsNullOrEmpty(x.address)).Any(),


            }).OrderByDescending(x => x.id).ToList();
            return Ok(new
            {
                key = 1,
                data = orders,
                notification_count
            });





        }

        [HttpPost(ApiRoutes.Provider.GetOrderDetails)]
        public async Task<ActionResult> GetOrderDetails(GetOrderDetailsDTO model)
        {
            string userId = User.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;
            int branchId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == "branch_id")?.Value ?? "0");

            var orderData = await _orders.Entity.GetCustomAll(predicate: order => order.FK_Provider.FK_UserID == userId && order.ID == model.orderId).Select(order => new
            {
                id = order.ID,
                client_id = order.FK_UserID,
                client_name = order.FK_User.fullName,
                client_phone = order.FK_User.PhoneNumber,
                client_img = BaseUrlHoste + order.FK_User.img,
                count_services = order.OrderServices.Count(),
                type_pay_id = order.typePay,
                type_pay = CreatMessage(model.lang, order.typePay == (int)Enums.AllEnums.TypePay.cash ? "كاش" : "اونلاين", order.typePay == (int)Enums.AllEnums.TypePay.cash ? "cash" : "online"),
                price = order.price,
                priceBeforDesc = order.priceBeforeDisc,

                //priceAtHome = order.OrderServices.Where(p => p.priceAtHome > 0 && !String.IsNullOrEmpty(p.address)).Select(p => p.priceAtHome).Sum(),
                //deliveryPrice = order.OrderServices.Where(p => p.priceAtHome > 0 && !String.IsNullOrEmpty(p.address)).Select(p => p.deliveryPrice).Sum(),

                //priceWithDeliveryPrice = order.price + (order.OrderServices.Where(p => p.priceAtHome > 0 && !String.IsNullOrEmpty(p.address)).Select(p => p.deliveryPrice).Sum()),

                applicationPercent = order.Applicationpercentage,
                pdf = order.pdf,
                address = !String.IsNullOrEmpty(order.address) ? order.address : order.FK_Provider.address,
                lat = !String.IsNullOrEmpty(order.lat) ? order.lat : order.FK_Provider.lat,
                lng = !String.IsNullOrEmpty(order.lng) ? order.lng : order.FK_Provider.lng,
                orderDate = order.orderDate.ToString("dd/MM/yyyy"),
                orderTime = order.orderDate.ToString("hh:mm tt"),
                type = !String.IsNullOrEmpty(order.address) ? Helper.Helper.CreatMessage(model.lang, "بالمنزل", "Home") : Helper.Helper.CreatMessage(model.lang, "بالصالون", "Salon"),
                shippingValue = order.shippingPrice,
                status = order.status,
                rate = order.rate,
                comment = order.rateComment,
                refusedReason = order.refusedReason,
                services = order.OrderServices.Select(serv => new
                {
                    id = serv.ID,
                    img = BaseUrlHoste + order.FK_User.img,
                    service_name = CreatMessage(model.lang, serv.SubServicNameAr, serv.SubServicNameEn),
                    main_service_name = CreatMessage(model.lang, serv.mainServiceNameAr, serv.mainServiceNameEn),
                    price = serv.price,
                    date = serv.date.ToString("dd/MM/yyyy"),
                    time = serv.date.ToString("HH:mm tt"),
                    address = String.IsNullOrEmpty(serv.address) ? CreatMessage(model.lang, "بالصالون", "at salon") : serv.address,
                    lat = String.IsNullOrEmpty(serv.address) ? CreatMessage(model.lang, "بالصالون", "at salon") : serv.lat,
                    lng = String.IsNullOrEmpty(serv.address) ? CreatMessage(model.lang, "بالصالون", "at salon") : serv.lng,
                    employeeName = model.lang == "ar" ? serv.EmployeeNameAr : serv.EmployeeNameEn,
                    employeeImg = BaseUrlHoste + serv.EmployeeImg,
                }).ToList()
            }).FirstOrDefaultAsync();


            return Ok(new
            {
                key = 1,
                data = orderData
            });


        }

        [HttpPost(ApiRoutes.Provider.ChangeOrderStatus)]
        public async Task<ActionResult> ChangeOrderStatus(ChangeOrderStatusDTO model)
        {
            string userId = User.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;
            int branchId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == "branch_id")?.Value ?? "0");

            Orders order = await _orders.Entity.GetAsync(predicate: o => o.ID == model.id, include: source => source.Include(i => i.FK_User).Include(i => i.FK_Provider).ThenInclude(i => i.FK_User), disableTracking: false);
            if (order.status != 0 && (model.type == 1 || model.type == 3))
            {
                return Ok(new
                {
                    key = 0,
                    msg = CreatMessage(model.lang, "لايمكن قبول او رفض الطلب", "can't accept or refuse order")
                });
            }
            order.status = model.type;
            if (model.type == (int)OrderStates.refused)
            {
                order.refusedReason = model.reason;
            }

            string msg = order.status switch
            {
                (int)OrderStates.waiting => CreatMessage(model.lang, "الطلب فى حالة الانتظار", "order in waitting"),
                (int)OrderStates.accepted => CreatMessage(model.lang, "تم قبول الطلب", "order accepted"),
                (int)OrderStates.finished => CreatMessage(model.lang, "تم انهاء الطلب", "order finished"),
                (int)OrderStates.refused => CreatMessage(model.lang, "تم رفض الطلب", "order refused"),
                (int)OrderStates.canceled => CreatMessage(model.lang, "تم الغاء الطلب", "order canceled"),
                _ => CreatMessage(model.lang, "خطأ", "error")
            };

            if (order.status == (int)OrderStates.canceled || order.status == (int)OrderStates.refused)
            {
                order.FK_User.userWallet += order.price;
            }



            // to send notification if not msg = "error"
            //if (order.status <= (int)OrderStates.canceled)
            //{

            //        HistoryNotify notifyObj = new HistoryNotify()
            //        {
            //            Text = msg,
            //            Date = Helper.Helper.GetCurrentDate(),
            //            FKUser = providerId,
            //        };
            //        historyNotifies.Add(notifyObj);


            //    _context.HistoryNotify.AddRange(historyNotifies);
            //    await _context.SaveChangesAsync();
            //    dynamic info = "";
            //    var user_devices = (from historyNotify in historyNotifies
            //                        join deviceId in _context.DeviceIds on historyNotify.FKUser equals deviceId.FK_UserID
            //                        select deviceId.deviceID
            //                        ).ToList();

            //    Helper.Helper.SendPushNotification(device_ids: user_devices, msg: msg, type: (int)Enums.AllEnums.FcmType.dashboard, branchId: userBranch, projectName: branchName);
            //}



            try
            {
                if (order.FK_User.FK_BranchID == 3 && order.status == 2)
                {
                    //////////////////////////////////////////////////////// bill /////////////////////////////////////////////////////////////////////////////

                    string pdfname = String.Format("{0}.pdf", Guid.NewGuid().ToString());
                    GlobalSettings globalSettings = new GlobalSettings
                    {
                        ColorMode = ColorMode.Color,
                        Orientation = DinkToPdf.Orientation.Portrait,
                        PaperSize = PaperKind.A4,
                        Margins = new MarginSettings { Top = 10 },
                        DocumentTitle = "PDF Report",
                        Out = Path.Combine(_HostingEnvironment.WebRootPath, "pdf", pdfname)
                    };
                    ObjectSettings objectSettings = new ObjectSettings
                    {
                        PagesCount = true,
                        Page = "https://mashaghil.ip4s.com/Home/CreateBill/" + order.ID,

                    };
                    HtmlToPdfDocument pdf = new HtmlToPdfDocument()
                    {
                        GlobalSettings = globalSettings,
                        Objects = { objectSettings }

                    };
                    byte[] files = _converter.Convert(pdf);


                    string pdfPath = globalSettings.Out; // output path

                    //order.pdf = pdfPath;    

                    order.pdf = BaseUrlHoste + "pdf/" + pdfname;


                }

                if (order.FK_User.FK_BranchID == 9 && order.status == 2)
                {
                    //////////////////////////////////////////////////////// bill /////////////////////////////////////////////////////////////////////////////

                    string pdfname = String.Format("{0}.pdf", Guid.NewGuid().ToString());
                    GlobalSettings globalSettings = new GlobalSettings
                    {
                        ColorMode = ColorMode.Color,
                        Orientation = DinkToPdf.Orientation.Portrait,
                        PaperSize = PaperKind.A4,
                        Margins = new MarginSettings { Top = 10 },
                        DocumentTitle = "PDF Report",
                        Out = Path.Combine(_HostingEnvironment.WebRootPath, "pdf", pdfname)

                    };
                    ObjectSettings objectSettings = new ObjectSettings
                    {
                        PagesCount = true,
                        Page = "https://mashaghil.ip4s.com/Home/CreateBillForLady/" + order.ID,

                    };
                    HtmlToPdfDocument pdf = new HtmlToPdfDocument()
                    {
                        GlobalSettings = globalSettings,
                        Objects = { objectSettings }

                    };
                    byte[] files = _converter.Convert(pdf);


                    string pdfPath = globalSettings.Out; // output path

                    //order.pdf = pdfPath;    

                    order.pdf = BaseUrlHoste + "pdf/" + pdfname;
                    Messages messages = new Messages()
                    {
                        DateSend = Helper.Helper.GetCurrentDate(),
                        FK_OrderId = order.ID,
                        ReceiverId = order.FK_UserID,
                        SenderId = order.FK_Provider.FK_UserID,
                        Text = BaseUrlHoste + "pdf/" + pdfname,
                        TypeMessage = (int)Enums.AllEnums.FileTypeChat.file
                    };
                    await _messages.Entity.InsertAsync(messages);

                }

            }
            catch (Exception)
            {

            }

            _orders.Entity.Update(order);
            await _orders.Save();


            string[] fcm_client_msg = order.status switch
            {
                1 => new string[] { $"تم قبول طلبك رقم {order.ID}", $"your order number {order.ID} accepted" },
                2 => new string[] { $"تم انهاء الطلب رقم {order.ID}", $"order number {order.ID} finished" },
                3 => new string[] { $"تم رفض الطلب رقم {order.ID}", $"order number {order.ID} refused" },
                4 => new string[] { $"تم الغاء الطلب رقم {order.ID}", $"order number {order.ID} canceled" },
                _ => new string[] { $"خطأ", $"error" }
            };


            await _notifications.Entity.InsertRangeAsync(new List<Notifications>() {
                                                            new Notifications(){
                                                                FK_OrderID = order.ID,
                                                                FK_UserID = order.FK_UserID,
                                                                msgAr=fcm_client_msg[0],
                                                                msgEn=fcm_client_msg[1],
                                                                show = false,
                                                                date = GetCurrentDate()
                                                            }
            });
            await _notifications.Save();
            var userDataAndIds = await _users.Entity.GetCustomAll(predicate: x => x.Id == order.FK_UserID /*|| x.Id == order.FK_Provider.FK_UserID*/).Select(x => new
            {
                lang = x.lang,
                ids = x.DeviceIds.Select(y => y.deviceID).ToList(),
                branchId = x.FK_BranchID,
            }).ToListAsync();
            foreach (var item in userDataAndIds)
            {
                string projectName = await _branches.Entity.GetCustomAll(x => x.ID == item.branchId).Select(x => x.nameAr + " | " + x.nameEn).FirstOrDefaultAsync();
                SendPushNotification(device_ids: item.ids, msg: CreatMessage(item.lang, fcm_client_msg[0], fcm_client_msg[1]), order_id: order.ID, branchId: branchId, projectName: projectName);

            }

            return Ok(new
            {
                key = 1,
                msg = msg
            });
        }

        [HttpPost(ApiRoutes.Provider.GetProviderData)]
        public async Task<ActionResult> GetProviderData(GetProviderDataDTO model)
        {
            string userId = User.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;

            ApplicationDbUser user = await _users.Entity.GetAsync(predicate: x => x.Id == userId,
                include: source => source.Include(i => i.ProviderAditionalData).ThenInclude(i => i.FK_District).Include(i => i.ProviderAditionalData).ThenInclude(i => i.FK_Category).Include(i => i.ProviderAditionalData).ThenInclude(i => i.SalonImages));

            var data = new
            {
                img = BaseUrlHoste + user.img,
                user_name = user.fullName,
                boutique_name_ar = user.ProviderAditionalData.nameAr,
                boutique_name_en = user.ProviderAditionalData.nameEn,
                phone = user.PhoneNumber,
                email = user.Email,
                commercial_register_number = user.ProviderAditionalData.commercialRegister,
                city = user.ProviderAditionalData.FK_District.FK_CityID,
                district = user.ProviderAditionalData.FK_DistrictID,
                address = user.ProviderAditionalData.address,
                lat = user.ProviderAditionalData.lat,
                lng = user.ProviderAditionalData.lng,
                time_from = user.ProviderAditionalData.timeForm.ToString("HH:mm"),
                time_to = user.ProviderAditionalData.timeTo.ToString("HH:mm"),
                days = GetDays(user.ProviderAditionalData.dayWorks, model.lang),
                salonType = user.ProviderAditionalData.salonType,
                description_ar = user.ProviderAditionalData.descriptionAr,
                description_en = user.ProviderAditionalData.descriptionEn,
                categoryId = user.ProviderAditionalData.FK_CategoryID,
                instagramProfile = user.ProviderAditionalData.socialMediaProfile,
                iDPhoto = BaseUrlHoste + user.iDPhoto,
                certificatePhoto = BaseUrlHoste + user.certificatePhoto,

                identityImage = BaseUrlHoste + user.ProviderAditionalData.IdentityImage,
                commercialRegisterImage = BaseUrlHoste + user.ProviderAditionalData.CommercialRegisterImage,
                healthCardImage = BaseUrlHoste + user.ProviderAditionalData.HealthCardImage,
                ibanNumber = user.ProviderAditionalData.IbanNumber,
                ibanImage = BaseUrlHoste + user.ProviderAditionalData.IbanImage,

                idNumber = user.ProviderAditionalData.IdNumber,
                bankName = user.ProviderAditionalData.BankName,

                categoryName = CreatMessage(model.lang, user.ProviderAditionalData.FK_Category.nameAr, user.ProviderAditionalData.FK_Category.nameEn),
                imgs = user.ProviderAditionalData.SalonImages.Select(x => new
                {
                    id = x.ID,
                    img = Helper.Helper.BaseUrlHoste + x.img
                }).ToList()
            };

            return Ok(new
            {
                key = 1,
                data = data
            });
        }


        [HttpPost(ApiRoutes.Provider.UpdateProviderData)]
        public async Task<ActionResult> UpdateProviderData(UpdateProviderDataDTO model)
        {
            string userId = User.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;
            ApplicationDbUser user = await _users.Entity.GetAsync(predicate: x => x.Id == userId,
    include: source => source.Include(i => i.ProviderAditionalData).ThenInclude(i => i.FK_District).Include(i => i.ProviderAditionalData).ThenInclude(i => i.FK_Category).Include(i => i.ProviderAditionalData).ThenInclude(i => i.SalonImages), disableTracking: false);


            #region validation
            //name

            if (model.user_name == null)
            {
                return Ok(new
                {
                    key = 0,
                    msg = CreatMessage(model.lang, "من فضلك ادخل الاسم", "Please enter your Name")
                });
            }

            //var UserName = await _users.Entity.GetAsync(x => x.fullName == model.user_name);
            //if (UserName != null)
            //{
            //    return Ok(new
            //    {
            //        key = 0,
            //        msg = CreatMessage(model.lang, "عذرا هذا الاسم موجود بالفعل", "Sorry this name is already present")
            //    });
            //}
            if (model.phone == null)
            {
                return Ok(new
                {
                    key = 0,
                    msg = CreatMessage(model.lang, "من فضلك ادخل رقم الجوال", "Please enter your phone number")
                });
            }

            ApplicationDbUser phone = await _users.Entity.GetAsync(x => x.PhoneNumber == model.phone && x.Id != userId && x.FK_BranchID == user.FK_BranchID && x.typeUser == (int)Enums.AllEnums.TypeUser.provider);
            if (phone != null)
            {
                return Ok(new
                {
                    key = 0,
                    msg = CreatMessage(model.lang, "عذرا هذا الجوال موجود بالفعل", "Sorry this mobile is already present")
                });
            }

            if (model.email != null)
            {
                ApplicationDbUser email = await _users.Entity.GetAsync(x => x.Email == model.email && x.Id != userId && x.FK_BranchID == user.FK_BranchID);
                if (email != null)
                {
                    return Ok(new
                    {
                        key = 0,
                        msg = CreatMessage(model.lang, "عذرا هذا الايميل موجود بالفعل", "Sorry this email is already present")
                    });
                }

            }



            #endregion
            //var user = await _users.Entity.GetAsync(predicate: x => x.Id == userId,
            //    include: source => source.Include(i => i.ProviderAditionalData).ThenInclude(i=>i.FK_District), disableTracking: false);
            if (model.img != null)
            {
                //var oldImages = user.ProviderAditionalData.SalonImages.ToList();
                //_salonImages.Entity.DeleteRange(oldImages);
                string profileImage = Helper.Helper.ProcessUploadedFile(_HostingEnvironment, model.img, "Users");
                //await _salonImages.Entity.InsertAsync(new SalonImages() { img = profileImage , FK_ProviderAdditionalDataID = user.ProviderAditionalData.ID });
                user.img = profileImage;
            }
            List<int> deletedImgs = new List<int>();
            if (!String.IsNullOrEmpty(model.deletedImgs))
            {
                deletedImgs = model.deletedImgs.Split(',').Where(x => !String.IsNullOrEmpty(x)).Select(x => Convert.ToInt32(x)).ToList();
            }
            if (deletedImgs.Count != 0)
            {
                List<SalonImages> usersimgss = user.ProviderAditionalData.SalonImages.Where(pi => deletedImgs.Contains(pi.ID)).ToList();
                _salonImages.Entity.DeleteRange(user.ProviderAditionalData.SalonImages.Where(pi => deletedImgs.Contains(pi.ID)).ToList());
            }

            if (model.imgs.Count != 0)
            {
                foreach (IFormFile item in model.imgs)
                {
                    string profileImage = Helper.Helper.ProcessUploadedFile(_HostingEnvironment, item, "Users");
                    await _salonImages.Entity.InsertAsync(new SalonImages() { img = profileImage, FK_ProviderAdditionalDataID = user.ProviderAditionalData.ID });
                }
            }



            user.iDPhoto = model.iDPhoto == null ? user.iDPhoto : Helper.Helper.ProcessUploadedFile(_HostingEnvironment, model.iDPhoto, "Users");
            user.certificatePhoto = model.certificatePhoto == null ? user.certificatePhoto : Helper.Helper.ProcessUploadedFile(_HostingEnvironment, model.certificatePhoto, "Users");
            user.ibanNumber = model.ibanNumber == null ? " " : model.ibanNumber;
            user.ProviderAditionalData.IbanNumber = model.ibanNumber == null ? " " : model.ibanNumber;
            user.fullName = model.user_name;
            user.ProviderAditionalData.nameAr = model.boutique_name_ar;
            user.ProviderAditionalData.nameEn = model.boutique_name_en;
            user.PhoneNumber = model.phone;
            user.UserName = user.FK_BranchID.ToString() + user.typeUser.ToString() + model.phone;
            user.Email = model.email;
            user.ProviderAditionalData.commercialRegister = model.commercial_register_number;
            user.ProviderAditionalData.FK_DistrictID = model.district;
            user.ProviderAditionalData.address = model.address;
            user.ProviderAditionalData.lat = model.lat;
            user.ProviderAditionalData.lng = model.lng;
            user.ProviderAditionalData.timeForm = model.time_from;
            user.ProviderAditionalData.timeTo = model.time_to;
            user.ProviderAditionalData.descriptionAr = model.description_ar;
            user.ProviderAditionalData.descriptionEn = model.description_en;
            user.ProviderAditionalData.dayWorks = model.days != null ? model.days : user.ProviderAditionalData.dayWorks;
            user.ProviderAditionalData.salonType = model.salonType != 0 ? model.salonType : user.ProviderAditionalData.salonType;
            user.ProviderAditionalData.FK_CategoryID = model.category != 0 ? model.category : user.ProviderAditionalData.FK_CategoryID;
            user.ProviderAditionalData.socialMediaProfile = model.instagramProfile != null ? model.instagramProfile : user.ProviderAditionalData.socialMediaProfile;

            user.ProviderAditionalData.IdentityImage = model.identityImage != null ? ProcessUploadedFile(_HostingEnvironment, model.identityImage, "Users") : user.ProviderAditionalData.IdentityImage;
            user.ProviderAditionalData.CommercialRegisterImage = model.commercialRegisterImage != null ? ProcessUploadedFile(_HostingEnvironment, model.commercialRegisterImage, "Users") : user.ProviderAditionalData.CommercialRegisterImage;
            user.ProviderAditionalData.HealthCardImage = model.healthCardImage != null ? ProcessUploadedFile(_HostingEnvironment, model.healthCardImage, "Users") : user.ProviderAditionalData.HealthCardImage;
            user.ProviderAditionalData.IbanNumber = model.ibanNumber != null ? model.ibanNumber : user.ProviderAditionalData.IbanNumber;
            user.ProviderAditionalData.IbanImage = model.ibanImage != null ? ProcessUploadedFile(_HostingEnvironment, model.ibanImage, "Users") : user.ProviderAditionalData.IbanImage;

            user.ProviderAditionalData.IdNumber = model.idNumber;
            user.ProviderAditionalData.BankName = model.bankName;

            _users.Entity.Update(user);
            await _users.Save();

            return Ok(new
            {
                key = 1,
                msg = CreatMessage(model.lang, "تم تعديل البيانات بنجاح", "data edited successfully"),
                data = GetUserInfo(user)
            });
        }

        [HttpPost(ApiRoutes.Provider.CountProviderOrders)]
        public async Task<ActionResult> CountProviderOrders(CountProviderOrdersDTO model)
        {

            string userId = User.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;
            int branchId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == "branch_id")?.Value ?? "0");

            var orders = (await _orders.Entity.GetAllAsync(predicate: order => order.FK_Provider.FK_UserID == userId && order.paid == true
                                                  )).Select(order => new
                                                  {
                                                      id = order.ID,
                                                      status = order.status,
                                                      price = order.status == (int)OrderStates.finished ? order.price : 0
                                                  }).ToList();

            ProviderAditionalData providerex = await _providerAditionalData.Entity.GetAsync(predicate: p => p.FK_UserID == userId, include: source => source.Include(i => i.FK_User));
            double pay = 0;

            //if (orders.Select(o=>o.price).Sum() > providerex.paied && providerex.lastPayDate.AddMonths(1) <= GetCurrentDate() && providerex.FK_User.FK_BranchID == 3)
            //{
            //    pay = orders.Select(o => o.price).Sum();
            //    if ((pay - providerex.paied) == 0 && providerex.lastPayDate.AddMonths(1) <= GetCurrentDate())
            //    {
            //        providerex.lastPayDate = GetCurrentDate();
            //        _providerAditionalData.Entity.Update(providerex);
            //        await _providerAditionalData.Save();
            //    }
            //}

            var notifications = (await _notifications.Entity.GetAllAsync(predicate: x => x.FK_UserID == userId && x.FK_Order.status == (int)OrderStates.waiting, include: source => source.Include(i => i.FK_Order))).Select(x => new
            {
                id = x.ID,
                msg = CreatMessage(model.lang, x.msgAr, x.msgEn),
                order_id = x.FK_OrderID,
                date = x.date.ToString("dd/MM/yyyy"),
                read = x.show,
                go_to_rate = x.FK_Order.status == (int)OrderStates.finished
            }).OrderByDescending(x => x.id).ToList();
            int notificationCount = await _notifications.Entity.GetCustomAll(x => x.FK_UserID == userId && !x.show).CountAsync();

            // get provider data


            ApplicationDbUser user = await _users.Entity.GetAsync(predicate: x => x.Id == userId,
                include: source => source.Include(i => i.ProviderAditionalData).ThenInclude(i => i.FK_District).Include(i => i.ProviderAditionalData).ThenInclude(i => i.FK_Category).Include(i => i.ProviderAditionalData).ThenInclude(i => i.SalonImages));



            UserInfoViewModel dataUser = new UserInfoViewModel
            {
                id = user.Id,
                user_name = user.fullName,
                img = BaseUrlHoste + user.img,
                phone = user.PhoneNumber,
                //email = user.Email,
                lang = user.lang,
                active = user.isActive,
                close_notify = user.closeNotification,
                type = user.typeUser,


                boutique_name_ar = user.ProviderAditionalData.nameAr,
                boutique_name_en = user.ProviderAditionalData.nameEn,
                email = user.Email,
                commercial_register_number = user.ProviderAditionalData.commercialRegister,
                city = user.ProviderAditionalData.FK_District.FK_CityID,
                district = user.ProviderAditionalData.FK_DistrictID,
                address = user.ProviderAditionalData.address,
                lat = user.ProviderAditionalData.lat,
                lng = user.ProviderAditionalData.lng,
                time_from = user.ProviderAditionalData.timeForm.ToString("HH:mm"),
                time_to = user.ProviderAditionalData.timeTo.ToString("HH:mm"),
                days = GetDays(user.ProviderAditionalData.dayWorks, model.lang),
                salonType = user.ProviderAditionalData.salonType,
                description_ar = user.ProviderAditionalData.descriptionAr,
                description_en = user.ProviderAditionalData.descriptionEn,
                iDPhoto = BaseUrlHoste + user.iDPhoto,
                certificatePhoto = BaseUrlHoste + user.certificatePhoto,
                ibanNumber = user.ibanNumber,
                categoryId = user.ProviderAditionalData.FK_CategoryID,
                categoryName = CreatMessage(model.lang, user.ProviderAditionalData.FK_Category.nameAr, user.ProviderAditionalData.FK_Category.nameEn),
                rate = user.ProviderAditionalData.rate

            };


            return Ok(new
            {
                key = 1,
                dataUser = dataUser,
                waiting = orders.Where(x => x.status == (int)Enums.AllEnums.OrderStates.waiting).Count(),
                accepted = orders.Where(x => x.status == (int)Enums.AllEnums.OrderStates.accepted).Count(),
                finished = orders.Where(x => x.status == (int)Enums.AllEnums.OrderStates.finished).Count(),
                refused = orders.Where(x => x.status == (int)Enums.AllEnums.OrderStates.refused).Count(),
                canceled = orders.Where(x => x.status == (int)Enums.AllEnums.OrderStates.canceled).Count(),
                pay = pay,
                notifications = notifications,
                notificationCount
            });


        }


        [HttpPost(ApiRoutes.Provider.DeleteProviderNotification)]
        public async Task<ActionResult> DeleteProviderNotification(DeleteProviderNotifyDTO model)
        {
            Notifications notify = await _notifications.Entity.FindByIdAsync(model.id);
            if (notify != null)
            {
                _notifications.Entity.Delete(notify);
                await _offers.Save();

                return Ok(new
                {
                    key = 1,
                    msg = CreatMessage(model.lang, "تم الحذف بنجاح", " deleted successfully")
                });

            }
            return Ok(new
            {
                key = 0,
                msg = CreatMessage(model.lang, " غير موجود", " not found")
            });

        }

        [HttpPost(ApiRoutes.Provider.ProviderWallet)]
        public async Task<ActionResult> ProviderWallet(string lang = "ar")
        {
            string userId = User.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;

            int branchId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == "branch_id")?.Value ?? "0");
            Settings settingAppPrecent = (await _settings.Entity.GetAsync(predicate: x => x.FK_BranchID == branchId));

            //var provider = await _users.Entity.GetAsync(predicate: x => x.Id == userId);
            //var wallet = provider.stableWallet;

            List<ListFinishedOrderProviderWalletDto> ListFinishedOrder = await _orders.Entity.GetCustomAll(predicate: order => order.FK_Provider.FK_UserID == userId && order.status == 2 && order.payOut == false)
                .Select(order => new ListFinishedOrderProviderWalletDto
                {
                    id = order.ID,

                    // after discount
                    price = ((order.priceBeforeDisc - ((order.priceBeforeDisc * settingAppPrecent.appPrecent) / 100))
                            +
                            (order.OrderServices.Where(p => !String.IsNullOrEmpty(p.address)).Select(p => p.deliveryPrice).Sum())
                            )
                    //OrderServices = order.OrderServices.ToList()

                }).OrderByDescending(x => x.id).AsNoTracking().ToListAsync();


            //ListFinishedOrder.ForEach(l =>
            //{
            //    l.price = (l.price + );
            //});

            double wallet = ListFinishedOrder.Sum(x => x.price);

            return Ok(new
            {
                key = 1,
                wallet,
                ListFinishedOrder
            });

        }



        [HttpPost(ApiRoutes.Provider.Reports)]
        public async Task<ActionResult> Reports(string lang = "ar")
        {
            string userId = User.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;


            var ListFOrders = await _orders.Entity.GetCustomAll(predicate: order => order.FK_Provider.FK_UserID == userId)
                .Select(order => new
                {
                    id = order.ID,
                    price = order.price,
                    date = order.date,
                    status = order.status,
                }).AsNoTracking().ToListAsync();

            double thisDayTotal = ListFOrders.Where(x => x.date.Day == GetCurrentDate().Day).Sum(x => x.price);
            int thisDayCount = ListFOrders.Where(x => x.date.Day == GetCurrentDate().Day).Count();
            double thisDayAvailableBalance = ListFOrders.Where(x => x.date.Day == GetCurrentDate().Day && x.status == 2).Sum(x => x.price);

            double thisWeekTotal = ListFOrders.Where(x => x.date.Day < GetCurrentDate().Day && x.date.Day >= GetCurrentDate().AddDays(-7).Day).Sum(x => x.price);
            int thisWeekCount = ListFOrders.Where(x => x.date.Day < GetCurrentDate().Day && x.date.Day >= GetCurrentDate().AddDays(-7).Day).Count();
            double thisWeekAvailableBalance = ListFOrders.Where(x => x.date.Day < GetCurrentDate().Day && x.date.Day >= GetCurrentDate().AddDays(-7).Day).Sum(x => x.price);


            return Ok(new
            {
                key = 1,
                thisDayTotal,
                thisDayCount,
                thisDayAvailableBalance,
                thisWeekTotal,
                thisWeekCount,
                thisWeekAvailableBalance,
            });

        }


        [HttpPost(ApiRoutes.Provider.SettlementRequest)]
        public async Task<ActionResult> SettlementRequest(double wallet, string lang = "ar")
        {
            string userId = User.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;
            ApplicationDbUser provider = await _users.Entity.GetAsync(predicate: x => x.Id == userId);
            int branchId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == "branch_id")?.Value ?? "0");


            //else if (branchId != (int)BranchName.Lady)
            //{
            provider.userWallet = provider.userWallet + wallet;

            List<Orders> ListFinishedOrder = (await _orders.Entity.GetAllAsync(predicate: order => order.FK_Provider.FK_UserID == userId && order.status == 2 && order.payOut == false)).ToList();
            ListFinishedOrder.ForEach(x => x.payOut = true);

            _users.Entity.Update(provider);
            _orders.Entity.UpdateRange(ListFinishedOrder);
            await _orders.Save();

            FinancialAccount settlementRequest = await _financialAccount.Entity.GetAsync(predicate: x => x.FkProviderId == userId && !x.IsPaid);

            if (settlementRequest == null)
            {
                FinancialAccount financialAccount = new FinancialAccount()
                {
                    FkProviderId = userId,
                    PayOutPrice = provider.userWallet,
                    Date = GetCurrentDate(),
                    IsPaid = false
                };
                await _financialAccount.Entity.InsertAsync(financialAccount);
            }
            else
            {
                settlementRequest.PayOutPrice = provider.userWallet;
                settlementRequest.Date = GetCurrentDate();
                _financialAccount.Entity.Update(settlementRequest);

            }

            await _financialAccount.Save();

            return Ok(new
            {
                key = 1,
                msg = CreatMessage(lang, "تم  ارسال طلب التسويه بنجاح", "SettlementRequest sent successfully"),
            });
            //}
            //else
            //{
            //    //Wallet = full amount 20% app ratio + 80% provider
            //    provider.userWallet = provider.userWallet + (.8 * wallet);

            //    var ListFinishedOrder = (await _orders.Entity.GetAllAsync(predicate: order => order.FK_Provider.FK_UserID == userId && order.status == 2 
            //    && order.payOut == false)).ToList();
            //    ListFinishedOrder.ForEach(x => x.payOut = true);

            //    _users.Entity.Update(provider);
            //    _orders.Entity.UpdateRange(ListFinishedOrder);
            //    await _orders.Save();

            //    var settlementRequest = await _financialAccount.Entity.GetAsync(predicate: x => x.FkProviderId == userId && !x.IsPaid);

            //    if (settlementRequest == null)
            //    {
            //        FinancialAccount financialAccount = new FinancialAccount()
            //        {
            //            FkProviderId = userId,
            //            PayOutPrice = provider.userWallet,
            //            Date = GetCurrentDate(),
            //            IsPaid = false
            //        };
            //        await _financialAccount.Entity.InsertAsync(financialAccount);
            //    }
            //    else
            //    {
            //        settlementRequest.PayOutPrice = provider.userWallet;
            //        settlementRequest.Date = GetCurrentDate();
            //        _financialAccount.Entity.Update(settlementRequest);

            //    }

            //    await _financialAccount.Save();

            //    return Ok(new
            //    {
            //        key = 1,
            //        msg = CreatMessage(lang, "تم  ارسال طلب التسويه بنجاح", "SettlementRequest sent successfully"),
            //    });
            //}

        }

        [HttpPost(ApiRoutes.Provider.ToggleAvailability)]
        public async Task<ActionResult> ToggleAvailability(string lang = "ar")
        {
            string userId = User.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;
            ApplicationDbUser provider = await _users.Entity.GetAsync(predicate: x => x.Id == userId, disableTracking: false);
            provider.IsAvailable = !provider.IsAvailable;
            await _users.Save();
            return Json(new
            {
                key = 1,
                status = provider.IsAvailable,
                msg = CreatMessage(lang, "تم التغيير بنجاح", "Changed successfully"),
            });
        }
        //[HttpPost(ApiRoutes.Provider.ListSubServices)]
        //public async Task<ActionResult> ListSubSe rvices(string lang = "ar")
        //{
        //    string userId = User.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;

        //    var services = await _subServices.Entity.GetCustomAll(s => s.FK_ProviderAdditionalData.FK_UserID == userId).Select(s => new
        //    {
        //        id = s.ID,
        //        name = CreatMessage(lang, s.nameAr, s.nameEn),
        //    }).ToListAsync();

        //    //var employees = await _employees.Entity.GetCustomAll(predicate: e => e.FK_ProviderAdditionalData.FK_UserID == userId).Select(e => new
        //    //{
        //    //    id = e.ID,
        //    //    name = Helper.Helper.CreatMessage(lang, e.NameAr, e.NameEn),
        //    //}).ToListAsync();

        //    return Json(new
        //    {
        //        key = 1,
        //        services = services
        //    });
        //}

        [HttpPost(ApiRoutes.Provider.AddEmployeeToSalon)]
        public async Task<ActionResult> AddEmployeeToSalon(string nameAr, string nameEn, IFormFile img, int serviceId, string lang = "ar")
        {
            string userId = User.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;
            ApplicationDbUser provider = await _users.Entity.GetAsync(predicate: x => x.Id == userId, source => source.Include(i => i.ProviderAditionalData), disableTracking: false);
            Employees employee = new Employees()
            {
                NameAr = nameAr,
                NameEn = nameEn,
                Img = Helper.Helper.ProcessUploadedFile(_HostingEnvironment, img, "Users"),
                FK_SubServiceID = serviceId,
                FK_ProviderAdditionalDataID = provider.ProviderAditionalData.ID
            };
            await _employees.Entity.InsertAsync(employee);
            await _employees.Save();

            return Json(new
            {
                key = 1,
                msg = CreatMessage(lang, "تم اضافة الموظف بنجاح", "employee added successfully"),
            });
        }

        [HttpPost(ApiRoutes.Provider.ListSalonEmployees)]
        public async Task<ActionResult> ListSalonEmployees(string lang = "ar")
        {
            string userId = User.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;

            var employees = await _employees.Entity.GetCustomAll(predicate: e => e.FK_ProviderAdditionalData.FK_UserID == userId).Select(e => new
            {
                id = e.ID,
                name = Helper.Helper.CreatMessage(lang, e.NameAr, e.NameEn),
                img = Helper.Helper.BaseUrlHoste + e.Img,
                serviceId = e.FK_SubServiceID,
                seviceNAme = CreatMessage(lang, e.NameAr, e.NameEn)
            }).ToListAsync();

            var services = await _subServices.Entity.GetCustomAll(s => s.FK_ProviderAdditionalData.FK_UserID == userId).Select(s => new
            {
                id = s.ID,
                name = CreatMessage(lang, s.nameAr, s.nameEn),
            }).ToListAsync();


            return Json(new
            {
                key = 1,
                employees = employees,
                services = services
            });
        }

        [HttpPost(ApiRoutes.Provider.DeleteSalonEmployee)]
        public async Task<ActionResult> DeleteSalonEmployee(int employeeId, string lang = "ar")
        {
            string userId = User.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;
            Employees employee = await _employees.Entity.GetAsync(predicate: x => x.ID == employeeId, disableTracking: false);

            _employees.Entity.Delete(employee);
            await _employees.Save();

            return Json(new
            {
                key = 1,
                msg = CreatMessage(lang, "تم حذف الموظف بنجاح", "employee deleted successfully"),
            });
        }


    }


    public class ListFinishedOrderProviderWalletDto
    {
        public int id { get; set; }
        public double price { get; set; }
        //public List<OrderServices> OrderServices { get; set; }
    }
}
using Core.Interfaces;
using Core.TableDb;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.VisualStudio.Web.CodeGeneration;
using Salony.Models.ApiDTO.Client;
using Salony.PathUrl;
using Salony.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Transactions;
using static Salony.Enums.AllEnums;
using static Salony.Helper.Helper;

namespace Salony.Controllers.API
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiExplorerSettings(GroupName = "MobileApi")]
    public class ClientController : Controller
    {
        private readonly IWebHostEnvironment _HostingEnvironment;
        private readonly IUnitOfWork<Categories> _categories;
        private readonly IUnitOfWork<Sliders> _sliders;
        private readonly IUnitOfWork<ProviderAditionalData> _providerAditionalData;
        private readonly IUnitOfWork<Carts> _carts;
        private readonly IUnitOfWork<Copons> _copons;
        private readonly IUnitOfWork<Orders> _orders;
        private readonly IUnitOfWork<Notifications> _notifications;
        private readonly IUnitOfWork<SubServices> _subServices;
        private readonly IUnitOfWork<ApplicationDbUser> _users;
        private readonly IUnitOfWork<Branches> _branches;
        private readonly IUnitOfWork<Settings> _settings;
        private readonly IUnitOfWork<Cities> _cities;
        private readonly IUnitOfWork<BankAccounts> _bankAccounts;
        private readonly IUnitOfWork<MainServices> _mainServices;
        private readonly IUnitOfWork<Employees> _employees;
        private readonly IUnitOfWork<ServiceDelivery> _serviceDelivery;


        public ClientController(IWebHostEnvironment HostingEnvironment, IUnitOfWork<Categories> categories, IUnitOfWork<Sliders> sliders, IUnitOfWork<ProviderAditionalData> providerAditionalData,
            IUnitOfWork<Carts> carts, IUnitOfWork<Copons> copons, IUnitOfWork<Orders> orders, IUnitOfWork<Notifications> notifications, IUnitOfWork<SubServices> subServices,
            IUnitOfWork<ApplicationDbUser> users, IUnitOfWork<Branches> branches, IUnitOfWork<Settings> settings, IUnitOfWork<Cities> cities, IUnitOfWork<BankAccounts> bankAccounts,
            IUnitOfWork<MainServices> mainServices, IUnitOfWork<Employees> employees, IUnitOfWork<ServiceDelivery> serviceDelivery)
        {
            this._HostingEnvironment = HostingEnvironment;
            this._categories = categories;
            this._sliders = sliders;
            this._providerAditionalData = providerAditionalData;
            this._carts = carts;
            this._copons = copons;
            this._orders = orders;
            this._notifications = notifications;
            this._subServices = subServices;
            this._users = users;
            this._branches = branches;
            this._settings = settings;
            this._cities = cities;
            this._bankAccounts = bankAccounts;
            this._mainServices = mainServices;
            this._employees = employees;
            _serviceDelivery = serviceDelivery;
        }

        [AllowAnonymous]
        [HttpPost(ApiRoutes.Client.ListCategories)]
        public async Task<ActionResult> ListCategories(ListCategoriesDTO model)
        {
            bool active = false;
            string userId = User.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;
            var user = await _users.Entity.FindByIdAsync(userId);
            if (user != null)
            {
                active = user.isActive;
            }

            var categories = await _categories.Entity.GetCustomAll(
                predicate: x => x.isActive == true && x.FK_BranchID == model.branch_id
                ).Select(x => new ListCategoriesViewModel
                {
                    id = x.ID,
                    name = CreatMessage(model.lang, x.nameAr, x.nameEn),
                    img = BaseUrlHoste + x.img,
                    boutiques = x.ProviderAditionalData.Where(b => b.FK_User.isActive).Select(b => new ListCategoriesBoutiquesViewModel
                    {
                        id = b.ID,
                        img = b.SalonImages.Select(i => Helper.Helper.BaseUrlHoste + i.img).FirstOrDefault() ?? "",
                        name = Helper.Helper.CreatMessage(model.lang, b.nameAr, b.nameEn),
                        description = Helper.Helper.CreatMessage(model.lang, b.descriptionAr, b.descriptionEn),
                        address = b.address,
                        rate = b.rate,
                        dayWorks = b.dayWorks,
                        timeFrom = b.timeForm,
                        timeTo = b.timeTo,
                        opened = false
                    }).OrderByDescending(x => x.rate).Take(10).ToList()
                }).ToListAsync();
            //if (model.branch_id == 10)
            //{
            try
            {
                foreach (var item in categories)
                {
                    foreach (var boutique in item.boutiques)
                    {
                        boutique.opened = ((boutique.dayWorks.Contains(((int)Helper.Helper.GetCurrentDate().DayOfWeek).ToString())) &&
                !(boutique.timeFrom >= boutique.timeFrom.Date.AddHours(Helper.Helper.GetCurrentDate().Hour) || boutique.timeTo <= boutique.timeTo.Date.AddHours(Helper.Helper.GetCurrentDate().Hour))
                );
                    }
                }

            }
            catch (Exception)
            {

            }

            //}
            var slider = (await _sliders.Entity.GetAllAsync(
                predicate: x => x.isActive == true && x.FK_BranchID == model.branch_id && x.type == (int)SliderType.home
                )).Select(x => new
                {
                    id = x.ID,
                    img = BaseUrlHoste + x.img,
                    type = x.type,
                    providerId = x.ProviderId
                }).ToList();

            var cart_count = _carts.Entity.GetCustomAll(predicate: x => x.FK_UserID == userId).Count();
            var notification_count = _notifications.Entity.GetCustomAll(predicate: x => x.FK_UserID == userId && x.show == false).Count();
            var min_price = _subServices.Entity.GetCustomAll(x => x.FK_ProviderAdditionalData.FK_User.FK_BranchID == model.branch_id).Select(x => x.price).Min();
            var max_price = _subServices.Entity.GetCustomAll(x => x.FK_ProviderAdditionalData.FK_User.FK_BranchID == model.branch_id).Select(x => x.price).Max();

            return Ok(new
            {
                key = 1,
                categories = categories,
                slider = slider,
                cart_count,
                notification_count,
                min_price,
                max_price,
                active
            });
        }


        [AllowAnonymous]
        [HttpPost(ApiRoutes.Client.ListBoutiques)]
        public async Task<ActionResult> ListBoutiques(ListBoutiquesDTO model)
        {
            string userId = User.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;

            var boutiques = (await _providerAditionalData.Entity.GetAllAsync(
                predicate: x => x.FK_CategoryID == model.category_id && x.FK_User.IsAvailable && x.FK_User.isActive == true && x.FK_User.activeCode == true && x.FK_User.FK_BranchID == model.branch_id,
                include: source => source.Include(i => i.SalonImages).Include(i => i.FK_User)
                )).Select(x => new
                {
                    id = x.ID,
                    img = BaseUrlHoste + x.SalonImages.Select(x => x.img).FirstOrDefault() ?? "",
                    providerName = x.FK_User.fullName,
                    name = CreatMessage(model.lang, x.nameAr, x.nameEn),
                    address = x.address,
                    rate = x.rate,
                    url = $"{BaseUrlHoste}show/share/{x.ID}",
                    //distance = model.branch_id == 5 ? GetDistance(model.lat, model.lng, x.lat, x.lng) : 0,
                    distance = GetDistance(model.lat, model.lng, x.lat, x.lng),
                }).OrderBy(d => d.distance).ToList();

            var slider = (await _sliders.Entity.GetAllAsync(
                predicate: x => x.isActive == true && x.FK_BranchID == model.branch_id && x.type == (int)SliderType.salons
                )).Select(x => new
                {
                    id = x.ID,
                    img = BaseUrlHoste + x.img,
                    type = x.type
                }).ToList();


            return Ok(new
            {
                key = 1,
                boutiques = boutiques,
                slider = slider
            });
        }

        [AllowAnonymous]
        [HttpPost(ApiRoutes.Client.Search)]
        public async Task<ActionResult> Search(SearchDTO model)
        {
            string userId = User.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;

            var boutiques = (await _providerAditionalData.Entity.GetAllAsync(
                predicate: x => (x.nameAr.Contains(model.text) || x.nameEn.Contains(model.text))
                    && x.FK_User.isActive == true && x.FK_User.FK_BranchID == model.branch_id && x.FK_User.activeCode == true,
                include: source => source.Include(i => i.SalonImages)
                )).Select(x => new
                {
                    id = x.ID,
                    img = BaseUrlHoste + x.SalonImages.Select(x => x.img).FirstOrDefault() ?? "",
                    name = CreatMessage(model.lang, x.nameAr, x.nameEn),
                    address = x.address,
                    rate = x.rate,
                    url = $"{BaseUrlHoste}show/share/{x.ID}",
                }).ToList();

            return Ok(new
            {
                key = 1,
                boutiques = boutiques
            });
        }

        [AllowAnonymous]
        [HttpPost(ApiRoutes.Client.SearchByService)]
        public async Task<ActionResult> SearchByService(SearchByServiceDTO model)
        {
            string userId = User.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;

            var boutiques = (await _providerAditionalData.Entity.GetAllAsync(
                predicate: x => (x.SubServices.Where(s => s.nameAr.Contains(model.text)).Any() || x.SubServices.Where(s => s.nameEn.Contains(model.text)).Any())
                    && x.FK_User.isActive == true && x.FK_User.FK_BranchID == model.branch_id && x.FK_User.activeCode == true,
                include: source => source.Include(i => i.SalonImages)
                )).Select(x => new
                {
                    id = x.ID,
                    img = BaseUrlHoste + x.SalonImages.Select(x => x.img).FirstOrDefault() ?? "",
                    name = CreatMessage(model.lang, x.nameAr, x.nameEn),
                    address = x.address,
                    rate = x.rate,
                    url = $"{BaseUrlHoste}show/share/{x.ID}",
                }).ToList();

            return Ok(new
            {
                key = 1,
                boutiques = boutiques
            });
        }

        [AllowAnonymous]
        [HttpPost(ApiRoutes.Client.ListMainServices)]
        public async Task<ActionResult> ListMainServices(ListMainServicesDTO model)
        {
            //var categories = _categories.Entity.GetAllAsync(predicate: x=>x.FK_BranchID)

            var mainServices = (await _mainServices.Entity.GetAllAsync(predicate: x => x.FK_BranchID == model.branch_id && (model.category_id != 0 ? x.FK_CategoryID == model.category_id : true) && x.isActive == true))
                .Select(s => new
                {
                    id = s.ID,
                    name = CreatMessage(model.lang, s.nameAr, s.nameEn)
                }).ToList();

            return Ok(new
            {
                key = 1,
                mainServices = mainServices,
            });
        }


        [AllowAnonymous]
        [HttpPost(ApiRoutes.Client.FilterBoutiques)]
        public async Task<ActionResult> FilterBoutiques(FilterBoutiquesDTO model)
        {
            string userId = User.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;

            // get current user
            //ApplicationDbUser user = await _users.Entity.GetAsync(u => u.Id == userId);


            List<int> mainServices = !String.IsNullOrEmpty(model.main_services) ? model.main_services.Split(',').Where(x => !String.IsNullOrEmpty(x)).Select(x => int.Parse(x)).ToList() : new List<int>();

            var boutiques = await _providerAditionalData.Entity.GetCustomAll(
                predicate: x => /*(model.rate != 0 ? (x.rate >= model.rate) : true)&&*/
                                 (mainServices.Count != 0 ? (x.SubServices.Select(b => b.FK_MainServiceID).Any(t => mainServices.Contains(t))) : true)
                                && x.SubServices.Where(x => x.price >= model.min_price && x.price <= model.max_price).Any()
                                && x.FK_User.FK_BranchID == model.branch_id 
                                && x.FK_User.isActive == true 
                                && x.FK_User.activeCode == true
                                && (model.categoryID != 0 ? (x.FK_CategoryID == model.categoryID) : true )
                ).Include(i => i.SubServices).Select(x => new
                {
                    //intersect = (x.SubServices.Select(b => b.FK_MainServiceID).Any(t => mainServices.Contains(t))),
                    id = x.ID,
                    img = BaseUrlHoste + x.SalonImages.Select(x => x.img).FirstOrDefault() ?? "",
                    name = CreatMessage(model.lang, x.nameAr, x.nameEn),
                    address = x.address,
                    lat = x.lat,
                    lng = x.lng,
                    rate = x.rate,
                    url = $"{BaseUrlHoste}show/share/{x.ID}",
                    //subServices = x.SubServices
                }).OrderByDescending(x => x.rate).ToListAsync();


            //boutiques = mainServices == null ? boutiques : boutiques.Where(x => (x.subServices.Select(y => y.FK_MainServiceID).Intersect(mainServices).Count() == mainServices.Count)).ToList();
            boutiques = model.top_rate ? boutiques.OrderByDescending(x => x.rate).ToList() : boutiques.OrderBy(x => x.rate).ToList();
            // check nullable of lat , lng based its value 0.0
            model.lat = double.TryParse(model.lat,out _) ? (double.Parse(model.lat) != 0.0 ? model.lat : "") : "";
            model.lng = double.TryParse(model.lng, out _) ? (double.Parse(model.lng) != 0.0 ? model.lng : "") : "";

            //boutiques = (String.IsNullOrEmpty(model.lat) || String.IsNullOrEmpty(model.lng)) ? boutiques : boutiques.OrderBy(x => GetDistance(model.lat, model.lng, x.lat, x.lng)).ToList();
            boutiques = (String.IsNullOrEmpty(model.lat) || String.IsNullOrEmpty(model.lng)) ? boutiques : boutiques.OrderBy(x => GetDistance(model.lat, model.lng, x.lat, x.lng)).ToList();


            return Ok(new
            {
                key = 1,
                boutiques = boutiques
            });
        }


        [AllowAnonymous]
        [HttpPost(ApiRoutes.Client.GetBoutique)]
        public async Task<ActionResult> GetBoutique(GetBoutiqueDTO model)
        {
            string userId = User.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;

            var cart_count = await _carts.Entity.GetCustomAll(predicate: x => x.FK_UserID == userId).CountAsync();
            var cart_price = await _carts.Entity.GetCustomAll(predicate: x => x.FK_UserID == userId).Select(x => x.FK_SubService.price).SumAsync();

            var main_services = await (_mainServices.Entity.GetCustomAll(predicate: m => m.SubServices.Where(ms => ms.FK_ProviderAdditionalDataID == model.boutique_id).Any() && m.isActive))
                 .Select(s => new
                 {
                     name = CreatMessage(model.lang, s.nameAr, s.nameEn),
                     sub_services = s.SubServices.Where(service => service.FK_ProviderAdditionalDataID == model.boutique_id).Select(service => new
                     {
                         id = service.ID,
                         name = CreatMessage(model.lang, service.nameAr, service.nameEn),
                         duration = service.duration,
                         price = service.price,
                         img = BaseUrlHoste + service.Image,
                         description = CreatMessage(model.lang, service.DescriptionAr, service.DescriptionEn),
                         found_in_cart = service.Carts.Where(x => x.FK_UserID == userId).Any(),
                         employees = service.Employees.Select(e => new
                         {
                             id = e.ID,
                             name = CreatMessage(model.lang, e.NameAr, e.NameEn),
                             img = Helper.Helper.BaseUrlHoste + e.Img
                         })
                     }).ToList()
                 }).ToListAsync();

            bool opened = false;
            try
            {
                var Boutique = _providerAditionalData.Entity.GetCustomAll(
                                predicate: x => x.ID == model.boutique_id && x.FK_User.isActive == true && x.FK_User.activeCode == true).Select(x => new
                                {
                                    dayWorks = x.dayWorks,
                                    time_from = x.timeForm,
                                    time_to = x.timeTo,
                                }).FirstOrDefault();
                opened = ((Boutique.dayWorks.Contains(((int)Helper.Helper.GetCurrentDate().DayOfWeek).ToString())) &&
                !(Boutique.time_from >= Boutique.time_from.Date.AddHours(Helper.Helper.GetCurrentDate().Hour) || Boutique.time_to <= Boutique.time_to.Date.AddHours(Helper.Helper.GetCurrentDate().Hour))
                );

            }
            catch (Exception ex)
            {
                string exx = ex.Message;
            }


            var newBoutiques = _providerAditionalData.Entity.GetCustomAll(
                predicate: x => x.ID == model.boutique_id && x.FK_User.isActive == true && x.FK_User.activeCode == true).Select(x => new
                {
                    id = x.ID,
                    img = x.SalonImages.Select(x => BaseUrlHoste + x.img).FirstOrDefault() ?? "",
                    imgs = x.SalonImages.Select(x => BaseUrlHoste + x.img).ToList(),
                    name = CreatMessage(model.lang, x.nameAr, x.nameEn),
                    description = CreatMessage(model.lang, x.descriptionAr, x.descriptionEn),
                    address = x.address,
                    instagramProfile = x.socialMediaProfile ?? "",
                    salonType = x.salonType,
                    SalonUsersType = x.SalonUsersType,
                    opened = opened,
                    lat = x.lat,
                    lng = x.lng,
                    rate = x.rate,
                    dayWorks = x.dayWorks,
                    time_from = x.timeForm.ToString("hh:mm tt"),
                    time_to = x.timeTo.ToString("hh:mm tt"),
                    days = GetDays(x.dayWorks, model.lang),
                    url = $"{BaseUrlHoste}show/share/{x.ID}",

                    initial_distance = 15,
                    initial_price = 10,
                    kilo_price = 1,

                    showEmployees = x.FK_CategoryID == 51,

                    offers = x.Offers.Select(x => BaseUrlHoste + x.img).ToList(),
                    employees = x.Employees.Select(e => new
                    {
                        id = e.ID,
                        name = Helper.Helper.CreatMessage(model.lang, e.NameAr, e.NameEn),
                        img = Helper.Helper.BaseUrlHoste + e.Img,
                        subServiceId = e.FK_SubServiceID,
                        subService = CreatMessage(model.lang, e.FK_SubService.nameAr, e.FK_SubService.nameEn)
                    }).ToList(),
                    //main_services = x.SubServices.Where(s => s.isActive == true).GroupBy(s => s.FK_MainServiceID).Select(s => s.FirstOrDefault())
                    //.Where(s => s.FK_MainService.isActive == true)
                    //.Select(s => new
                    //{
                    //    name = CreatMessage(model.lang, s.FK_MainService.nameAr, s.FK_MainService.nameEn),
                    //    sub_services = s.FK_MainService.SubServices.Where(service => service.FK_ProviderAdditionalDataID == x.ID).Select(service => new
                    //    {
                    //        id = service.ID,
                    //        name = CreatMessage(model.lang, service.nameAr, service.nameEn),
                    //        duration = service.duration,
                    //        price = service.price,
                    //        found_in_cart = service.Carts.Where(x => x.FK_UserID == userId).Any()
                    //    }).ToList()
                    //}).ToList(),      

                    main_services,
                    Rates = x.Orders.Where(r => r.rate != 0).Select(o => new
                    {
                        name = o.FK_User.fullName,
                        img = o.FK_User.img,
                        rate = o.rate
                    }).ToList(),
                    cart_count,
                    cart_price
                }).FirstOrDefault();



            //var boutiques = (await _providerAditionalData.Entity.GetAllAsync(
            //    predicate: x => x.ID == model.boutique_id && x.FK_User.isActive == true && x.FK_User.activeCode == true,
            //    include: source => source.Include(i => i.FK_User).Include(i => i.SalonImages).Include(x => x.Offers).Include(x => x.Orders).ThenInclude(x => x.FK_User)
            //                    .Include(i => i.SubServices).ThenInclude(i => i.FK_MainService).ThenInclude(i => i.SubServices).ThenInclude(i => i.Carts),
            //    disableTracking: false
            //    )).Select(x => new
            //    {
            //        id = x.ID,
            //        imgs = x.SalonImages.Select(x => BaseUrlHoste + x.img).ToList(),
            //        name = CreatMessage(model.lang, x.nameAr, x.nameEn),
            //        description = CreatMessage(model.lang, x.descriptionAr, x.descriptionEn),
            //        address = x.address,
            //        instagramProfile = x.socialMediaProfile ?? "",
            //        lat = x.lat,
            //        lng = x.lng,
            //        rate = x.rate,
            //        time_from = x.timeForm.ToString("hh:mm tt"),
            //        time_to = x.timeTo.ToString("hh:mm tt"),
            //        url = $"{BaseUrlHoste}show/share/{x.ID}",

            //        initial_distance = 15,
            //        initial_price = 10,
            //        kilo_price = 1,

            //        offers = x.Offers.Select(x => BaseUrlHoste + x.img).ToList(),
            //        main_services = x.SubServices.Where(s => s.isActive == true).GroupBy(s => s.FK_MainServiceID).Select(s => s.FirstOrDefault())
            //        .Where(s => s.FK_MainService.isActive == true)
            //        .Select(s => new
            //        {
            //            name = CreatMessage(model.lang, s.FK_MainService.nameAr, s.FK_MainService.nameEn),
            //            sub_services = s.FK_MainService.SubServices.Where(service => service.FK_ProviderAdditionalDataID == x.ID).Select(service => new
            //            {
            //                id = service.ID,
            //                name = CreatMessage(model.lang, service.nameAr, service.nameEn),
            //                duration = service.duration,
            //                price = service.price,
            //                found_in_cart = service.Carts.Where(x => x.FK_UserID == userId).Any()
            //            }).ToList()
            //        }).ToList(),

            //        Rates = x.Orders.Where(r => r.rate != 0).Select(o => new
            //        {
            //            name = o.FK_User.fullName,
            //            img = o.FK_User.img,
            //            rate = o.rate
            //        }).ToList(),
            //        cart_count
            //    }).FirstOrDefault();



            return Ok(new
            {
                key = 1,
                data = newBoutiques
            });
        }

        [AllowAnonymous]
        [HttpPost(ApiRoutes.Client.AddToCart)]
        public async Task<ActionResult> AddToCart(AddToCartDTO model)
        {
            string userId = User.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;
            int branchId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == "branch_id")?.Value ?? "0");
            var provider = await _subServices.Entity.GetCustomAll(p => p.ID == model.service_id).Select(p => p.FK_ProviderAdditionalData).FirstOrDefaultAsync();

            int cart_count = 0;
            string datestring = $"{model.date} {model.time}";
            DateTime dt = DateTime.ParseExact(datestring, "dd/MM/yyyy h:mm tt", System.Globalization.CultureInfo.InvariantCulture);

            var old_cart = (await _carts.Entity.GetAsync(x => x.FK_UserID == userId && x.FK_SubServiceID == model.service_id));
            if (old_cart != null)
            {

                old_cart.date = dt;
                old_cart.address = model.address;
                old_cart.lat = model.lat;
                old_cart.lng = model.lng;
                old_cart.note = model.notes;
                _carts.Entity.Update(old_cart);
                await _carts.Save();
                cart_count = _carts.Entity.GetCustomAll(predicate: x => x.FK_UserID == userId).Count();

                


                return Ok(new
                {
                    key = 1,
                    msg = CreatMessage(model.lang, "تم اضافة الخدمة الى السلة بنجاح", "service added to cart successfully"),
                    data = new
                    {
                        cart_count
                    }
                });
            }

           

                provider.timeForm = DateTime.Now.Date.AddHours(provider.timeForm.Hour).AddMinutes(provider.timeForm.Minute); // 23/2/2022 00:00:00
                provider.timeTo = provider.timeTo.Hour < provider.timeForm.Hour ? DateTime.Now.Date.AddDays(1).AddHours(provider.timeTo.Hour).AddMinutes(provider.timeTo.Minute) : DateTime.Now.Date.AddHours(provider.timeTo.Hour).AddMinutes(provider.timeTo.Minute); // 23/2/2022 00:00:00
                DateTime dateTimeToCheck = DateTime.ParseExact(DateTime.Now.ToString("dd/MM/yyyy") + " " + model.time, "dd/MM/yyyy h:mm tt", System.Globalization.CultureInfo.InvariantCulture);
                if (provider.timeForm >= dateTimeToCheck || provider.timeTo <= dateTimeToCheck)
                {
                    return Ok(new
                    {
                        key = 0,
                        msg = CreatMessage(model.lang, "لا يمكن الحجز خارج اوقات دوام الصالون", "Can't reseve outside work hours of the saloon")
                    });
                }
                try
                {
                    var days = provider.dayWorks.Split(',').Where(x => !String.IsNullOrEmpty(x)).Select(x => Convert.ToInt32(x)).ToList();
                    if (!days.Contains(((int)dt.DayOfWeek)))
                    {
                        return Ok(new
                        {
                            key = 0,
                            msg = CreatMessage(model.lang, "لا يمكن الحجز خارج اوقات دوام الصالون", "Can't reseve outside work hours of the saloon")
                        });
                    }

                }
                catch (Exception)
                {

                }

            


            Carts carts = new Carts()
            {
                FK_UserID = userId,
                date = dt,
                address = model.home ? model.address : string.Empty,
                lat = model.home ? model.lat : string.Empty,
                lng = model.home ? model.lng : string.Empty,
                FK_SubServiceID = model.service_id,
                note = model.notes

            };

            if (model.employeeId == 0)
            {
                carts.Fk_EmployeeID = null;
            }
            else
            {
                carts.Fk_EmployeeID = model.employeeId;
            }

            await _carts.Entity.InsertAsync(carts);
            await _carts.Save();
            var cart_count2 = _carts.Entity.GetCustomAll(predicate: x => x.FK_UserID == userId).Count();

            return Ok(new
            {
                key = 1,
                msg = CreatMessage(model.lang, "تم اضافة الخدمة الى السلة بنجاح", "service added to cart successfully"),
                data = new
                {
                    cart_count = cart_count2
                }
            });
        }


        [HttpPost(ApiRoutes.Client.GetCart)]
        public async Task<ActionResult> GetCart(GetCartDTO model)
        {
            string userId = User.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;
            var CurrentUser = await _users.Entity.FindByIdAsync(userId);
            int branchId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == "branch_id")?.Value ?? "0");
            var settingApp = (await _settings.Entity.GetAsync(predicate: x => x.FK_BranchID == branchId));
            //var cart = (await _carts.Entity.GetAllAsync(
            //    predicate: x => x.FK_UserID == userId,
            //    include: source => source.Include(i => i.FK_SubService).ThenInclude(i => i.FK_ProviderAdditionalData).ThenInclude(i => i.SalonImages)
            //    .Include(i => i.FK_User).ThenInclude(i => i.Carts).ThenInclude(i => i.FK_SubService).ThenInclude(i => i.FK_MainService),
            //    disableTracking: false

            var cart = (await _carts.Entity.GetCustomAll(
                predicate: x => x.FK_UserID == userId
                ).Include(c => c.FK_SubService).ThenInclude(c => c.FK_ProviderAdditionalData).Select(c => new CartVM
                {
                    boutique_id = c.FK_SubService.FK_ProviderAdditionalDataID,
                    img = c.FK_SubService.FK_ProviderAdditionalData.SalonImages.Select(x => BaseUrlHoste + x.img).FirstOrDefault() ?? "",
                    salonType = c.FK_SubService.FK_ProviderAdditionalData.salonType,
                    name = CreatMessage(model.lang, c.FK_SubService.FK_ProviderAdditionalData.nameAr, c.FK_SubService.FK_ProviderAdditionalData.nameEn),
                    providerName = c.FK_SubService.FK_ProviderAdditionalData.FK_User.fullName,
                    address = c.FK_SubService.FK_ProviderAdditionalData.address,
                    lat = c.FK_SubService.FK_ProviderAdditionalData.lat,
                    lng = c.FK_SubService.FK_ProviderAdditionalData.lng,
                    initial_distance = 15,
                    initial_price = 10,
                    kilo_price = 1,

                    rate = c.FK_SubService.FK_ProviderAdditionalData.rate,


                    //price = c.FK_User.Carts.Where(x => x.FK_SubService.FK_ProviderAdditionalDataID == c.FK_SubService.FK_ProviderAdditionalDataID)
                    //.Select(x => x.FK_SubService.price).Sum(),


                    price = // not >> add tax or not if (other)
                    c.FK_User.Carts.Where(x => x.FK_SubService.FK_ProviderAdditionalDataID == c.FK_SubService.FK_ProviderAdditionalDataID)
                    .Select(x => x.FK_SubService.price).Sum(),


                    services = c.FK_User.Carts
                    .Where(x => x.FK_SubService.FK_ProviderAdditionalDataID == c.FK_SubService.FK_ProviderAdditionalDataID)
                    .Select(x => new ServiceVM
                    {
                        id = x.FK_SubServiceID,
                        main_service = CreatMessage(model.lang, x.FK_SubService.FK_MainService.nameAr, x.FK_SubService.FK_MainService.nameEn),
                        name = CreatMessage(model.lang, x.FK_SubService.nameAr, x.FK_SubService.nameEn),
                        date = x.date.ToString("dd/MM/yyyy"),
                        time = x.date.ToString("hh:mm tt"),
                        address = x.address,
                        
                        servicePrice = x.FK_SubService.price - settingApp.appPrecent,
                        price = x.FK_SubService.price,

                        // Branch >> Show
                        priceAtHome = !String.IsNullOrEmpty(x.address) ? (x.FK_SubService.price + settingApp.TaxOfHome) : x.FK_SubService.price,
                        taxOfHome = settingApp.TaxOfHome,

                        addedValuePrice = settingApp.appPrecent,

                        at_boutique = String.IsNullOrEmpty(x.address),
                        distanceDetweenThem = !String.IsNullOrEmpty(x.address) ? Helper.Helper.GetDistance(x.lat, x.lng, x.FK_SubService.FK_ProviderAdditionalData.lat, x.FK_SubService.FK_ProviderAdditionalData.lng) : 0,
                        //priceOfDelivery= CalculateDeliveryPriceBasedOnDistance(Helper.Helper.GetDistance(x.FK_User.lat, x.lng, CurrentUser.lat, CurrentUser.lng)),
                        EmployeeID = x.Fk_EmployeeID,
                        employeeName = x.FK_SubService.Employees.Where(e => e.ID == x.Fk_EmployeeID).Select(e => CreatMessage(model.lang, e.NameAr, e.NameEn)).FirstOrDefault() ?? "",
                        employeeImg = x.FK_SubService.Employees.Where(e => e.ID == x.Fk_EmployeeID).Select(e => Helper.Helper.BaseUrlHoste + e.Img).FirstOrDefault() ?? "",
                        note = x.note
                    }),
                    atHome = c.FK_User.Carts.Where(x => x.FK_SubService.FK_ProviderAdditionalDataID == c.FK_SubService.FK_ProviderAdditionalDataID && !String.IsNullOrEmpty(x.address)).Any(),
                    note = c.note

                }).ToListAsync()).GroupBy(x => x.boutique_id).Select(x => x.FirstOrDefault()).ToList();
            



            return Ok(new
            {
                key = 1,
                data = cart
            });
        }

        [HttpPost(ApiRoutes.Client.GetCartWithDetails)]
        public async Task<ActionResult> GetCartWithDetails(GetCartWithDetailsDTO model)
        {
            string userId = User.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;

            //var cart = (await _carts.Entity.GetAllAsync(
            //    predicate: x => x.FK_UserID == userId,
            //    include: source => source.Include(i => i.FK_SubService).ThenInclude(i => i.FK_ProviderAdditionalData).ThenInclude(i => i.SalonImages)
            //    .Include(i => i.FK_User).ThenInclude(i => i.Carts).ThenInclude(i => i.FK_SubService).ThenInclude(i => i.FK_MainService),
            //    disableTracking: false
            var cart = (await _carts.Entity.GetCustomAll(
                predicate: x => x.FK_UserID == userId && x.FK_SubService.FK_ProviderAdditionalDataID == model.boutiqueId
                ).Select(c => new
                {
                    boutique_id = c.FK_SubService.FK_ProviderAdditionalDataID,
                    img = c.FK_SubService.FK_ProviderAdditionalData.SalonImages.Select(x => BaseUrlHoste + x.img).FirstOrDefault() ?? "",
                    salonType = c.FK_SubService.FK_ProviderAdditionalData.salonType,
                    name = CreatMessage(model.lang, c.FK_SubService.FK_ProviderAdditionalData.nameAr, c.FK_SubService.FK_ProviderAdditionalData.nameEn),
                    providerName = c.FK_SubService.FK_ProviderAdditionalData.FK_User.fullName,
                    address = c.FK_SubService.FK_ProviderAdditionalData.address,
                    lat = c.FK_SubService.FK_ProviderAdditionalData.lat,
                    lng = c.FK_SubService.FK_ProviderAdditionalData.lng,
                    initial_distance = 15,
                    initial_price = 10,
                    kilo_price = 1,

                    rate = c.FK_SubService.FK_ProviderAdditionalData.rate,
                    price = c.FK_User.Carts.Where(x => x.FK_SubService.FK_ProviderAdditionalDataID == c.FK_SubService.FK_ProviderAdditionalDataID)
                    .Select(x => x.FK_SubService.price).Sum(),
                    services = c.FK_User.Carts.Where(x => x.FK_SubService.FK_ProviderAdditionalDataID == c.FK_SubService.FK_ProviderAdditionalDataID).Select(x => new
                    {
                        id = x.FK_SubServiceID,
                        main_service = CreatMessage(model.lang, x.FK_SubService.FK_MainService.nameAr, x.FK_SubService.FK_MainService.nameEn),
                        name = CreatMessage(model.lang, x.FK_SubService.nameAr, x.FK_SubService.nameEn),
                        date = x.date.ToString("dd/MM/yyyy"),
                        time = x.date.ToString("hh:mm tt"),
                        address = x.address,
                        price = x.FK_SubService.price,
                        at_boutique = String.IsNullOrEmpty(x.address),

                    }),
                    note = c.note
                }).ToListAsync()).GroupBy(x => x.boutique_id).Select(x => x.FirstOrDefault()).FirstOrDefault();

            return Ok(new
            {
                key = 1,
                data = cart
            });
        }

        [HttpPost(ApiRoutes.Client.DeleteServiceFromCart)]
        public async Task<ActionResult> DeleteServiceFromCart(DeleteServiceFromCartDTO model)
        {
            string userId = User.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;

            var cart = await _carts.Entity.GetAsync(predicate: x => x.FK_SubServiceID == model.service_id && x.FK_UserID == userId
            , include: source => source.Include(i => i.FK_SubService),
            disableTracking: true);
            if (cart == null)
            {
                return Ok(new
                {
                    key = 0,
                    msg = CreatMessage(model.lang, "الخدمة غير موجوده داخل السلة", "service not found at cart")
                });
            }
            int boutique_id = cart.FK_SubService.FK_ProviderAdditionalDataID;
            _carts.Entity.Delete(cart);
            await _carts.Save();
            var cart_count = _carts.Entity.GetCustomAll(predicate: x => x.FK_UserID == userId).Count();

            var cart_data = (await _carts.Entity.GetAllAsync(
                predicate: x => x.FK_UserID == userId,
                include: source => source.Include(i => i.FK_SubService).ThenInclude(i => i.FK_ProviderAdditionalData).ThenInclude(i => i.SalonImages)
                .Include(i => i.FK_User).ThenInclude(i => i.Carts).ThenInclude(i => i.FK_SubService).ThenInclude(i => i.FK_MainService),
                disableTracking: false
                )).Select(c => new
                {
                    boutique_id = c.FK_SubService.FK_ProviderAdditionalDataID,
                    img = c.FK_SubService.FK_ProviderAdditionalData.SalonImages.Select(x => BaseUrlHoste + x.img).FirstOrDefault() ?? "",
                    name = CreatMessage(model.lang, c.FK_SubService.FK_ProviderAdditionalData.nameAr, c.FK_SubService.FK_ProviderAdditionalData.nameEn),
                    address = c.FK_SubService.FK_ProviderAdditionalData.address,
                    rate = c.FK_SubService.FK_ProviderAdditionalData.rate,
                    price = c.FK_User.Carts.Where(x => x.FK_SubService.FK_ProviderAdditionalDataID == c.FK_SubService.FK_ProviderAdditionalDataID)
                    .Select(x => x.FK_SubService.price).Sum(),
                    services = c.FK_User.Carts.Where(x => x.FK_SubService.FK_ProviderAdditionalDataID == c.FK_SubService.FK_ProviderAdditionalDataID).Select(x => new
                    {
                        id = x.FK_SubServiceID,
                        main_service = CreatMessage(model.lang, x.FK_SubService.FK_MainService.nameAr, x.FK_SubService.FK_MainService.nameEn),
                        name = CreatMessage(model.lang, x.FK_SubService.nameAr, x.FK_SubService.nameEn),
                        date = x.date.ToString("dd/MM/yyyy hh:mm tt"),
                        price = x.FK_SubService.price
                    }),
                    cart_count
                }).GroupBy(x => x.boutique_id).Select(x => x.FirstOrDefault()).ToList();
            return Ok(new
            {
                key = 1,
                data = cart_data,
                msg = CreatMessage(model.lang, "تم حذف الخدمة من السلة", "service deleted from cart")
            });
        }

        [HttpPost(ApiRoutes.Client.CheckOrder)]
        public async Task<ActionResult> CheckOrder(SaveOrderDTO model)
        {
            string userId = User.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;
            int branchId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == "branch_id")?.Value ?? "0");
            var settingAppPrecent = (await _settings.Entity.GetAsync(predicate: x => x.FK_BranchID == branchId));

            var provider = await _providerAditionalData.Entity.FindByIdAsync(model.boutique_id);



            var cart = (await _carts.Entity.GetAllAsync(predicate: x => x.FK_UserID == userId && x.FK_SubService.FK_ProviderAdditionalDataID == model.boutique_id,
                include: source => source.Include(i => i.FK_SubService).ThenInclude(i => i.FK_MainService),
            disableTracking: false)).ToList();

            var copon = await _copons.Entity.GetAsync(predicate: x => x.code == model.code && x.FK_Branch == branchId,
            disableTracking: true);
            if (cart.Count() == 0)
            {
                return Ok(new
                {
                    key = 0,
                    msg = CreatMessage(model.lang, "الخدمة غير موجوده داخل السلة", "service not found at cart")
                });
            }

            //if (model.type_pay == (int)TypePay.online && !(branchId == 3 || branchId == 4 || branchId == 5 || branchId == 6 || branchId == 9))
            //{
            //    return Ok(new
            //    {
            //        key = 0,
            //        msg = CreatMessage(model.lang, "خدمة الدفع اونلاين غير متاحة حاليا", "online pay not avilable now")
            //    });
            //}

            if (copon == null && !String.IsNullOrEmpty(model.code))
            {
                return Ok(new
                {
                    key = 0,
                    msg = CreatMessage(model.lang, "تأكد من كود الخصم", "check valid discount code")
                });
            }
            var coponused = await _orders.Entity.GetCustomAll(x => x.copon == model.code && x.FK_UserID == userId).AnyAsync();
            if (coponused && !String.IsNullOrEmpty(model.code))
            {
                return Ok(new
                {
                    key = 0,
                    msg = CreatMessage(model.lang, "تم استخدام كود الخصم من قبل", "copon used before")
                });
            }
            if (false)
            //if (branchId == 8)
            {

                provider.timeForm = DateTime.Now.Date.AddHours(provider.timeForm.Hour); // 23/2/2022 00:00:00
                provider.timeTo = DateTime.Now.Date.AddHours(provider.timeTo.Hour); // 23/2/2022 00:00:00
                DateTime dateTimeToCheck = DateTime.ParseExact(DateTime.Now.ToString("dd/MM/yyyy") + " " + model.time, "dd/MM/yyyy hh:mm tt", System.Globalization.CultureInfo.InvariantCulture);
                if (provider.timeForm >= dateTimeToCheck || provider.timeTo <= dateTimeToCheck)
                {
                    return Ok(new
                    {
                        key = 0,
                        msg = CreatMessage(model.lang, "لا يمكن الحجز خارج اوقات دوام الصالون", "Can't reseve outside work hours of the saloon")
                    });
                }
                try
                {
                    var days = provider.dayWorks.Split(',').Where(x => !String.IsNullOrEmpty(x)).Select(x => Convert.ToInt32(x)).ToList();
                    string datestring = $"{model.date} {model.time}";
                    DateTime dt = DateTime.ParseExact(datestring, "dd/MM/yyyy hh:mm tt", System.Globalization.CultureInfo.InvariantCulture);
                    if (!days.Contains(((int)dt.DayOfWeek)))
                    {
                        return Ok(new
                        {
                            key = 0,
                            msg = CreatMessage(model.lang, "لا يمكن الحجز خارج اوقات دوام الصالون", "Can't reseve outside work hours of the saloon")
                        });
                    }

                }
                catch (Exception)
                {

                }

            }


            return Ok(new
            {
                key = 1,
                msg = "true"
            });
        }

        [HttpPost(ApiRoutes.Client.SaveOrder)]
        public async Task<ActionResult> SaveOrder(SaveOrderDTO model)
        {
            string userId = User.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;
            var CurrentUser = await _users.Entity.FindByIdAsync(userId);
            int branchId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == "branch_id")?.Value ?? "0");
            var settingAppPrecent = (await _settings.Entity.GetAsync(predicate: x => x.FK_BranchID == branchId));

            var provider = await _providerAditionalData.Entity.FindByIdAsync(model.boutique_id);
            var user = await _users.Entity.FindByIdAsync(userId);

            var cart = (await _carts.Entity.GetAllAsync(predicate: x => x.FK_UserID == userId && x.FK_SubService.FK_ProviderAdditionalDataID == model.boutique_id,
                 include: source => source.Include(i => i.FK_SubService).ThenInclude(i => i.FK_MainService)
                                    .Include(i => i.FK_SubService).ThenInclude(i => i.FK_ProviderAdditionalData),
             disableTracking: false)).ToList();

            var copon = await _copons.Entity.GetAsync(predicate: x => x.code == model.code && x.FK_Branch == branchId,
            disableTracking: true);
            if (cart.Count() == 0)
            {
                return Ok(new
                {
                    key = 0,
                    msg = CreatMessage(model.lang, "الخدمة غير موجوده داخل السلة", "service not found at cart")
                });
            }
            //if (model.type_pay == (int)TypePay.online && !(branchId == 3 || branchId == 4 || branchId == 5 || branchId == 6))
            //{
            //    return Ok(new
            //    {
            //        key = 0,
            //        msg = CreatMessage(model.lang, "خدمة الدفع اونلاين غير متاحة حاليا", "online pay not avilable now")
            //    });
            //}

            if (copon == null && !String.IsNullOrEmpty(model.code))
            {
                return Ok(new
                {
                    key = 0,
                    msg = CreatMessage(model.lang, "تأكد من كود الخصم", "check valid discount code")
                });
            }
            var coponused = await _orders.Entity.GetCustomAll(x => x.copon == model.code && x.FK_UserID == userId).AnyAsync();
            if (coponused && !String.IsNullOrEmpty(model.code))
            {
                return Ok(new
                {
                    key = 0,
                    msg = CreatMessage(model.lang, "تم استخدام كود الخصم من قبل", "copon used before")
                });
            }

            double orderPrice = 0.0;
            //// check if branch Show 
            //if (branchId == (int)BranchName.Show)
            //{
            //    orderPrice = cart.Select(x => x.FK_SubService.price).Sum();
            //}
            //else
            //{
            //    orderPrice = cart.Select(x => x.FK_SubService.price).Sum();
            //}
            orderPrice = cart.Select(x => x.FK_SubService.price).Sum();




            string datestring = $"{model.date} {model.time}";
            DateTime dt = DateTime.ParseExact(datestring, "dd/MM/yyyy hh:mm tt", System.Globalization.CultureInfo.InvariantCulture);

            #region commented code
            //if (false)
            ////if (branchId == 8)
            //{

            //    provider.timeForm = DateTime.Now.Date.AddHours(provider.timeForm.Hour); // 23/2/2022 00:00:00
            //    provider.timeTo = DateTime.Now.Date.AddHours(provider.timeTo.Hour); // 23/2/2022 00:00:00
            //    DateTime dateTimeToCheck = DateTime.ParseExact(DateTime.Now.ToString("dd/MM/yyyy") + " " + model.time, "dd/MM/yyyy hh:mm tt", System.Globalization.CultureInfo.InvariantCulture);
            //    if (provider.timeForm >= dateTimeToCheck || provider.timeTo <= dateTimeToCheck)
            //    {
            //        return Ok(new
            //        {
            //            key = 0,
            //            msg = CreatMessage(model.lang, "لا يمكن الحجز خارج اوقات دوام الصالون", "Can't reseve outside work hours of the saloon")
            //        });
            //    }

            //    try
            //    {
            //        var days = provider.dayWorks.Split(',').Where(x => !String.IsNullOrEmpty(x)).Select(x => Convert.ToInt32(x)).ToList();
            //        if (!days.Contains(((int)dt.DayOfWeek)))
            //        {
            //            return Ok(new
            //            {
            //                key = 0,
            //                msg = CreatMessage(model.lang, "لا يمكن الحجز خارج اوقات دوام الصالون", "Can't reseve outside work hours of the saloon")
            //            });
            //        }

            //    }
            //    catch (Exception)
            //    {

            //    }
            //} 
            #endregion

            var employees = await _employees.Entity.GetCustomAll(x => x.FK_ProviderAdditionalDataID == model.boutique_id).ToListAsync();
            Orders order = null;

            order = new Orders()
            {
                FK_UserID = userId,
                copon = copon?.code,
                discountPercentage = copon?.discPercentage ?? 0,
                date = GetCurrentDate(),
                FK_ProviderID = model.boutique_id,
                paid = /*true*/ model.type_pay == (int)TypePay.online ? false : true,
                typePay = model.type_pay,
                status = (int)OrderStates.waiting,
                commentNote = model.Notes,
                priceBeforeDisc = orderPrice,
                //price = orderPrice - ((orderPrice * copon?.discPercentage ?? 0) / 100),
                price = // not >> add tax or not if (other)
                    orderPrice - ((orderPrice * copon?.discPercentage ?? 0) / 100),

                shippingPrice = model.shippingPrice,
                orderDate = dt,
                address = model.home ? model.address : string.Empty,
                lat = model.home ? model.lat : string.Empty,
                lng = model.home ? model.lng : string.Empty,

                Applicationpercentagepaid = model.type_pay == (int)TypePay.online ? true : false,


                Applicationpercentage = SaveOrder_Applicationpercentage(branchId, cart.Count(), settingAppPrecent.appPrecent, orderPrice),

                Adminpercentage = SaveOrder_Adminpercentage(branchId, cart.Count(), settingAppPrecent.appPrecent, orderPrice),
                Providerpercentage = SaveOrder_Providerpercentage(branchId, cart.Count(), settingAppPrecent.appPrecent, orderPrice),
                ApplicationProviderpercentagepaid = false,

                //التغير ------

                AppCommission = SaveOrder_AppCommission(branchId, cart.Count(), settingAppPrecent.appPrecent, orderPrice),
                Deposit = settingAppPrecent.Deposit,

                //--------------
                // update payment id in the order
                PaymentId = user.TempPaymentId,
                //--------------

                OrderServices = cart.Select(x => new OrderServices()
                {
                    SubServiceID = x.FK_SubServiceID,
                    SubServicNameAr = x.FK_SubService.nameAr,
                    SubServicNameEn = x.FK_SubService.nameEn,
                    mainServiceID = x.FK_SubService.FK_MainServiceID,
                    mainServiceNameAr = x.FK_SubService.FK_MainService.nameAr,
                    mainServiceNameEn = x.FK_SubService.FK_MainService.nameEn,
                    date = x.date,
                    address = x.address,
                    duration = x.FK_SubService.duration,
                    lat = x.lat,
                    lng = x.lng,
                    price = x.FK_SubService.price,
                    //price = (branchId == (int)BranchName.Show ?
                    //!String.IsNullOrEmpty(x.address) ? (x.FK_SubService.price + settingAppPrecent.TaxOfHome) : x.FK_SubService.price
                    //:
                    //x.FK_SubService.price
                    //),


                    // Branch >> Show 
                    priceAtHome = !String.IsNullOrEmpty(x.address) ? (x.FK_SubService.price + settingAppPrecent.TaxOfHome) : x.FK_SubService.price,



                    Image = x.FK_SubService.Image,
                    DescriptionAr = x.FK_SubService.DescriptionAr,
                    DescriptionEn = x.FK_SubService.DescriptionEn,

                    EmployeeId = employees.Where(e => e.ID == x.Fk_EmployeeID).Select(x => x.ID).FirstOrDefault(),
                    EmployeeNameAr = employees.Where(e => e.ID == x.Fk_EmployeeID).Select(x => x.NameAr).FirstOrDefault(),
                    EmployeeNameEn = employees.Where(e => e.ID == x.Fk_EmployeeID).Select(x => x.NameEn).FirstOrDefault(),
                    EmployeeImg = employees.Where(e => e.ID == x.Fk_EmployeeID).Select(x => x.Img).FirstOrDefault(),


                }).ToList()


            };
            //if (branchId == (int)BranchName.Eleklil)
            //{
            //    double totalservicesPrice = 0;
            //    IEnumerable<ServiceDelivery> serviceDeliveries = await _serviceDelivery.Entity.GetAllAsync();
            //    foreach (var service in order.OrderServices)
            //    {

            //        if (!string.IsNullOrEmpty(service.address))
            //        {
            //            var distance = Helper.Helper.GetDistance(model.lat, model.lng, CurrentUser.lat, CurrentUser.lng);
            //            service.deliveryPrice = serviceDeliveries.Where(s => distance >= s.FromInKM && distance <= s.ToInKM).FirstOrDefault() != null ? service.deliveryPrice = serviceDeliveries.Where(s => distance >= s.FromInKM && distance <= s.ToInKM).FirstOrDefault().DeliveryPrice : serviceDeliveries.Select(s=>s.DeliveryPrice).Max();
            //            totalservicesPrice += service.deliveryPrice;
            //        }


            //    }
            //    order.price = order.price + totalservicesPrice; 

            //}
            if (model.type_pay == (int)TypePay.wallet)
            {
                if (user.userWallet < order.price)
                {
                    return Ok(new
                    {
                        key = 0,
                        msg = CreatMessage(model.lang, "المبلغ الموجود فى المحفظة غير كافى", "wallet amount not enough")
                    });
                }

                user.userWallet -= order.price;
                _users.Entity.Update(user);

            }

            await _orders.Entity.InsertAsync(order);
            _carts.Entity.DeleteRange(cart);
            await _orders.Save();
            var result = await SendProviderNotifications(order.ID, userId, provider.ID);

            //if (model.type_pay == (int)TypePay.cash)
            //{
            //    var result = await SendProviderNotifications(order.ID, userId, provider.ID);
            //    //await _notifications.Entity.InsertRangeAsync(new List<Notifications>() {
            //    //                                            new Notifications(){
            //    //                                                FK_OrderID = order.ID,
            //    //                                                FK_UserID = userId,
            //    //                                                msgAr=$"تم ارسال طلبك رقم {order.ID} بنجاح",
            //    //                                                msgEn=$"your order number {order.ID} sent successfully",
            //    //                                                show = false,
            //    //                                                date = GetCurrentDate()
            //    //                                            },
            //    //                                            new Notifications(){
            //    //                                                FK_OrderID = order.ID,
            //    //                                                FK_UserID = provider.FK_UserID,
            //    //                                                msgAr=$"لديك طلب جديد برقم {order.ID} وف انتظار دفع العميل",
            //    //                                                msgEn=$"you have new order number {order.ID},waitting provider to pay",
            //    //                                                show = false,
            //    //                                                date = GetCurrentDate()
            //    //                                            }
            //    //                                        });
            //    //await _notifications.Save();

            //    //var userDataAndIds = await _users.Entity.GetCustomAll(predicate: x => x.ProviderAditionalData.ID == model.boutique_id).Select(x => new
            //    //{
            //    //    lang = x.lang,
            //    //    ids = x.DeviceIds.Select(y => y.deviceID).ToList(),
            //    //    branchId = x.FK_BranchID,
            //    //}).FirstOrDefaultAsync();

            //    //var projectName = await _branches.Entity.GetCustomAll(x => x.ID == userDataAndIds.branchId).Select(x => x.nameAr + " | " + x.nameEn).FirstOrDefaultAsync();
            //    //SendPushNotification(userDataAndIds.ids, CreatMessage(userDataAndIds.lang, $"لديك طلب جديد برقم {order.ID}", $"you have new order number {order.ID}"), order.ID, projectName: projectName);
            //}
            var cart_count = _carts.Entity.GetCustomAll(predicate: x => x.FK_UserID == userId).Count();
            var userPoint = await _users.Entity.GetAsync(z => z.Id == userId);
            userPoint.PointsBalance += 1;
            _users.Entity.Update(userPoint);
            return Ok(new
            {
                key = 1,
                msg = CreatMessage(model.lang, "تم ارسال الطلب بنجاح", "order sent successfully"),
                data = new
                {
                    cart_count,
                    orderId = order.ID
                },
                cart_count
            });
        }



        [HttpPost(ApiRoutes.Client.ListOrders)]
        public async Task<ActionResult> ListOrders(ListOrdersDTO model)
        {
            string userId = User.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;
            int branchId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == "branch_id")?.Value ?? "0");
            var settingApp = (await _settings.Entity.GetAsync(predicate: x => x.FK_BranchID == branchId));

            //var orders = (await _orders.Entity.GetAllAsync(predicate: x => x.FK_UserID == userId && !x.IsDeleted,
            //    include: source => source.Include(i => i.FK_Provider).ThenInclude(i => i.SalonImages).Include(i => i.OrderServices),
            //    disableTracking: true))
            var orders = await _orders.Entity.GetCustomAll(predicate: x => x.FK_UserID == userId && !x.IsDeleted)

            .Select(order => new
            {
                id = order.ID,
                boutique_id = order.FK_ProviderID,
                boutique_name = CreatMessage(model.lang, order.FK_Provider.nameAr, order.FK_Provider.nameEn),
                providerName = order.FK_Provider.FK_User.fullName,
                boutique_address = order.FK_Provider.address,
                boutique_img = order.FK_Provider.SalonImages.Select(x => BaseUrlHoste + x.img).FirstOrDefault() ?? "",
                rate = order.FK_Provider.rate,
                status = order.status,
                price = order.price,
                Paied = order.Applicationpercentagepaid,
                orderPaid = order.paid,
                applicationPercent = order.Applicationpercentage,
                applicationPercentImg = BaseUrlHoste + order.ApplicationpercentageImg,
                status_name = GetOrderStatus(order.status, model.lang),
                type_pay = order.typePay,
                date = order.date.ToString("dd/MM/yyyy HH:mm tt"),
                refused_reason = order.refusedReason,
                services = order.OrderServices.Select(x => new
                {
                    id = x.SubServiceID,
                    name = CreatMessage(model.lang, x.SubServicNameAr, x.SubServicNameEn),
                    main_service_name = CreatMessage(model.lang, x.mainServiceNameAr, x.mainServiceNameEn),
                    date = x.date.ToString("dd/MM/yyyy"),
                    time = x.date.ToString("hh:mm tt"),
                    price = x.price,
                    priceAtHome = x.priceAtHome,
                    servicePrice = x.price - settingApp.appPrecent,
                    addedValuePrice = settingApp.appPrecent,
                    duration = x.duration,
                    at_boutique = String.IsNullOrEmpty(x.address),
                    address = x.address,
                    lat = x.lat,
                    lng = x.lng
                }).ToList(),
                atHome = order.OrderServices.Where(x => !String.IsNullOrEmpty(x.address)).Any()

            }).OrderByDescending(x => x.id).ToListAsync();

            return Ok(new
            {
                key = 1,
                current_orders = orders.Where(x => x.status == (int)OrderStates.waiting || x.status == (int)OrderStates.accepted),
                finished_orders = orders.Where(x => x.status != (int)OrderStates.waiting && x.status != (int)OrderStates.accepted),
            });
        }

        [HttpPost(ApiRoutes.Client.ListAllOrders)]
        public async Task<ActionResult> ListAllOrders(ListAllOrdersDTO model)
        {
            string userId = User.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;

            var orders = (await _orders.Entity.GetAllAsync(predicate: x => x.FK_UserID == userId,
                include: source => source.Include(i => i.FK_Provider).ThenInclude(i => i.SalonImages).Include(i => i.OrderServices),
                disableTracking: true))
            .Select(order => new
            {
                id = order.ID,
                boutique_id = order.FK_ProviderID,
                boutique_name = CreatMessage(model.lang, order.FK_Provider.nameAr, order.FK_Provider.nameEn),
                boutique_address = order.FK_Provider.address,
                boutique_img = order.FK_Provider.SalonImages.Select(x => BaseUrlHoste + x.img).FirstOrDefault() ?? "",
                rate = order.FK_Provider.rate,
                status = order.status,
                price = order.price,
                Paied = order.Applicationpercentagepaid,
                orderPaid = order.paid,
                applicationPercent = order.Applicationpercentage,
                applicationPercentImg = BaseUrlHoste + order.ApplicationpercentageImg,
                status_name = order.status switch
                {
                    0 => CreatMessage(model.lang, "جديد", "new"),
                    1 => CreatMessage(model.lang, "مقبول", "accepted"),
                    2 => CreatMessage(model.lang, "منتهى", "finished"),
                    3 => CreatMessage(model.lang, "مرفوض", "refused"),
                    4 => CreatMessage(model.lang, "ملغى", "canceled"),
                    _ => CreatMessage(model.lang, "خطأ", "error")
                },
                type_pay = order.typePay,
                date = order.date.ToString("dd/MM/yyyy HH:mm tt"),
                refused_reason = order.refusedReason,
                services = order.OrderServices.Select(x => new
                {
                    id = x.SubServiceID,
                    name = CreatMessage(model.lang, x.SubServicNameAr, x.SubServicNameEn),
                    main_service_name = CreatMessage(model.lang, x.mainServiceNameAr, x.mainServiceNameEn),
                    date = x.date.ToString("dd/MM/yyyy"),
                    time = x.date.ToString("hh:mm tt"),
                    price = x.price,
                    duration = x.duration,
                    at_boutique = String.IsNullOrEmpty(x.address),
                    address = x.address,
                    lat = x.lat,
                    lng = x.lng
                }).ToList()
            }).OrderByDescending(x => x.id).ToList();

            return Ok(new
            {
                key = 1,
                waiting_orders = orders.Where(x => x.status == (int)OrderStates.waiting),
                accepted_orders = orders.Where(x => x.status == (int)OrderStates.accepted),
                finished_orders = orders.Where(x => x.status != (int)OrderStates.waiting && x.status != (int)OrderStates.accepted),
            });
        }

        [HttpPost(ApiRoutes.Client.PayOrder)]
        public async Task<ActionResult> PayOrder(PayOrderDTO model)
        {
            string userId = User.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;
            int branchId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == "branch_id")?.Value ?? "0");
            var user = await _users.Entity.FindByIdAsync(userId);
            var order = (await _orders.Entity.GetAsync(predicate: x => x.ID == model.order_id,
                //include: source => source.Include(i => i.FK_Provider).ThenInclude(i => i.SalonImages).Include(i=>i.OrderServices),
                disableTracking: false));

            order.ApplicationpercentageImg = Helper.Helper.ProcessUploadedFile(HostingEnvironment: _HostingEnvironment, model.img, "Users");
            

            _orders.Entity.Update(order);
            _users.Entity.Update(user);
            await _orders.Save();
            await _users.Save();

            

            return Ok(new
            {
                key = 1,
                msg = Helper.Helper.CreatMessage(model.lang, "تم ارسال الايصال بنجاح", "paied successfully")
            });
        }

        [HttpPost(ApiRoutes.Client.CancelOrder)]
        public async Task<ActionResult> CancelOrder(CancelOrderDTO model)
        {
            //PaymentHayaaController paymentHayaaController = new PaymentHayaaController(_notifications, _providerAditionalData, _users, _branches, _orders);

            string userId = User.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;

            int branchId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == "branch_id")?.Value ?? "0");


            //var order = await _orders.Entity.FindByIdAsync(model.order_id);
            var order = await _orders.Entity.GetAsync(predicate: o => o.ID == model.order_id, include: source => source.Include(i => i.FK_User).Include(i => i.FK_Provider).ThenInclude(i => i.FK_User), disableTracking: false);

            //var provider = await _providerAditionalData.Entity.FindByIdAsync(order.FK_ProviderID);
            //var provider = await _providerAditionalData.Entity.FindByIdAsync(order.FK_ProviderID);

            if (order.status != (int)OrderStates.waiting)
            {
                return Ok(new
                {
                    key = 0,
                    msg = CreatMessage(model.lang, "لايمكن الغاء هذا الطلب", "can't cancel that order")
                });
            }


            //var successCancel = await paymentHayaaController.RefundRequest(order.ID, order.price.ToString());

            //if (successCancel)
            //{
            order.status = (int)OrderStates.canceled;
            order.refusedReason = model.refusedReason;
            order.FK_User.userWallet += order.typePay == (int)TypePay.online ? order.price : 0;

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
                                                                FK_UserID = userId,
                                                                msgAr=$"تم الغاء طلبك رقم {order.ID} ",
                                                                msgEn=$"your order number {order.ID} canceled ",
                                                                show = false,
                                                                date = GetCurrentDate()
                                                            },
                                                            new Notifications(){
                                                                FK_OrderID = order.ID,
                                                                FK_UserID =order.FK_Provider.FK_UserID,
                                                                msgAr=$"تم الغاء طلب رقم {order.ID} ",
                                                                msgEn=$" order number {order.ID} canceled ",
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
                var projectName = await _branches.Entity.GetCustomAll(x => x.ID == item.branchId).Select(x => x.nameAr + " | " + x.nameEn).FirstOrDefaultAsync();
                SendPushNotification(device_ids: item.ids, msg: CreatMessage(item.lang, fcm_client_msg[0], fcm_client_msg[1]), order_id: order.ID, branchId: branchId, projectName: projectName);

            }


            return Ok(new
            {
                key = 1,
                msg = CreatMessage(model.lang, "تم الغاء الطلب بنجاح", "order canceled successfully")
            });
            //}
            //return Ok(new
            //{
            //    key = 0,
            //    ms*g = CreatMessage(model.lang, "حدث مشلكة في الغاء الطلب", "An error occured in canceling the order")
            //});



        }

        [HttpPost(ApiRoutes.Client.RateOrder)]
        public async Task<ActionResult> RateOrder(RateOrderDTO model)
        {
            string userId = User.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;
            int branchId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == "branch_id")?.Value ?? "0");

            var order = await _orders.Entity.FindByIdAsync(model.order_id);

            if (order.status != (int)OrderStates.finished || order.rate != 0)
            {
                return Ok(new
                {
                    key = 0,
                    msg = CreatMessage(model.lang, "لقد قمت بتقييم الطلب مسبقا", "You have rated the order before")
                });
            }

            order.rate = model.rate;
            order.rateComment = model.comment;
            
            _orders.Entity.Update(order);
            await _orders.Save();

            var provider = await _providerAditionalData.Entity.GetAsync(predicate: x => x.ID == order.FK_ProviderID,
                include: source => source.Include(i => i.Orders));
            provider.rate = (int)provider.Orders.Where(x => x.rate != 0).Select(x => x.rate).Average();
            _providerAditionalData.Entity.Update(provider);
            await _providerAditionalData.Save();

            await _notifications.Entity.InsertRangeAsync(new List<Notifications>() {
                                                            new Notifications(){
                                                                FK_OrderID = order.ID,
                                                                FK_UserID = provider.FK_UserID,
                                                                msgAr=$"تم تقييم طلبك رقم {order.ID} ",
                                                                msgEn=$"Your order number {order.ID} was rated ",
                                                                show = false,
                                                                date = GetCurrentDate()
                                                            }
                                                        });
            await _notifications.Save();

            var userDataAndIds = await _users.Entity.GetCustomAll(predicate: x => x.ProviderAditionalData.ID == provider.ID).Select(x => new
            {
                lang = x.lang,
                ids = x.DeviceIds.Select(y => y.deviceID).ToList(),
                branchId = x.FK_BranchID,
            }).FirstOrDefaultAsync();

            var projectName = await _branches.Entity.GetCustomAll(x => x.ID == userDataAndIds.branchId).Select(x => x.nameAr + " | " + x.nameEn).FirstOrDefaultAsync();

            Helper.Helper.SendPushNotification(device_ids: userDataAndIds.ids, msg: Helper.Helper.CreatMessage(userDataAndIds.lang, $"تم تقييم طلبك رقم  {order.ID}", $"Your order number {order.ID} was rated"), order_id: order.ID, branchId: branchId, projectName: projectName);


            return Ok(new
            {
                key = 1,
                msg = CreatMessage(model.lang, "تم تقييم الطلب بنجاح", "order rated successfully"),
                data = new { }
            });

        }

        [HttpPost(ApiRoutes.Client.GetDiscountPercentage)]
        public async Task<ActionResult> GetDiscountPercentage(GetDiscountPercentageDTO model)
        {
            string userId = User.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;

            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return Ok(new
                {
                    key = 0,
                    data = 0,
                    allErrors,
                });
            }

            var copon = await _copons.Entity.GetAsync(predicate: x => x.code == model.copon && x.isActive == true);
            if (copon == null)
            {
                return Ok(new
                {
                    key = 0,
                    data = 0,
                    msg = CreatMessage(model.lang, "تأكد من الكوبون", "Failed copon")
                });
            }

            var orderFound = await _orders.Entity.GetCustomAll(o => o.copon == model.copon && o.FK_UserID == userId).AnyAsync();
            if (orderFound)
            {
                return Ok(new
                {
                    key = 0,
                    data = 0,
                    msg = CreatMessage(model.lang, "هذا الكوبون مستخدم من قبل", "your copon used")
                });
            }



            return Ok(new
            {
                key = 1,
                data = copon.discPercentage / 100,
                msg = CreatMessage(model.lang, "تم الخصم بنجاح", "Discounted successfully")
            });
        }

        [HttpPost(ApiRoutes.Client.ConfirmOrderPayment)]
        public async Task<ActionResult> ConfirmOrderPayment(int orderId, int boutiqueId, string lang = "ar")
        {
            string userId = User.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;

            _ = await SendProviderNotifications(orderId, userId, boutiqueId);

            return Ok(new
            {
                key = 1,
                msg = CreatMessage(lang, "تم التأكيد بنجاح", "Confirmed successfully")
            });
        }


        [HttpPost(ApiRoutes.Client.GetOrder)]
        public async Task<ActionResult> GetOrder(GetOrderDTO model)
        {
            string userId = User.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;
            int branchId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == "branch_id")?.Value ?? "0");
            var settingApp = (await _settings.Entity.GetAsync(predicate: x => x.FK_BranchID == branchId));





            var order = (await _orders.Entity.GetAllAsync(predicate: x => x.FK_UserID == userId && x.ID == model.order_id,
                include: source => source.Include(i => i.FK_Provider).ThenInclude(i => i.SalonImages).Include(i => i.OrderServices).Include(s => s.FK_Provider).ThenInclude(i => i.FK_User),
                disableTracking: true))
            .Select(order => new OrderVM
            {
                id = order.ID,
                boutique_id = order.FK_ProviderID,
                provider_id = order.FK_Provider.FK_UserID,
                providerName = order.FK_Provider.FK_User.fullName,
                boutique_name = CreatMessage(model.lang, order.FK_Provider.nameAr, order.FK_Provider.nameEn),
                boutique_address = order.FK_Provider.address,
                boutique_img = order.FK_Provider.SalonImages.Select(x => BaseUrlHoste + x.img).FirstOrDefault() ?? "",
                rate = order.FK_Provider.rate,
                status = order.status,
                price = order.price,
                priceBeforDesc =order.priceBeforeDisc,
                Paied = order.Applicationpercentagepaid,
                pdf = order.pdf,
                applicationPercent = order.Applicationpercentage,
                applicationPercentImg = order.ApplicationpercentageImg != null ? BaseUrlHoste + order.ApplicationpercentageImg : "",
                refused_reason = order.refusedReason,
                payText = settingApp.payText,
                address = !String.IsNullOrEmpty(order.address) ? order.address : order.FK_Provider.address,
                lat = !String.IsNullOrEmpty(order.lat) ? order.lat : order.FK_Provider.lat,
                lng = !String.IsNullOrEmpty(order.lng) ? order.lng : order.FK_Provider.lng,
                orderDate = order.orderDate.ToString("dd/MM/yyyy"),
                orderTime = order.orderDate.ToString("hh:mm tt"),
                type = !String.IsNullOrEmpty(order.address) ? Helper.Helper.CreatMessage(model.lang, "بالمنزل", "Home") : Helper.Helper.CreatMessage(model.lang, "بالصالون", "Salon"),
                shippingValue = order.shippingPrice,
                isRate = order.rate != 0,
                //----------
                appCommission = order.AppCommission,
                deposit = order.Deposit,
                paymentId = order.PaymentId ?? "",
                returnMoney = order.returnMoney,
                //----------
                paidFromMobile = order.paid, // for branch 9 
                status_name = order.status switch
                {
                    0 => CreatMessage(model.lang, "جديد", "new"),
                    1 => CreatMessage(model.lang, "مقبول", "accepted"),
                    2 => CreatMessage(model.lang, "منتهى", "finished"),
                    3 => CreatMessage(model.lang, "مرفوض", "refused"),
                    4 => CreatMessage(model.lang, "ملغى", "canceled"),
                    _ => CreatMessage(model.lang, "خطأ", "error")
                },
                type_pay = order.typePay,

                date = order.date.ToString("dd/MM/yyyy HH:mm tt"),
                services = order.OrderServices.Select(x => new ServiceVM
                {
                    id = x.SubServiceID,
                    name = CreatMessage(model.lang, x.SubServicNameAr, x.SubServicNameEn),
                    main_service_name = CreatMessage(model.lang, x.mainServiceNameAr, x.mainServiceNameEn),
                    date = x.date.ToString("dd/MM/yyyy"),
                    time = x.date.ToString("hh:mm tt"),
                    price = x.price,
                    priceAtHome = x.priceAtHome,
                    duration = x.duration,
                    at_boutique = String.IsNullOrEmpty(x.address),
                    address = x.address,
                    lat = x.lat,
                    lng = x.lng,
                    employeeName = CreatMessage(model.lang, x.EmployeeNameAr, x.EmployeeNameEn),
                    employeeImg = Helper.Helper.BaseUrlHoste + x.EmployeeImg,
                    note = x.note,

                }).ToList()

            }).FirstOrDefault();

            //if (branchId == (int)BranchName.Eleklil)
            //{
            //    //order.price = order.priceBeforDesc;
            //    // order.shippingValue = order.services.Select(s => s.priceOfDelivery).Sum();

            //}

            return Ok(new
            {
                key = 1,
                data = order
            });



        }

        [HttpPost(ApiRoutes.Client.ListNotifications)]
        public async Task<ActionResult> ListNotifications(ListNotificationsDTO model)
        {
            string userId = User.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;

            var notifications = (await _notifications.Entity.GetAllAsync(predicate: x => x.FK_UserID == userId, include: source => source.Include(i => i.FK_Order))).Select(x => new
            {
                id = x.ID,
                msg = CreatMessage(model.lang, x.msgAr, x.msgEn),
                order_id = x.FK_OrderID,
                date = x.date.ToString("dd/MM/yyyy"),
                read = x.show,
                go_to_rate = x.FK_Order.status == (int)OrderStates.finished,

                // at Eleklil
                at_boutique = x.FK_Order != null ? String.IsNullOrEmpty(x.FK_Order.address) : false,
                paid = x.FK_Order != null ? x.FK_Order.paid : false,

            }).OrderByDescending(x => x.id).ToList();

            var readed = (await _notifications.Entity.GetAllAsync(predicate: x => x.show == false, disableTracking: false)).ToList();
            readed.ForEach(a => a.show = true);
            await _notifications.Save();


            return Ok(new
            {
                key = 1,
                data = notifications
            });
        }

        [HttpPost(ApiRoutes.Client.DeleteNotifications)]
        public async Task<ActionResult> DeleteNotifications(DeleteNotificationsDTO model)
        {
            string userId = User.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;

            var notifications = await _notifications.Entity.GetAllAsync();
            _notifications.Entity.DeleteRange(notifications.ToList());

            await _notifications.Save();


            return Ok(new
            {
                key = 1,
                msg = CreatMessage(model.lang, "تم حذف الاشعارات بنجاح", "notifictions deleted successfully")
            });
        }

        [AllowAnonymous]
        [HttpPost(ApiRoutes.Client.GetankAccounts)]
        public async Task<ActionResult> GetankAccounts(GetankAccountsDTO model)
        {
            string userId = User.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;
            int branchId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == "branch_id")?.Value ?? "0");

            var bankLists = await _bankAccounts.Entity.GetCustomAll(predicate: x => x.FK_BranchID == branchId && x.isActive).Select(x => new
            {
                id = x.ID,
                name = Helper.Helper.CreatMessage(model.lang, x.bankNameAr, x.bankNameEn),
                number = x.bankAccountNumber,
                owner = x.OwnerNameAr
            }).ToListAsync();


            return Ok(new
            {
                key = 1,
                data = bankLists
            });
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        private async Task<bool> SendProviderNotifications(int orderId, string userId, int boutiqueId)
        {
            int branchId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == "branch_id")?.Value ?? "0");

            var order = await _orders.Entity.FindByIdAsync(orderId);
            var user = await _users.Entity.FindByIdAsync(userId);
            var provider = await _providerAditionalData.Entity.FindByIdAsync(boutiqueId);

            await _notifications.Entity.InsertRangeAsync(new List<Notifications>() {
                                                            new Notifications(){
                                                                FK_OrderID = order.ID,
                                                                FK_UserID = userId,
                                                                msgAr=$"تم ارسال طلبك رقم {order.ID} بنجاح",
                                                                msgEn=$"your order number {order.ID} sent successfully",
                                                                show = false,
                                                                date = Helper.Helper.GetCurrentDate()
                                                            },
                                                            new Notifications(){
                                                                FK_OrderID = order.ID,
                                                                FK_UserID = provider.FK_UserID,
                                                                msgAr=$"لديك طلب جديد برقم {order.ID} ",
                                                                msgEn=$"you have new order number {order.ID}",
                                                                show = false,
                                                                date = Helper.Helper.GetCurrentDate()
                                                            }
                                                        });
            await _notifications.Save();

            var userDataAndIds = await _users.Entity.GetCustomAll(predicate: x => x.ProviderAditionalData.ID == boutiqueId).Select(x => new
            {
                lang = x.lang,
                ids = x.DeviceIds.Select(y => y.deviceID).ToList(),
                branchId = x.FK_BranchID,
            }).FirstOrDefaultAsync();

            var projectName = await _branches.Entity.GetCustomAll(x => x.ID == userDataAndIds.branchId).Select(x => x.nameAr + " | " + x.nameEn).FirstOrDefaultAsync();
            Helper.Helper.SendPushNotification(device_ids: userDataAndIds.ids, msg: Helper.Helper.CreatMessage(userDataAndIds.lang, $"لديك طلب جديد برقم {order.ID}", $"you have new order number {order.ID}"), order_id: order.ID, branchId: branchId, projectName: projectName);
            
            order.paid = true;

            await _orders.Save();
            return order.paid;
        }


        [HttpPost(ApiRoutes.Client.ChargeWallet)]
        public async Task<ActionResult> ChargeWallet(double price, string lang = "ar")
        {
            string userId = User.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;

            var user = await _users.Entity.FindByIdAsync(userId);
            user.userWallet = user.userWallet + price;
            _users.Entity.Update(user);
            await _users.Save();

            return Ok(new
            {
                key = 1,
                msg = CreatMessage(lang, "تم شحن المحفظة بنجاح", "Wallet charged successfully")
            });
        }


        [HttpPost(ApiRoutes.Client.GetUserWallet)]
        public ActionResult GetUserWallet()
        {
            string userId = User.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;

            var userWallet = _users.Entity.GetCustomAll(predicate: x => x.Id == userId).Select(u => u.userWallet).FirstOrDefault();

            return Ok(new
            {
                key = 1,
                data = userWallet
            });
        }

        [HttpPost(ApiRoutes.Client.GetPointBalanceForUser)]
        public async Task<ActionResult> GetPointBalanceForUser(string userId)
        {
           var user = await _users.Entity.GetAsync(x => x.Id == userId);
            if(user != null)
            {
                return Ok(new
                {
                    PointBalance = user.PointsBalance
                }) ;

            }
            else
            {
                return Ok(new
                {
                    msg = "the user not Exsit"
                });
            }
        }


        //[AllowAnonymous]
        //[HttpPost(ApiRoutes.Client.FillData)]
        //public async Task<ActionResult> FillbranchData(FillDataDTO model)
        //{
        //    var cities = _cities.Entity.GetCustomAll(predicate: city => city.FK_BranchID == model.fromBranch).Select(city =>new Cities)


        //    return Ok(new
        //    {
        //        key = 1,
        //        msg = "done"
        //    });
        //}




    }
}
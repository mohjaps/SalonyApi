using Core.Interfaces;
using Core.TableDb;
using Microsoft.AspNetCore.Mvc;
using Salony.ViewModels;
using System.Threading.Tasks;
using static Salony.Enums.AllEnums;
using static Salony.Helper.Helper;

namespace Salony.Controllers
{
    [AuthorizeRoles(Roles.Admin,Roles.SpacePrice)]
    public class ServiceDeliveryController : Controller
    {
        private readonly IUnitOfWork<ServiceDelivery> _serviceDeliveries;
        private readonly IUnitOfWork<MainServices> _service;
        private readonly IUnitOfWork<Branches> _branches;
        private readonly IUnitOfWork<Settings> _settings;

        public ServiceDeliveryController(
            IUnitOfWork<ServiceDelivery> serviceDeliveries,
            IUnitOfWork<MainServices> service,
            IUnitOfWork<Branches> branches,
            IUnitOfWork<Settings> settings)
        {
            _serviceDeliveries = serviceDeliveries;
            _service = service;
            _branches = branches;
            _settings = settings;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _serviceDeliveries.Entity.GetAllAsync());
        }
        public IActionResult CreateDeliveryService()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateDeliveryService(DeliveryServiceViewModel model)
        {
            await _serviceDeliveries.Entity.InsertAsync(new ServiceDelivery
            {
                FromInKM = model.From,
                ToInKM = model.To,
                DeliveryPrice = model.Price
            });
            await _serviceDeliveries.Save();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            ServiceDelivery serviceDelivery = await _serviceDeliveries.Entity.FindByIdAsync(id);
            return View(new ServiceDeliveryEditVM
            {
                Id = serviceDelivery.Id,
                From = serviceDelivery.FromInKM,
                To = serviceDelivery.ToInKM,
                Price = serviceDelivery.DeliveryPrice
            });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ServiceDeliveryEditVM model)
        {
            ServiceDelivery serviceDelivery = await _serviceDeliveries.Entity.FindByIdAsync(model.Id);
            serviceDelivery.FromInKM = model.From != 0 ? model.From : serviceDelivery.FromInKM;
            serviceDelivery.ToInKM = model.To != 0 ? model.To : serviceDelivery.ToInKM;
            serviceDelivery.DeliveryPrice = model.Price != 0 ? model.Price : serviceDelivery.DeliveryPrice;
            _serviceDeliveries.Entity.Update(serviceDelivery);
            await _serviceDeliveries.Save();
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> remove(int  id)
        {
            ServiceDelivery serviceDelivery = await _serviceDeliveries.Entity.FindByIdAsync(id);
            _serviceDeliveries.Entity.Delete(serviceDelivery);
            await _serviceDeliveries.Save();
            return Json(new { data = true });
        }

    }
}

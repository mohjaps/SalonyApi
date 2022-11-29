using Core.Interfaces;
using Core.TableDb;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static Salony.Enums.AllEnums;
using static Salony.Helper.Helper;

namespace Salony.Controllers
{
    [AuthorizeRoles(Roles.Admin, Roles.ContactUs)]
    public class DContactUsController : Controller
    {
        private readonly IUnitOfWork<ContactUs> _contactUs;
        private readonly IUnitOfWork<ApplicationDbUser> _users;


        public DContactUsController(IUnitOfWork<ContactUs> contactUs,IUnitOfWork<ApplicationDbUser> users)
        {
            _contactUs = contactUs;
            _users = users;
        }

        public async Task<IActionResult> Index()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userFk_BranchID = await _users.Entity.GetAsync(predicate: b => b.Id == userId);


            var ContactDB = (await _contactUs.Entity.GetAllAsync(predicate: x=>x.FK_BranchID == userFk_BranchID.FK_BranchID)).OrderByDescending(x => x.ID).ToList();
            return View(ContactDB);

        }
    }
}

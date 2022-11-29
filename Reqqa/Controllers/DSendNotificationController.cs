using Core.TableDb;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Salony.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static Salony.Helper.Helper;

namespace Salony.Controllers
{
    [AuthorizeRoles(AllEnums.Roles.Admin, AllEnums.Roles.Notifications)]

    public class DSendNotificationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DSendNotificationController(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userBranch = await _context.Users.Where(u => u.Id == userId).Select(u => u.FK_BranchID).FirstOrDefaultAsync();

            var applicationDbContext = _context.HistoryNotify.Where(n=>n.User.FK_BranchID == userBranch).Include(n => n.User);
            return View(await applicationDbContext.ToListAsync());
        }


        public IActionResult SendNotify()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployees()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userBranch = await _context.Users.Where(u => u.Id == userId).Select(u => u.FK_BranchID).FirstOrDefaultAsync();

            var users = await _context.Users.Where(u => u.FK_BranchID == userBranch&& u.isActive == true && u.typeUser == (int)AllEnums.TypeUser.client).Select(x => new
            {
                id = x.Id,
                name = x.fullName
            }).ToListAsync();

            return Ok(new { key = 1, users = users });
        }

        [HttpGet]
        public async Task<IActionResult> GetProviders()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userBranch = await _context.Users.Where(u => u.Id == userId).Select(u => u.FK_BranchID).FirstOrDefaultAsync();

            var delegets = await _context.Users.Where(u => u.FK_BranchID == userBranch && u.isActive == true && u.typeUser == (int)AllEnums.TypeUser.provider).Select(x => new
            {
                id = x.Id,
                name = x.fullName
            }).ToListAsync();

            return Ok(new { key = 1, delegets = delegets });
        }




        [HttpPost]
        public async Task<IActionResult> Send(string msg, string employees, string providers)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userBranch = await _context.Users.Where(u => u.Id == userId).Select(u => u.FK_BranchID).FirstOrDefaultAsync();
            var branchName = await _context.Branches.Where(u => u.ID == userBranch).Select(u => u.nameAr + " | " + u.nameEn).FirstOrDefaultAsync();

            if (employees != null)
            {
                if (employees.Length > 0)
                {
                    var employeeArr = employees.Split(',');
                    List<HistoryNotify> historyNotifies = new List<HistoryNotify>();
                    foreach (var clientId in employeeArr)
                    {
                        HistoryNotify notifyObj = new HistoryNotify()
                        {
                            Text = msg,
                            Date = Helper.Helper.GetCurrentDate(),
                            FKUser = clientId,
                        };
                        historyNotifies.Add(notifyObj);
                    }

                    _context.HistoryNotify.AddRange(historyNotifies);
                    await _context.SaveChangesAsync();
                    dynamic info = "";
                    List<string> user_devices = (from historyNotify in historyNotifies
                                        join deviceId in _context.DeviceIds on historyNotify.FKUser equals deviceId.FK_UserID
                                        select  deviceId.deviceID
                                        ).ToList();

                    Helper.Helper.SendPushNotification(device_ids: user_devices,msg: msg,type: (int)Enums.AllEnums.FcmType.dashboard,branchId: userBranch, projectName: branchName);
                }
            }
            if (providers != null)
            {
                if (providers.Length > 0)
                {
                    var providerArr = providers.Split(',');
                    List<HistoryNotify> historyNotifies = new List<HistoryNotify>();
                    foreach (var providerId in providerArr)
                    {
                        HistoryNotify notifyObj = new HistoryNotify()
                        {
                            Text = msg,
                            Date = Helper.Helper.GetCurrentDate(),
                            FKUser = providerId,
                        };
                        historyNotifies.Add(notifyObj);
                    }

                    _context.HistoryNotify.AddRange(historyNotifies);
                    await _context.SaveChangesAsync();
                    dynamic info = "";
                    var user_devices = (from historyNotify in historyNotifies
                                        join deviceId in _context.DeviceIds on historyNotify.FKUser equals deviceId.FK_UserID
                                        select deviceId.deviceID
                                        ).ToList();

                    Helper.Helper.SendPushNotification(device_ids: user_devices,msg: msg,type: (int)Enums.AllEnums.FcmType.dashboard,branchId:userBranch,projectName: branchName);
                }
            }

            return Ok(new { key = 1, message = "send successfully" });

        }



    }

}

using Core.Interfaces;
using Core.TableDb;
using Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Salony.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Salony.Controllers
{
    [Helper.Helper.AuthorizeRoles(Enums.AllEnums.Roles.Admin)]
    public class AdminController : Controller
    {


        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationDbUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork<ApplicationDbUser> _users;



        public AdminController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationDbUser> userManager, ApplicationDbContext context, IUnitOfWork<ApplicationDbUser> users)
        {
            _userManager = userManager;
            _context = context;
            _roleManager = roleManager;
            this._users = users;
        }

        //[Authorize(Roles = "Mobile,ادمن")]
        //public async Task<IActionResult> MobileUsers()
        //{
        //    var users = await _userManager.GetUsersInRoleAsync(Helper.Roles.Mobile.ToString());

        //    return View(users);

        //}

        [HttpPost]
        public async Task<IActionResult> ChangeState(string id)
        {

            var user = _context.Users.Find(id);
            if (user.isActive == true)
            {
                user.isActive = false;

            }
            else
            {
                user.isActive = true;
            }
            await _context.SaveChangesAsync();
            return Ok(new { key = 1, data = user.isActive });

        }




        /*************************************************************/
        #region UserInRoles

        public IActionResult UserInRoles()
        {

            ViewBag.User = User.Identity.Name;

            return View();
        }



        [HttpPost]
        public async Task<IActionResult> EditUsersInRoles([FromBody] UserRolesDto obj)
        {

            foreach (var userId in obj.users)
            {
                var user = await _userManager.FindByIdAsync(userId);
                var userRoles = await _userManager.GetRolesAsync(user);

                if (userRoles.Count == 0)
                {
                    foreach (var roleId in obj.roles)
                    {
                        var roleFound = await _roleManager.FindByIdAsync(roleId);
                        await _userManager.AddToRoleAsync(user, roleFound.Name);
                    }
                }
                else
                {
                    foreach (var userRole in userRoles)
                    {
                        if (userRole != Enums.AllEnums.Roles.SuperAdmin.ToString())
                        {
                            var userFound = await _userManager.FindByIdAsync(userId);
                            await _userManager.RemoveFromRoleAsync(userFound, userRole);
                        }
                    }

                    foreach (var roleId in obj.roles)
                    {
                        var roleFound = await _roleManager.FindByIdAsync(roleId);
                        await _userManager.AddToRoleAsync(user, roleFound.Name);
                    }
                }
            }

            return Json(new { key = 1 });
        }

        [HttpGet]
        public async Task<IActionResult> GetUsersWithRoles()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userData = await _users.Entity.GetAsync(predicate: b => b.Id == userId);
            
            var users = await _userManager.Users.Where(u => u.typeUser == (int)Enums.AllEnums.TypeUser.admin && u.FK_BranchID == userData.FK_BranchID).ToListAsync();
            var noMobileUsers = new List<ApplicationDbUser>();
            foreach (var user in users)
            {
                if (!await _userManager.IsInRoleAsync(user, Enums.AllEnums.Roles.Mobile.ToString()) && !await _userManager.IsInRoleAsync(user, Enums.AllEnums.Roles.SuperAdmin.ToString()))
                {
                    noMobileUsers.Add(user);
                }
            }

            var rolesList = new List<string>();
            var roles = "";


            foreach (var user in noMobileUsers)
            {

                var userRoles = await _userManager.GetRolesAsync(user);
                foreach (var userRole in userRoles)
                {
                    roles += Helper.Helper.GetRole(userRole, "ar") + " , ";
                }
                //roles = roles.Remove(roles.Length - 2);

                rolesList.Add(roles);
                roles = "";


            }

            return Json(new { users = noMobileUsers, roles = rolesList });
        }

        [HttpPost]
        public async Task<IActionResult> EditUserRoles([FromBody] UserIdDto userId)
        {
            var user = await _userManager.FindByIdAsync(userId.id);



            var userRoles = await _userManager.GetRolesAsync(user);

            var userRoleList = new List<IdentityRole>();

            foreach (var userRole in userRoles)
            {

                var roleExist = await _roleManager.RoleExistsAsync(userRole);
                if (roleExist)
                {
                    var role = _roleManager.Roles.FirstOrDefault(r => r.Name == userRole);
                    userRoleList.Add(role);
                }
            }





            return Json(new { user = user, userRoles = userRoleList });
        }


        [HttpGet]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _roleManager.Roles.Where(role => role.Name != Enums.AllEnums.Roles.Mobile.ToString() && role.Name != Enums.AllEnums.Roles.SuperAdmin.ToString()).Select(x => new { Id = x.Id, name = Helper.Helper.GetRole(x.Name, "ar") }).ToListAsync();
            if (roles.Count() > 0)
            {
                return Json(new { key = 1, roles = roles });
            }
            else
            {
                return Json(new { key = 0, msg = "لايوجد صلاحيات .." });
            }
        }



        [HttpGet]
        public async Task<IActionResult> GetAlluser()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUser = await _users.Entity.FindByIdAsync(userId);

            var users = await _userManager.Users.Where(u => u.typeUser == (int)Enums.AllEnums.TypeUser.admin && u.FK_BranchID == currentUser.FK_BranchID).ToListAsync();

            var noMobileUsers = new List<ApplicationDbUser>();

            foreach (var user in users)
            {
                if (!await _userManager.IsInRoleAsync(user, Enums.AllEnums.Roles.Mobile.ToString()) && !await _userManager.IsInRoleAsync(user, Enums.AllEnums.Roles.SuperAdmin.ToString()))
                {
                    noMobileUsers.Add(user);
                }
            }

            if (users.Count() > 0)
            {
                return Json(new { key = 1, users = noMobileUsers });
            }
            else
            {
                return Json(new { key = 0, msg = "لايوجد مستخدمين .." });
            }
        }


        #endregion


        /*************************************************************************************/

        #region CreateRole
        [HttpGet]
        public IActionResult CreateRole()
        {
            //  var x = User.Claims;
            ViewData["roles"] = _roleManager.Roles;
            ViewData["users"] = _userManager.Users;
            return View();
        }
        public IActionResult Adminstration()
        {
            return View();
        }

        public IActionResult UseRoles()
        {
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                // We just need to specify a unique role name to create a new role
                IdentityRole identityRole = new IdentityRole
                {
                    Name = model.RoleName
                };

                // Saves the role in the underlying AspNetRoles table
                IdentityResult result = await _roleManager.CreateAsync(identityRole);

                if (result.Succeeded)
                {
                    return RedirectToAction("CreateRole", "Adminstation");
                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }
        #endregion

        #region EditEole
        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            // Find the role by Role ID
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found";
                return View("NotFound");
            }

            var model = new EditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name
            };

            // Retrieve all the Users
            foreach (var user in _userManager.Users)
            {
                // If the user is in this role, add the username to
                // Users property of EditRoleViewModel. This model
                // object is then passed to the view for display
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }

            return View(model);
        }

        // This action responds to HttpPost and receives EditRoleViewModel
        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            var role = await _roleManager.FindByIdAsync(model.Id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {model.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                role.Name = model.RoleName;

                // Update the Role using UpdateAsync
                var result = await _roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("CreateRole", "Adminstation");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }
        }
        #endregion

        #region ControlUserInRole
        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            ViewBag.roleId = roleId;

            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }

            var model = new List<UserRoleViewModel>();

            foreach (var user in _userManager.Users)
            {
                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };

                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleViewModel.IsSelected = true;
                }
                else
                {
                    userRoleViewModel.IsSelected = false;
                }

                model.Add(userRoleViewModel);
            }

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model, string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }

            for (int i = 0; i < model.Count; i++)
            {
                var user = await _userManager.FindByIdAsync(model[i].UserId);

                IdentityResult result = null;

                if (model[i].IsSelected && !(await _userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await _userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!model[i].IsSelected && await _userManager.IsInRoleAsync(user, role.Name))
                {
                    result = await _userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }

                if (result.Succeeded)
                {
                    if (i < (model.Count - 1))
                        continue;
                    else
                        return RedirectToAction("EditRole", new { Id = roleId });
                }
            }

            return RedirectToAction("EditRole", new { Id = roleId });
        }
        #endregion
    }
}

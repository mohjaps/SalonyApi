using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Salony.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Core.TableDb;
using static Salony.Helper.Helper;
using System.Security.Claims;
using Core.Interfaces;

namespace Salony.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationDbUser> _signInManager;
        private readonly UserManager<ApplicationDbUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        //private readonly IEmailSender _emailSender;
        private readonly IHostingEnvironment _environment;
        private readonly IUnitOfWork<ApplicationDbUser> _users;


        public RegisterModel(
            UserManager<ApplicationDbUser> userManager,
            SignInManager<ApplicationDbUser> signInManager,
            ILogger<RegisterModel> logger,
            //IEmailSender emailSender,
            IHostingEnvironment environment,
             IUnitOfWork<ApplicationDbUser> users)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            //_emailSender = emailSender;
            _environment = environment;
            this._users = users;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "من فضلك ادخل البريد الالكتروني")]
            [EmailAddress(ErrorMessage = "يجب ادخال بريد الكتروني صحيح")]
            [Display(Name = "Email")]
            public string Email { get; set; }

            //[Required(ErrorMessage = "من فضلك ادخل اسم المستخدم")]
            //public string FullName { get; set; }

            [Required(ErrorMessage = "من فضلك ادخل صورة المستخدم")]
            [Display(Name = "Img")]
            public IFormFile Img { get; set; }

            public string PhotoPath { get; set; }

            [Required(ErrorMessage = "من فضلك ادخل كلمة المرور")]
            [StringLength(100, ErrorMessage = "يجب ان تزيد كلمة المرور عن 6 ارقام", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "يرجى التأكد من تطابق كلمتا المرور")]
            public string ConfirmPassword { get; set; }


        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        private string ProcessUploadedFile(IFormFile Photo, string Place)
        {
            string uniqueFileName = null;
            if (Photo != null)
            {
                string uploadsFolder = Path.Combine(_environment.WebRootPath, $"images/{Place}");
                uniqueFileName = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(Photo.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    Photo.CopyTo(fileStream);
                }
            }
            return "images/Users/" + uniqueFileName;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {

                string uniqueFileName = null;
                if (Input.Img != null)
                {
                    uniqueFileName = ProcessUploadedFile(Input.Img, "Users");
                }

                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var currentUser = await _users.Entity.FindByIdAsync(userId);

                var user = new ApplicationDbUser { UserName = Input.Email, Email = Input.Email, typeUser = (int)Enums.AllEnums.TypeUser.admin,FK_BranchID = currentUser.FK_BranchID, img = uniqueFileName ,fullName = Input.Email ,isActive = true,activeCode = true};

                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //var callbackUrl = Url.Page(
                    //    "/Account/ConfirmEmail",
                    //    pageHandler: null,
                    //    values: new { userId = user.Id, code = code },
                    //    protocol: Request.Scheme);

                    //await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    //await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                else
                {

                }
                foreach (var error in result.Errors)
                {
                    if (error.Description.Contains("already taken") && error.Description.Contains("User name"))
                    {
                        ModelState.AddModelError(string.Empty, "هذا الايميل مستخدم من قبل");
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}

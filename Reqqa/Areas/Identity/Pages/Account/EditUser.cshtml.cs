using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Salony.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Core.TableDb;
using static Salony.Helper.Helper;
using Infrastructure;

namespace Salony.Areas.Identity.Pages.Account
{
    public class EditUserModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<ApplicationDbUser> _signInManager;
        private readonly UserManager<ApplicationDbUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        //private readonly IEmailSender _emailSender;
        private readonly IHostingEnvironment _environment;

        public EditUserModel(SignInManager<ApplicationDbUser> signInManager, UserManager<ApplicationDbUser> userManager, ILogger<RegisterModel> logger, IHostingEnvironment environment, ApplicationDbContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _environment = environment;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {

            [Required(ErrorMessage = "من فضلك ادخل البريد الالكترونى")]
            [EmailAddress(ErrorMessage = "يجب ادخال بريد الكترونى صحيح")]
            [Display(Name = "Email")]
            public string Email { get; set; }
            public string Id { get; set; }
            [Display(Name = "Img")]
            public IFormFile Img { get; set; }

            public string PhotoPath { get; set; }

            //[Required(ErrorMessage = "من فضلك ادخل  العنوان")]
            //[Display(Name = "Address")]
            //public string Address { get; set; }

            public string FullName { get; set; }

        }

        public IActionResult OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {



                var userEmail = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == Input.Email && u.Id != Input.Id);
                var foundedUser = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == Input.Id);

                if (userEmail != null)
                {
                    ModelState.AddModelError("Email", "البريد الالكتروني مسجل من قبل");
                }



                string uniqueFileName = null;


                if (Input.Img != null)
                {
                    string uploadFolder = Path.Combine(_environment.WebRootPath, "Images\\Users");
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + Input.Img.FileName;
                    string filePath = Path.Combine(uploadFolder, uniqueFileName);

                    Input.Img.CopyTo(new FileStream(filePath, FileMode.Create));

                    Input.PhotoPath = uniqueFileName;

                    foundedUser.img = "Images/Users/" + uniqueFileName;
                }

                else
                {
                    foundedUser.img = foundedUser.img;
                }

                foundedUser.Email = Input.Email;
                foundedUser.UserName = Input.Email;


                var result = await _userManager.UpdateAsync(foundedUser);

                await _context.SaveChangesAsync();

                if (result.Succeeded)
                {
                    return LocalRedirect("/Home/Index");


                }




            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
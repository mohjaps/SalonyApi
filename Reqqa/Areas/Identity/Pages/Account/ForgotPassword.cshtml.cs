using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Core.TableDb;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Salony.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationDbUser> _userManager;
        //private readonly IEmailSender _emailSender;

        public ForgotPasswordModel(UserManager<ApplicationDbUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "من فضلك ادخل البريد الالكتروني")]
            [EmailAddress(ErrorMessage = "من فضلك ادخل بريد الكتروني صحيح")]
            [Display(Name = "البريد الالكتروني")]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Input.Email != null)
            {
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == Input.Email);
                if (user == null)
                {
                    ModelState.AddModelError("Email","يرجى التأكد من البريد الالكتروني");
                    //return Page();
                }

            }


            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null /*|| !(await _userManager.IsEmailConfirmedAsync(user))*/)
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please 
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { code },
                    protocol: Request.Scheme);

                //await _emailSender.SendEmailAsync(
                //    Input.Email,
                //    "Reset Password",
                //    $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                try
                {
                    SendMailToCompany("", callbackUrl, Input.Email);
                }
                catch (Exception)
                {

                }
                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }

        public void SendMailToCompany(string head, string body, string Email)
        {
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential("awamercompany22@gmail.com", "Awamer123456@#");


            using (var message = new MailMessage("awamercompany22@gmail.com", Email))
            {
                message.Subject = "تغيير كلمة المرور";
                message.Body = body;
                message.IsBodyHtml = true;
                smtp.Send(message);
            }
        }
    }
}

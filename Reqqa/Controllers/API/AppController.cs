using Core.Interfaces;
using Core.TableDb;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Salony.Models.ApiDTO.App;
using Salony.Models.ApiDTO.Client;
using Salony.PathUrl;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static Salony.Enums.AllEnums;
using static Salony.Helper.Helper;

namespace Salony.Controllers.API
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiExplorerSettings(GroupName = "MobileApi")]
    public class AppController : Controller
    {
        private readonly IWebHostEnvironment _HostingEnvironment;
        private readonly IUnitOfWork<Settings> _settings;

        private readonly IUnitOfWork<ContactUs> _contactUs;


        public AppController(IWebHostEnvironment HostingEnvironment, IUnitOfWork<Settings> settings, IUnitOfWork<ContactUs> contactUs)
        {
            this._HostingEnvironment = HostingEnvironment;
            this._settings = settings;
            this._contactUs = contactUs;
        }


        [AllowAnonymous]
        [HttpPost(ApiRoutes.App.AboutApp)]
        public async Task<ActionResult> AboutApp(AboutAppDTO model)
        {
            //string userId = User.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;

            //int[] nums = new int[] { 0, 1, 0, 3, 12 };
            //int[] nums2 = new int[nums.Length];
            //int zeros = 0;
            //for (int i = 0; i < nums.Length; i++)
            //{
            //    if (nums[i] != 0)
            //    {
            //        nums2[i - zeros] = nums[i] ;
            //    }
            //    else
            //    {
            //        zeros++;
            //    }
            //}
            //if (zeros != 0)
            //{
            //    for (int i = 0; i < zeros; i++)
            //    {
            //        nums2[nums2.Length - zeros] = 0;
            //        zeros--;
            //    }
            //}
            //for (int i = 0; i < nums2.Length; i++)
            //{
            //    nums[i] = nums2[i];
            //}    



            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return Ok(new
                {
                    key = 0,
                    allErrors,
                });
            }

            Settings settings = (await _settings.Entity.GetAsync(x=>x.FK_BranchID == model.branch_id))??new Settings { };


            var data = new
            {
                phone = settings.phone,
                phone2 = settings.phone2,
                facebook = settings.facebook,
                twitter = settings.twitter,
                telegram = settings.telegram,
                instagram = settings.instagram,
                whatsApp = settings.whatsApp,
                snapChat = settings.snapChat,
                youtube = settings.youtube,
                commercialRegister = settings.commercialRegister,

                tax = settings.Tax,

                google_play_url = settings.googlePlayUrl,
                apple_store_url = settings.appleStoreUrl,
                key_map = settings.keyMap,

                about_app = model.lang == "ar" ? settings.aboutAppAr : settings.aboutAppEn,
                condetions = model.lang == "ar" ? settings.condtionsAr : settings.condtionsEn,
                paymentPolicy = model.lang == "ar" ? settings.paymentPolicyAr : settings.paymentPolicyEn,

                screen1Title = Helper.Helper.CreatMessage(model.lang,settings.Screen1TitleAr,settings.Screen1TitleEn),
                screen2Title = Helper.Helper.CreatMessage(model.lang,settings.Screen2TitleAr,settings.Screen2TitleEn),
                screen3Title = Helper.Helper.CreatMessage(model.lang,settings.Screen3TitleAr,settings.Screen3TitleEn),
                screen1Description = Helper.Helper.CreatMessage(model.lang,settings.Screen1DescriptionAr,settings.Screen1DescriptionEn),
                screen2Description = Helper.Helper.CreatMessage(model.lang,settings.Screen2DescriptionAr,settings.Screen2DescriptionEn),
                screen3Description = Helper.Helper.CreatMessage(model.lang,settings.Screen3DescriptionAr,settings.Screen3DescriptionEn),
            };



            return Json(new
            {
                key = 1,
                data
            });
        }


        [AllowAnonymous]
        [HttpPost(ApiRoutes.App.ContactUs)]
        public async Task<ActionResult> ContactUs(ContactUsDTO model)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return Ok(new
                {
                    key = 0,
                    allErrors,
                });
            }

            ContactUs contactUs = new ContactUs()
            {
                comment = model.msg,
                date = GetCurrentDate(),
                Email = model.email,
                Name = model.name,
                Phone = model.phone,
                FK_BranchID = model.branch_id
            };
            await _contactUs.Entity.InsertAsync(contactUs);
            await _contactUs.Save();

            return Ok(new
            {
                key = 1,
                msg = model.lang == "ar" ? "تم ارسال رسالتك بنجاح" : "your message sent successfully"
            });

        }


    }
}
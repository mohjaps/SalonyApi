using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Core.Interfaces;
using Core.TableDb;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Salony.Models.ControllerDTO;
using static Salony.Enums.AllEnums;
using static Salony.Helper.Helper;

namespace Salony.Controllers
{
    [AuthorizeRoles(Roles.Admin, Roles.Settings)]
    public class DSettingsController : Controller
    {
        private readonly UserManager<ApplicationDbUser> _userManager;
        private readonly IUnitOfWork<ApplicationDbUser> _users;
        private readonly IUnitOfWork<Settings> _settings;

        public DSettingsController(UserManager<ApplicationDbUser> userManager, IUnitOfWork<ApplicationDbUser> users, IUnitOfWork<Settings> settings)
        {
            this._userManager = userManager;
            this._users = users;
            this._settings = settings;
        }


        public async Task<IActionResult> Edit()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userFk_BranchID = await _users.Entity.GetAsync(predicate: b => b.Id == userId);
            var settings = await _settings.Entity.GetAsync(predicate: b => b.FK_BranchID == userFk_BranchID.FK_BranchID);
            if (settings == null)
            {
                Settings setting = new Settings() {
                    FK_BranchID = userFk_BranchID.FK_BranchID
                };
                await _settings.Entity.InsertAsync(setting);
                await _settings.Save();

                SettingsDTO returnedSettings = new SettingsDTO() {
                    ID = setting.ID,
                    FK_BranchID = setting.FK_BranchID
                };
                return View(returnedSettings);

            }

            SettingsDTO returnedSettingsout = new SettingsDTO()
            {
                ID = settings.ID,
                FK_BranchID = settings.FK_BranchID,
                aboutAppAr = settings.aboutAppAr,
                aboutAppEn = settings.aboutAppEn,
                appleStoreUrl = settings.appleStoreUrl,
                condtionsAr = settings.condtionsAr,
                condtionsEn = settings.condtionsEn,
                facebook = settings.facebook,
                googlePlayUrl = settings.googlePlayUrl,
                instagram = settings.instagram,
                keyMap = settings.keyMap,
                phone = settings.phone,
                phone2 = settings.phone2,
                telegram = settings.telegram,
                twitter = settings.twitter,
                whatsApp = settings.whatsApp,
                snapChat = settings.snapChat,
                youtube = settings.youtube,
                commercialRegister = settings.commercialRegister,
                patText = settings.payText,
                appPrecent = settings.appPrecent,
                appPrecentPercentage = settings.appPrecentPercentage,
                ExpireTime = settings.ExpireTime,
                Screen1TitleAr = settings.Screen1TitleAr,
                Screen1TitleEn = settings.Screen1TitleEn,
                Screen2TitleAr = settings.Screen2TitleAr,
                Screen2TitleEn = settings.Screen2TitleEn,
                Screen3TitleAr = settings.Screen3TitleAr,
                Screen3TitleEn = settings.Screen3TitleEn,
                Screen1DescriptionAr = settings.Screen1DescriptionAr,
                Screen1DescriptionEn = settings.Screen1DescriptionEn,
                Screen2DescriptionAr = settings.Screen2DescriptionAr,
                Screen2DescriptionEn = settings.Screen2DescriptionEn,
                Screen3DescriptionAr = settings.Screen3DescriptionAr,
                Screen3DescriptionEn = settings.Screen3DescriptionEn,
                tax = settings.Tax,
                taxOfHome = settings.TaxOfHome,
                invitationCodeBallance = settings.invitationCodeBallance
            };

            return View(returnedSettingsout);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(SettingsDTO model)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userFk_BranchID = await _users.Entity.GetAsync(predicate: b => b.Id == userId);


            var SettingDB = await _settings.Entity.GetAsync(predicate: b => b.FK_BranchID == userFk_BranchID.FK_BranchID);
            if (ModelState.IsValid)
            {
                SettingDB.appleStoreUrl = model.appleStoreUrl;
                SettingDB.facebook = model.facebook;
                SettingDB.instagram = model.instagram;
                SettingDB.telegram = model.telegram;
                SettingDB.whatsApp = model.whatsApp;
                SettingDB.snapChat = model.snapChat;
                SettingDB.phone = model.phone;
                SettingDB.phone2 = model.phone2;
                SettingDB.youtube = model.youtube;
                SettingDB.keyMap = model.keyMap;
                SettingDB.googlePlayUrl = model.googlePlayUrl;
                SettingDB.keyMap = model.keyMap;
                SettingDB.twitter = model.twitter;
                SettingDB.commercialRegister = model.commercialRegister;
                SettingDB.appPrecent = 10;
                SettingDB.payText = model.patText;
                SettingDB.appPrecent = model.appPrecent;
                SettingDB.appPrecentPercentage = model.appPrecentPercentage;
                SettingDB.ExpireTime = model.ExpireTime;

                SettingDB.Tax = model.tax;
                SettingDB.TaxOfHome = model.taxOfHome;

                SettingDB.Screen1TitleAr = model.Screen1TitleAr;
                SettingDB.Screen1TitleEn = model.Screen1TitleEn;
                SettingDB.Screen2TitleAr = model.Screen2TitleAr;
                SettingDB.Screen2TitleEn = model.Screen2TitleEn;
                SettingDB.Screen3TitleAr = model.Screen3TitleAr;
                SettingDB.Screen3TitleEn = model.Screen3TitleEn;
                SettingDB.Screen1DescriptionAr = model.Screen1DescriptionAr;
                SettingDB.Screen1DescriptionEn = model.Screen1DescriptionEn;
                SettingDB.Screen2DescriptionAr = model.Screen2DescriptionAr;
                SettingDB.Screen2DescriptionEn = model.Screen2DescriptionEn;
                SettingDB.Screen3DescriptionAr = model.Screen3DescriptionAr;
                SettingDB.Screen3DescriptionEn = model.Screen3DescriptionEn;

                SettingDB.invitationCodeBallance = model.invitationCodeBallance;


                _settings.Entity.Update(SettingDB);
                await _settings.Save();
                ViewBag.Msg = "تم الحفظ بنجاح";

                model.ID = SettingDB.ID;
                model.FK_BranchID = SettingDB.FK_BranchID;

                return View(model);
            }

            return View(model);

        }


        public async Task<IActionResult> Edit2()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userFk_BranchID = await _users.Entity.GetAsync(predicate: b => b.Id == userId);

            var settings = (await _settings.Entity.GetAsync(x=>x.FK_BranchID == userFk_BranchID.FK_BranchID)) ?? new Settings() { }; ;
            if (settings.ID == 0)
            {
                await _settings.Entity.InsertAsync(settings);
                await _settings.Save();
            }
            SettingsDTO settingViewModel = new SettingsDTO()
            {
                aboutAppAr = settings.aboutAppAr,
                aboutAppEn = settings.aboutAppEn,
                condtionsAr = settings.condtionsAr,
                condtionsEn = settings.condtionsEn,
                paymentPolicyAr = settings.paymentPolicyAr,
                paymentPolicyEn = settings.paymentPolicyEn
            };

            return View(settingViewModel);
        }


        [HttpPost]
        public async Task<IActionResult> Edit2([FromBody] SettingsDTO settingViewModel)
        {

                try
                {

                if (String.IsNullOrEmpty(settingViewModel.aboutAppAr) ||String.IsNullOrEmpty(settingViewModel.aboutAppEn) ||String.IsNullOrEmpty(settingViewModel.condtionsAr) ||String.IsNullOrEmpty(settingViewModel.condtionsAr))
                {
                    return Ok(new
                    {
                        key = 0
                    });
                }

                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var userFk_BranchID = await _users.Entity.GetAsync(predicate: b => b.Id == userId);

                    var foundedSetting = (await _settings.Entity.GetAsync(x => x.FK_BranchID == userFk_BranchID.FK_BranchID)) ?? new Settings() { };

                    foundedSetting.aboutAppAr = settingViewModel.aboutAppAr;
                    foundedSetting.aboutAppEn = settingViewModel.aboutAppEn;
                    foundedSetting.condtionsAr = settingViewModel.condtionsAr;
                    foundedSetting.condtionsEn = settingViewModel.condtionsEn;
                    foundedSetting.paymentPolicyAr = settingViewModel.paymentPolicyAr;
                    foundedSetting.paymentPolicyEn = settingViewModel.paymentPolicyEn;

                    _settings.Entity.Update(foundedSetting);
                    await _settings.Save();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await SettingsExists(settingViewModel.ID))
                    {
                        return Ok(new
                        {
                            key = 0
                        });
                    }
                    else
                    {
                        return Ok(new
                        {
                            key = 0
                        });
                    }
                }
                return Ok(new
                {
                    key = 1
                });
        }

        private async Task<bool> SettingsExists(int id)
        {
            return (await _settings.Entity.GetAllAsync(e => e.ID == id)).Any();
        }


    }
}

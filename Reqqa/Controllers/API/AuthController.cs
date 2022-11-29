using Core.Interfaces;
using Core.TableDb;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Nancy.ViewEngines;
using Salony.Models.ApiDTO;
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
    public class AuthController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _HostingEnvironment;
        private readonly UserManager<ApplicationDbUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOfWork<ApplicationDbUser> _users;
        private readonly IUnitOfWork<Districts> _districts;
        private readonly IUnitOfWork<Categories> _categories;
        private readonly IUnitOfWork<DeviceIds> _deviceIds;
        private readonly IUnitOfWork<Orders> _orders;
        private readonly IUnitOfWork<Settings> _settings;
        private readonly IUnitOfWork<ProviderAditionalData> _providerAdditionData;
        private static int _counter;


        public AuthController(IWebHostEnvironment HostingEnvironment, IConfiguration configuration, UserManager<ApplicationDbUser> userManager, RoleManager<IdentityRole> roleManager,
            IUnitOfWork<ApplicationDbUser> users, IUnitOfWork<Districts> districts, IUnitOfWork<Categories> categories, IUnitOfWork<DeviceIds> deviceIds, IUnitOfWork<Orders> orders, IUnitOfWork<Settings> settings, IUnitOfWork<ProviderAditionalData> providerAdditionData)
        {
            this._HostingEnvironment = HostingEnvironment;
            this._userManager = userManager;
            this._roleManager = roleManager;
            this._configuration = configuration;
            this._users = users;
            this._districts = districts;
            this._categories = categories;
            this._deviceIds = deviceIds;
            _orders = orders;
            _settings = settings;
            _providerAdditionData = providerAdditionData;
        }


        #region MainInfo

        [AllowAnonymous]
        [HttpPost(ApiRoutes.Identity.RegisterClient)]
        public async Task<ActionResult> RegisterClient(RegisterClientDTO model)
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

            #region validation
            //name

            if (model.user_name == null)
            {
                return Ok(new
                {
                    key = 0,
                    msg = CreatMessage(model.lang, "من فضلك ادخل الاسم", "Please enter your Name")
                });
            }

            var UserName = await _users.Entity.GetAsync(x => x.fullName == model.user_name && x.typeUser == (int)TypeUser.client && x.FK_BranchID == model.branch_id);
            if (UserName != null)
            {
                return Ok(new
                {
                    key = 0,
                    msg = CreatMessage(model.lang, "عذرا هذا الاسم موجود بالفعل", "Sorry this name is already present")
                });
            }
            if (model.phone == null)
            {
                return Ok(new
                {
                    key = 0,
                    msg = CreatMessage(model.lang, "من فضلك ادخل رقم الجوال", "Please enter your phone number")
                });
            }

            var phone = await _users.Entity.GetAsync(x => x.PhoneNumber == model.phone && x.typeUser == (int)TypeUser.client && x.FK_BranchID == model.branch_id);
            if (phone != null)
            {
                return Ok(new
                {
                    key = 0,
                    msg = CreatMessage(model.lang, "عذرا هذا الجوال موجود بالفعل", "Sorry this mobile is already present")
                });
            }

            //Password
            if (model.password == null)
            {
                return Ok(new
                {
                    key = 0,
                    msg = CreatMessage(model.lang, "من فضلك ادخل  كلمة المرور", "Please enter your password")
                });
            }


            #endregion

            // generate invitation Code  >> ToGo
            string invCode = "";
            

            var user = new ApplicationDbUser
            {
                Email = model.email ?? "",
                UserName = model.branch_id.ToString() + ((int)TypeUser.client).ToString() + model.phone,
                PhoneNumber = model.phone,
                fullName = model.user_name,
                showPassword = model.password,
                img = "Images/Users/Default.png",
                isActive = true,
                activeCode = false,
                closeNotification = false,
                registerDate = GetCurrentDate(),
                sendCodeDate = GetCurrentDate(),
                code = GetFormNumber(model.branch_id),
                lang = model.lang,
                typeUser = (int)TypeUser.client,
                FK_BranchID = model.branch_id,
                address = model.address,
                lat = model.lat,
                lng = model.lng,
                invitationCode = invCode
            };



            var result = await _userManager.CreateAsync(user, model.password);
            if (result.Succeeded)
            {

                

                if (!await _roleManager.RoleExistsAsync(Enums.AllEnums.Roles.Mobile.ToString()))
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName: Enums.AllEnums.Roles.Mobile.ToString()));
                }

                await _userManager.AddToRoleAsync(user, Enums.AllEnums.Roles.Mobile.ToString());
            }
            else
            {
                return Ok(new
                {
                    key = 0,
                    msg = CreatMessage(model.lang, result.ToString(), result.ToString())
                });
            }

            await SendMessage(user.code.ToString(), user.PhoneNumber, branch: model.branch_id);

            return Ok(new
            {
                key = 1,
                data = GetUserInfo(user),
                msg = CreatMessage(model.lang, "تم التسجيل بنجاح", "successfully registered"),
                status = false,
                user.code
            });
        }

        [AllowAnonymous]
        [HttpPost(ApiRoutes.Identity.RegisterProvider)]
        public async Task<ActionResult> RegisterProvider(RegisterProviderDTO model)
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

            #region validation
            //name

            if (model.user_name == null)
            {
                return Ok(new
                {
                    key = 0,
                    msg = CreatMessage(model.lang, "من فضلك ادخل الاسم", "Please enter your Name")
                });
            }

            var UserName = await _users.Entity.GetAsync(x => x.fullName == model.user_name && x.typeUser == (int)TypeUser.provider && x.FK_BranchID == model.branch_id);
            if (UserName != null)
            {
                return Ok(new
                {
                    key = 0,
                    msg = CreatMessage(model.lang, "عذرا هذا الاسم موجود بالفعل", "Sorry this name is already present")
                });
            }
            if (model.phone == null)
            {
                return Ok(new
                {
                    key = 0,
                    msg = CreatMessage(model.lang, "من فضلك ادخل رقم الجوال", "Please enter your phone number")
                });
            }

            var phone = await _users.Entity.GetAsync(x => x.PhoneNumber == model.phone && x.typeUser == (int)TypeUser.provider && x.FK_BranchID == model.branch_id);
            if (phone != null)
            {
                return Ok(new
                {
                    key = 0,
                    msg = CreatMessage(model.lang, "عذرا هذا الجوال موجود بالفعل", "Sorry this mobile is already present")
                });
            }

            if (!String.IsNullOrEmpty(model.email))
            {
                var email = await _users.Entity.GetAsync(x => x.Email == model.email && x.typeUser == (int)TypeUser.provider && x.FK_BranchID == model.branch_id);
                if (email != null)
                {
                    return Ok(new
                    {
                        key = 0,
                        msg = CreatMessage(model.lang, "عذرا هذا الايميل موجود بالفعل", "Sorry this email is already present")
                    });
                }
            }

            //Password
            if (model.password == null)
            {
                return Ok(new
                {
                    key = 0,
                    msg = CreatMessage(model.lang, "من فضلك ادخل  كلمة المرور", "Please enter your password")
                });
            }
            var district = await _districts.Entity.FindByIdAsync(model.district_id);

            if (district == null)
            {
                return Ok(new
                {
                    key = 0,
                    msg = CreatMessage(model.lang, "من فضلك ادخل  الحى", "Please enter your district")
                });
            }

            var category = await _categories.Entity.FindByIdAsync(model.category);

            if (category == null)
            {
                return Ok(new
                {
                    key = 0,
                    msg = CreatMessage(model.lang, "من فضلك ادخل  القسم", "Please enter your category")
                });
            }


            #endregion

            var user = new ApplicationDbUser
            {
                Email = model.email ?? model.phone + "@gmail.com",
                UserName = model.branch_id.ToString() + ((int)TypeUser.provider).ToString() + model.phone,
                PhoneNumber = model.phone,
                fullName = model.user_name,
                showPassword = model.password,
                img = ProcessUploadedFile(_HostingEnvironment, model.imgs[0], "Users"),
                iDPhoto = ProcessUploadedFile(_HostingEnvironment, model.iDPhoto, "Users"),
                certificatePhoto = ProcessUploadedFile(_HostingEnvironment, model.certificatePhoto, "Users"),
                ibanNumber = model.ibanNumber?.Trim(),
                isActive = true,
                //IsAvailable = model.branch_id == 6 ? false : true,
                IsAvailable = true,
                activeCode = false,
                isDeleted = false,
                closeNotification = false,
                registerDate = GetCurrentDate(),
                sendCodeDate = GetCurrentDate(),
                code = GetFormNumber(model.branch_id),
                lang = model.lang,
                typeUser = (int)TypeUser.provider,
                FK_BranchID = model.branch_id,
                ProviderAditionalData = new ProviderAditionalData()
                {
                    nameAr = model.boutique_name_ar,
                    nameEn = model.boutique_name_en,
                    commercialRegister = model.commercial_register_number,
                    FK_DistrictID = model.district_id,
                    address = model.address,
                    lat = model.lat,
                    lng = model.lng,
                    timeForm = model.start_date,
                    timeTo = model.end_date,
                    dayWorks = !String.IsNullOrEmpty(model.days) ? model.days : "0,1,2,3,4,5,6",
                    salonType = model.salonType,
                    SalonUsersType = model.SalonUsersType,
                    descriptionAr = model.description_ar,
                    descriptionEn = model.description_en,
                    FK_CategoryID = model.category,
                    lastPayDate = GetCurrentDate(),
                    bankAccount = model.bankAccount,
                    BankName = model.bankName,
                    socialMediaProfile = model.socialMediaProfile,
                    paied = 0,

                    IdentityImage = ProcessUploadedFile(_HostingEnvironment, model.identityImage, "Users"),
                    CommercialRegisterImage = ProcessUploadedFile(_HostingEnvironment, model.commercialRegisterImage, "Users"),
                    HealthCardImage = ProcessUploadedFile(_HostingEnvironment, model.healthCardImage, "Users"),
                    IbanNumber = model.ibanNumber,
                    IbanImage = ProcessUploadedFile(_HostingEnvironment, model.ibanImage, "Users"),

                    IdNumber = model.idNumber

                }
            };

            foreach (var item in model.imgs)
            {
                user.ProviderAditionalData.SalonImages.Add(new SalonImages()
                {
                    img = ProcessUploadedFile(_HostingEnvironment, item, "SalonImages")
                });
            }

            var result = await _userManager.CreateAsync(user, model.password);
            if (result.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync(Enums.AllEnums.Roles.Mobile.ToString()))
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName: Enums.AllEnums.Roles.Mobile.ToString()));
                }

                await _userManager.AddToRoleAsync(user, Enums.AllEnums.Roles.Mobile.ToString());
            }
            else
            {
                return Ok(new
                {
                    key = 0,
                    msg = CreatMessage(model.lang, result.ToString(), result.ToString())
                });
            }

            await SendMessage(user.code.ToString(), user.PhoneNumber, branch: model.branch_id);

            var codeuser = await _users.Entity.GetAsync(predicate: x => x.Id == user.Id, include: source => source.Include(i => i.ProviderAditionalData).ThenInclude(i => i.FK_District).Include(i => i.ProviderAditionalData).ThenInclude(i => i.FK_Category));

            return Ok(new
            {
                key = 1,
                data = GetUserInfo(codeuser),
                msg = CreatMessage(model.lang, "تم التسجيل بنجاح", "successfully registered"),
                status = false,
                user.code
            });
        }


        [AllowAnonymous]
        [HttpPost(ApiRoutes.Identity.ConfirmCodeRegister)]
        public async Task<ActionResult> ConfirmCodeRegister(ConfirmCodeRegisterDTO model)
        {
            try
            {
                if (model.code == null)
                {
                    return Ok(new
                    {
                        key = 0,
                        msg = CreatMessage(model.lang, "من فضلك ادخل كود التحقق", "Please enter your verification code")
                    });
                }

                var codeuser = await _users.Entity.GetAsync(predicate: x => x.Id == model.user_id, include: source => source.Include(i => i.ProviderAditionalData).ThenInclude(i => i.FK_District).Include(i => i.ProviderAditionalData).ThenInclude(i => i.FK_Category));

                if (codeuser != null)
                {
                    if (codeuser.code == model.code)
                    {
                        codeuser.activeCode = true;
                        _users.Entity.Update(codeuser);
                        await _users.Save();

                        return Ok(new
                        {
                            key = 1,
                            data = GetUserInfo(codeuser),
                            msg = CreatMessage(model.lang, "تم تفعيل الدخول بنجاح", "code has been successfully activated")
                        });
                    }
                    else
                    {
                        return Ok(new { key = 0, msg = CreatMessage(model.lang, "كود التفعيل غير صحيح ", "Please enter the code correctly") });
                    }
                }
                else
                {
                    return Ok(new { key = 0, msg = CreatMessage(model.lang, "عذرا هذا المستخدم غير مسجل لدينا", "Sorry this phone is not registered") });

                }

            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    key = 0,
                    msg = ex.Message
                });
            }

        }



        [AllowAnonymous]
        [HttpPost(ApiRoutes.Identity.ConfirmCodeRegisterMashglShow)]
        public async Task<ActionResult> ConfirmCodeRegisterMashglShow(ConfirmCodeRegisterDTO model)
        {
            try
            {
                if (model.code == null)
                {
                    return Ok(new
                    {
                        key = 0,
                        msg = CreatMessage(model.lang, "من فضلك ادخل كود التحقق", "Please enter your verification code")
                    });
                }

                var codeuser = await _users.Entity.GetAsync(predicate: x => x.Id == model.user_id, include: source => source.Include(i => i.ProviderAditionalData).ThenInclude(i => i.FK_District).Include(i => i.ProviderAditionalData).ThenInclude(i => i.FK_Category));


                if (codeuser != null)
                {
                    if (codeuser.code == model.code)
                    {
                        codeuser.activeCode = true;
                        _users.Entity.Update(codeuser);
                        await _users.Save();



                        var claim = new[] {
                    new Claim(JwtRegisteredClaimNames.Sub, codeuser.UserName),
                    new Claim("user_id", codeuser.Id),
                    new Claim("branch_id", codeuser.FK_BranchID.ToString())
                };
                        var signinKey = new SymmetricSecurityKey(
                      Encoding.UTF8.GetBytes(_configuration["Jwt:SigningKey"]));

                        int expiryInMinutes = Convert.ToInt32(_configuration["Jwt:ExpiryInMinutes"]);

                        var token = new JwtSecurityToken(
                             issuer: _configuration["Jwt:Site"],
                             audience: _configuration["Jwt:Site"],
                             claims: claim,
                             expires: DateTime.UtcNow.AddMinutes(expiryInMinutes),
                             signingCredentials: new SigningCredentials(signinKey, SecurityAlgorithms.HmacSha256)
                           );

                        return Ok(new
                        {
                            key = 1,
                            data = GetUserInfo(codeuser),
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo,
                            status = 1,
                            msg = CreatMessage(model.lang, "تم تفعيل الدخول بنجاح", "code has been successfully activated")
                        });
                    }
                    else
                    {
                        return Ok(new { key = 0, msg = CreatMessage(model.lang, "كود التفعيل غير صحيح ", "Please enter the code correctly") });
                    }
                }
                else
                {
                    return Ok(new { key = 0, msg = CreatMessage(model.lang, "عذرا هذا المستخدم غير مسجل لدينا", "Sorry this phone is not registered") });

                }

            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    key = 0,
                    msg = ex.Message
                });
            }

        }


        [AllowAnonymous]
        [HttpPost(ApiRoutes.Identity.Login)]
        public async Task<ActionResult> Login(LoginDTO model)
        {
            try
            {


                var user = await _users.Entity.GetAsync(predicate: x => x.PhoneNumber == model.phone
                && (x.typeUser == (model.type_user != 0 ? model.type_user : x.typeUser))
                && x.FK_BranchID == model.branch_id, include: source => source.Include(i => i.ProviderAditionalData).ThenInclude(i => i.FK_District).Include(i => i.ProviderAditionalData).ThenInclude(i => i.FK_Category));
                //var user = await _users.Entity.GetAsync(predicate: x => x.PhoneNumber == model.phone
                //&&((model.branch_id == 3 || model.branch_id == 5) ? (x.typeUser == (model.type_user != 0 ? model.type_user : x.typeUser/*(int)TypeUser.client*/)) : true) 
                //&& x.FK_BranchID == model.branch_id, include: source => source.Include(i => i.ProviderAditionalData).ThenInclude(i => i.FK_District).Include(i => i.ProviderAditionalData).ThenInclude(i => i.FK_Category));


                #region validation
                if (user == null)
                {
                    return Ok(new
                    {
                        key = 0,
                        msg = CreatMessage(model.lang, "من فضلك  تاكد من رقم الهاتف", "Please make sure your phone number")
                    });
                }
                if (model.phone == "")
                {
                    return Ok(new
                    {
                        key = 0,
                        msg = CreatMessage(model.lang, "من فضلك  تاكد من رقم الهاتف", "Please enter your phone number")
                    });
                }
                if (model.password == "")
                {
                    return Ok(new
                    {
                        key = 0,
                        msg = CreatMessage(model.lang, "من فضلك ادخل  كلمة المرور ", "Please enter your  password")
                    });
                }
                if (model.password != user.showPassword)
                {
                    return Ok(new
                    {
                        key = 0,
                        msg = CreatMessage(model.lang, "من فضلك تاكد من كلمة المرور ", "Please sure your  password")
                    });
                }
                if (user.isActive == false)
                {
                    return Ok(new
                    {
                        key = 0,
                        data = new { },
                        status = 0,
                        msg = CreatMessage(model.lang, "تم حظر حسابكم, لمزيد من الأستفسارات يرجى التواصل معنا", "This account is blocked, for more info contact us")
                    });
                }
                if (user.activeCode == false)
                {
                    return Ok(new
                    {
                        key = 1,
                        data = new
                        {
                            id = user.Id,
                            user.code
                        },

                        status = 2,
                        msg = CreatMessage(model.lang, "هذا الحساب لم يفعل بعد", "This account is not active")
                    });
                }
                #endregion
                if (user != null && await _userManager.CheckPasswordAsync(user, model.password))
                {
                    var claim = new[] {
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                    new Claim("user_id", user.Id),
                    new Claim("branch_id", user.FK_BranchID.ToString())
                };
                    var signinKey = new SymmetricSecurityKey(
                      Encoding.UTF8.GetBytes(_configuration["Jwt:SigningKey"]));

                    int expiryInMinutes = Convert.ToInt32(_configuration["Jwt:ExpiryInMinutes"]);

                    var token = new JwtSecurityToken(
                      issuer: _configuration["Jwt:Site"],
                      audience: _configuration["Jwt:Site"],
                      claims: claim,
                      expires: DateTime.UtcNow.AddMinutes(expiryInMinutes),
                      signingCredentials: new SigningCredentials(signinKey, SecurityAlgorithms.HmacSha256)
                    );

                    var check_device_id = await _deviceIds.Entity.GetAsync(x => x.deviceID == model.device_id);
                    if (check_device_id == null && (user.typeUser == (int)TypeUser.client || user.typeUser == (int)TypeUser.provider))
                    {
                        
                        try
                        {
                            DeviceIds d = new DeviceIds()
                            {
                                deviceID = model.device_id,
                                FK_UserID = user.Id,
                                deviceType = "android",
                                date = GetCurrentDate()
                            };
                            user.lang = model.lang;
                            await _deviceIds.Entity.InsertAsync(d);
                            _users.Entity.Update(user);
                            await _users.Save();
                        }
                        catch (Exception)
                        {
                        }
                        

                    }

                    return Ok(new
                    {
                        key = 1,
                        data = GetUserInfo(user),
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expiration = token.ValidTo,
                        status = 1,
                        msg = CreatMessage(model.lang, "تم تسجيل الدخول بنجاح", "Logged in successfully"),
                    });

                }
                return Unauthorized();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [HttpPost(ApiRoutes.Identity.UpdateDataUser)]
        public async Task<ActionResult> UpdateDataUser(UpdateDataUserDTO model)
        {
            try
            {

                var userId = User.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;

                var user = await _users.Entity.GetAsync(predicate: x => x.Id == userId, include: source => source.Include(i => i.ProviderAditionalData).ThenInclude(i => i.FK_District).Include(i => i.ProviderAditionalData).ThenInclude(i => i.FK_Category));

                //userModel.user_id = userId;

                #region validation
                //name

                if (model.user_name == null)
                {
                    return Ok(new
                    {
                        key = 0,
                        msg = CreatMessage(model.lang, "من فضلك ادخل الاسم", "Please enter your Name")
                    });
                }
                if (model.phone == null)
                {
                    return Ok(new
                    {
                        key = 0,
                        msg = CreatMessage(model.lang, "من فضلك ادخل رقم الجوال", "Please enter your phone number")
                    });
                }

                if (user == null)
                {
                    return Ok(new
                    {
                        key = 0,
                        msg = CreatMessage(model.lang, "من فضلك تاكد من بياناتك", "Please verify your data")
                    });
                }

                var phone = await _users.Entity.GetAsync(predicate: x => x.PhoneNumber == model.phone && x.Id != userId && x.FK_BranchID == user.FK_BranchID);
                if (phone != null)
                {
                    return Ok(new
                    {
                        key = 0,
                        msg = CreatMessage(model.lang, "عذرا هذا الجوال موجود بالفعل", "Sorry this mobile is already present")
                    });
                }
                var UserName = await _users.Entity.GetAsync(predicate: x => x.fullName == model.user_name && x.Id != userId && x.FK_BranchID == user.FK_BranchID);
                if (UserName != null)
                {
                    return Ok(new
                    {
                        key = 0,
                        msg = CreatMessage(model.lang, "عذرا هذا الاسم موجود بالفعل", "Sorry this name is already present")
                    });
                }

                //email
                //if (model.email == null)
                //{
                //    return Ok(new
                //    {
                //        key = 0,
                //        msg = CreatMessage(model.lang, "من فضلك ادخل البريد الالكترونى", "Please enter your email")
                //    });
                //}
                //bool checkemail = IsValidEmail(model.email);
                //if (!checkemail)
                //{
                //    return Ok(new
                //    {
                //        key = 0,
                //        msg = CreatMessage(model.lang, "تاكد من ان البريد اللاكترونى مكتوب بشكل صحيح", "Make sure your e-mail is written correctly")
                //    });
                //}
                //var email = await _users.Entity.GetAsync(predicate: x => x.Email == model.email && x.Id != userId);
                //if (email != null)
                //{
                //    return Ok(new
                //    {
                //        key = 0,
                //        msg = CreatMessage(model.lang, "عذرا هذا البريد الالكترونى موجود بالفعل", "Sorry this email is already present")
                //    });
                //}


                #endregion


                user.fullName = model.user_name;
                user.UserName = user.FK_BranchID.ToString() + user.typeUser.ToString() + model.phone;
                user.PhoneNumber = model.phone;
                user.Email = model.email ?? user.Email;

                user.ProviderAditionalData.BankName = model.bankName != null ? model.bankName : user.ProviderAditionalData.BankName;
                user.ibanNumber = model.ibanNumber != null ? model.ibanNumber : user.ibanNumber;
                user.ProviderAditionalData.IdNumber = model.idNumber != null ? model.idNumber : user.ProviderAditionalData.IdNumber;


                if (model.img != null)
                {
                    user.img = ProcessUploadedFile(_HostingEnvironment, model.img, "Users");
                }

                _users.Entity.Update(user);
                _providerAdditionData.Entity.Update(user.ProviderAditionalData);


                await _users.Save();
                await _providerAdditionData.Save();



                return Ok(new
                {
                    key = 1,
                    data = GetUserInfo(user),
                    msg = CreatMessage(model.lang, "تم التعديل بنجاح", "successfully modified")
                });
            }
            catch (Exception ex)
            {

                return Ok(new
                {
                    key = 0,
                    msg = ex.Message
                });
            }

        }

        [HttpPost(ApiRoutes.Identity.ChangePassward)]
        public async Task<IActionResult> ChangePassword(ChangePasswordDTO userModel)
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Ok(new
                {
                    key = 0,
                    msg = CreatMessage(userModel.lang, "عذرا لم يتم العثور على هذا المستخدم ", "Sorry this User was not found")
                });
            }
            if (userModel.old_password == null)
            {
                return Ok(new
                {
                    key = 0,
                    msg = CreatMessage(userModel.lang, "من فضلك ادخل كلمة المرور القديمة ", "Please enter your old password")
                });
            }
            if (userModel.new_password == null)
            {
                return Ok(new
                {
                    key = 0,
                    msg = CreatMessage(userModel.lang, "من فضلك ادخل كلمة المرور الجديدة  ", "Please enter your new password")
                });
            }
            if (userModel.new_password.Length < 6)
            {
                return Ok(new
                {
                    key = 0,
                    msg = CreatMessage(userModel.lang, "كلمة المرور اقل من 6 حروق  ", "new password less that 6 character")
                });
            }
            if (user.showPassword != userModel.old_password)
            {
                return Ok(new
                {
                    key = 0,
                    msg = CreatMessage(user.lang, " كلمة المرور القديمة غير صحيحة", "Please enter the old password correctly")
                });
            }


            var changePasswordResult = await _userManager.ChangePasswordAsync(user, userModel.old_password, userModel.new_password);
            if (!changePasswordResult.Succeeded)
            {
                return Ok(new { key = 0, msg = CreatMessage(userModel.lang, changePasswordResult.ToString(), "Something went wrong") });
            }
            user.showPassword = userModel.new_password;
            _users.Entity.Update(user);
            await _users.Save();
            //await _signInManager.RefreshSignInAsync(user);
            return Ok(new { key = 1, msg = CreatMessage(userModel.lang, "تم تغيير الباسورد بنجاح", "Password changed successfully") });
        }

        [AllowAnonymous]
        [HttpPost(ApiRoutes.Identity.ForgetPassword)]
        public async Task<ActionResult> ForgetPassword(ForgetPasswordDTO userModel)
        {
            try
            {
                if (userModel.phone == "")
                {
                    return Ok(new
                    {
                        key = 0,
                        msg = CreatMessage(userModel.lang, "من فضلك ادخل رقم الهاتف", "Plaese enter your phone number")
                    });
                }
                var codeuser = await _users.Entity.GetAsync(predicate: x => x.PhoneNumber == userModel.phone
                && (x.typeUser == (userModel.type_user != 0 ? userModel.type_user : x.typeUser))
                //&& ( userModel.branch_id == 3 ? ( x.typeUser == (userModel.type_user != 0 ? userModel.type_user : (int)TypeUser.client ) ) : true)
                && x.FK_BranchID == userModel.branch_id);

                if (codeuser != null)
                {
                    if (codeuser.isActive == false)
                    {
                        return Ok(new
                        {
                            key = 0,
                            data = new { },
                            status = "blocked",
                            msg = CreatMessage(userModel.lang, "هذا الحساب مغلق من قبل الادمن", "This account is closed by the addict")
                        });
                    }
                    string code = GetFormNumber(userModel.branch_id);
                    string s = await SendMessage(code, userModel.phone, branch: codeuser.FK_BranchID);
                    codeuser.code = code;
                    _users.Entity.Update(codeuser);
                    await _users.Save();
                    return Ok(new
                    {
                        key = 1,
                        code = new { code = code, user_id = codeuser.Id },
                        msg = CreatMessage(userModel.lang, "تم ارسال الكود ", "Code sent"),
                        status = "active",
                    });
                }
                else
                {
                    return Ok(new { key = 0, msg = CreatMessage(userModel.lang, "عذرا رقم الهاتف غير مسجل لدينا", "Sorry phone number is not registered") });
                }
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    key = 0,
                    msg = ex.Message
                });
            }
        }


        [AllowAnonymous]
        [HttpPost(ApiRoutes.Identity.ResendCode)]
        public async Task<ActionResult> ResendCode(ResendCodeDTO model)
        {
            try
            {
                if (model.user_id == "")
                {
                    return Ok(new
                    {
                        key = 0,
                        msg = CreatMessage(model.lang, "من فضلك تاكد من البيانات", "Please verify the data")
                    });
                }
                var codeuser = await _users.Entity.FindByIdAsync(model.user_id);

                if (codeuser != null)
                {
                    string code = GetFormNumber(codeuser.FK_BranchID);
                    await SendMessage(code, codeuser.PhoneNumber, branch: codeuser.FK_BranchID);
                    codeuser.code = code;
                    _users.Entity.Update(codeuser);
                    await _users.Save();

                    return Ok(new
                    {
                        key = 1,
                        code = new { code = code, user_id = codeuser.Id, phone = codeuser.PhoneNumber },
                        msg = CreatMessage(model.lang, "تم ارسال الكود ", "Code sent"),

                    });
                }
                else
                {
                    return Ok(new { key = 0, msg = CreatMessage(model.lang, "عذرا رقم الهاتف غير مسجل لدينا", "Sorry phone number is not registered") });
                }

            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    key = 0,
                    msg = ex.Message
                });
            }
        }


        [AllowAnonymous]
        [HttpPost(ApiRoutes.Identity.ChangePasswordByCode)]
        public async Task<IActionResult> ChangePasswordByCode(ChangePasswordByCodeDTO model)
        {
            try
            {
                if (model.code == null)
                {
                    return Ok(new
                    {
                        key = 0,
                        msg = CreatMessage(model.lang, "من فضلك ادخل كود التحقق", "Please enter your verification code")
                    });
                }
                if (model.new_password == "")
                {
                    return Ok(new
                    {
                        key = 0,
                        msg = CreatMessage(model.lang, "من فضلك ادخل كلمة المرور الجديدة ", "Please enter your new password")
                    });
                }
                try
                {

                    var codeuser = await _users.Entity.GetAsync(predicate: x => x.code == model.code && x.Id == model.user_id, disableTracking: false);

                    if (codeuser != null)
                    {
                        var changePasswordResult = await _userManager.ChangePasswordAsync(codeuser, codeuser.showPassword, model.new_password);
                        if (!changePasswordResult.Succeeded)
                        {
                            return Ok(new { key = 0, msg = CreatMessage(model.lang, changePasswordResult.ToString(), "Something went wrong") });
                        }
                        codeuser.showPassword = model.new_password;
                        _users.Entity.Update(codeuser);
                        await _users.Save();

                        return Ok(new { key = 1, msg = CreatMessage(model.lang, "تم تغيير الباسورد بنجاح", "Password changed successfully"), data = new { } });
                    }
                    else
                    {
                        return Ok(new { key = 0, msg = CreatMessage(model.lang, " كود التحقق غير صحيح", "  Invalid verification code") });
                    }
                }
                catch (Exception)
                {
                    return Ok(new { key = 0, msg = CreatMessage(model.lang, "حدث خطا ما", "Something went wrong") });
                }
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    key = 0,
                    msg = ex.Message
                });
            }
        }



        [HttpPost(ApiRoutes.Identity.logout)]
        public async Task<ActionResult> Logout(LogoutDTO model)
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;

                // AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                var info = await _deviceIds.Entity.GetAsync(predicate: x => x.deviceID == model.device_id && x.FK_UserID == userId);

                if (info != null)
                {
                    _deviceIds.Entity.Delete(info);
                    await _deviceIds.Save();

                    return Ok(new
                    {
                        key = 1,
                        msg = CreatMessage(model.lang, "تم تسجيل الخروج بنجاح", "Logged out successfully"),
                    });
                }
                else
                {
                    return Ok(new
                    {
                        key = 1,
                        msg = CreatMessage(model.lang, "تم تسجيل الخروج بنجاح", "Logged out successfully"),
                    });
                }
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    key = 0,
                    msg = ex.Message
                });
            }
        }


        [HttpPost(ApiRoutes.Identity.ChangeLanguage)]
        public async Task<ActionResult> ChangeLanguage(ChangeLanguageDTO model)
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;

                var user = await _users.Entity.GetAsync(predicate: x => x.Id == userId, include: source => source.Include(i => i.ProviderAditionalData).ThenInclude(i => i.FK_District).Include(i => i.ProviderAditionalData).ThenInclude(i => i.FK_Category));
                if (user != null)
                {
                    user.lang = model.language ?? model.lang;
                    _users.Entity.Update(user);
                    await _users.Save();

                    return Ok(new
                    {
                        key = 1,
                        data = GetUserInfo(user),
                        msg = CreatMessage(model.language ?? model.lang, "تم  بنجاح", " successfully"),
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
            catch (Exception ex)
            {
                return Ok(new
                {
                    key = 0,
                    msg = ex.Message
                });
            }
        }

        [HttpPost(ApiRoutes.Identity.RemoveAccount)]
        public async Task<ActionResult> RemoveAccount(string lang)
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;
                int branchId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == "branch_id")?.Value ?? "0");
                //var user = await _userManager.FindByIdAsync(userId);
                var user = await _users.Entity.GetAsync(u => u.Id == userId && u.FK_BranchID == branchId);

                if (user != null)
                {
                    user.Email = $"{user.Email}-{Guid.NewGuid()}";
                    user.PhoneNumber = $"{user.PhoneNumber}-{Guid.NewGuid()}";
                    user.UserName = $"{user.UserName}-{Guid.NewGuid()}";
                    user.NormalizedUserName = $"{user.NormalizedUserName}-{Guid.NewGuid()}";
                    user.fullName = $"{user.fullName}-{Guid.NewGuid()}";
                    user.isDeleted = true;
                    await _userManager.UpdateAsync(user);

                    return Ok(new
                    {
                        key = 1,
                        msg = CreatMessage(lang, "تم  بنجاح", " successfully"),
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
            catch (Exception ex)
            {
                return Ok(new
                {
                    key = 0,
                    msg = ex.Message
                });
            }
        }


        #endregion

        [AllowAnonymous]
        [HttpGet(ApiRoutes.Identity.DeleteExpiredOrders)]
        public async Task<IActionResult> DeleteExpiredOrders()
        {
            var setting = await _settings.Entity.GetAsync(s => s.FK_BranchID == 6);
            var expiredOrders = await _orders.Entity.GetCustomAll(o =>
                                                                        o.FK_User.FK_BranchID == 6
                                                                        && o.status == (int)OrderStates.waiting
                                                                        && (o.date.AddMinutes(setting.ExpireTime) < GetCurrentDate())
                                                                        ).Include(o => o.FK_User).ToListAsync();

            foreach (var order in expiredOrders)
            {
                order.FK_User.userWallet += order.price;
                order.status = (int)OrderStates.canceled;
            }

            await _orders.Save();
            //_counter ++;
            return Ok(true);
            //return Ok(_counter);
        }
    }
}
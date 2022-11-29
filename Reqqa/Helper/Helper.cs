using Core.TableDb;
using GeoCoordinatePortable;
using Nancy.Json;
using QRCoder;
using Salony.ViewModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Salony.Enums.AllEnums;

namespace Salony.Helper
{
    public class Helper
    {
        //public readonly static string BaseUrlHoste = "https://localhost:44315/";
        public readonly static string BaseUrlHoste = "https://mashaghil.ip4s.com/";


        public static DateTime GetCurrentDate()
        {
            DateTime date = DateTime.UtcNow.AddHours(3);
            return date;
        }

        public static string GetFormNumber(int branch = 2)
        {
            int code = 1234;
            if (branch == 1 || branch == 3 || branch == 4 || branch == 5 || branch == 6 || branch == 9)
            {
                Random rnd = new Random();
                code = rnd.Next(1000, 9999);
                //int code = 1234;
            }
            return code.ToString();
        }

        #region QrCode

        public static byte[] GenerateQrcode(string textCode)
        {


            byte[] QrCode = null;

            if (textCode != null)
            {
                QRCodeGenerator qr = new QRCodeGenerator();
                QRCodeData data = qr.CreateQrCode(textCode, QRCodeGenerator.ECCLevel.Q);
                QRCode code = new QRCode(data);

                Bitmap bitMap = code.GetGraphic(20);
                QrCode = BitmapToBytesCode(bitMap);
                return QrCode;
            }
            return QrCode;
        }

        private static byte[] BitmapToBytesCode(Bitmap image)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }
        #endregion



        public static string CreatMessage(string lang, string textAr, string textEn)
        {
            if (lang == "ar")
            {
                return textAr;
            }
            else
            {
                return textEn;
            }

        }


        public static string GetOrderStatus(int status, string lang)
        {
            return status switch
            {
                0 => CreatMessage(lang, "جديد", "new"),
                1 => CreatMessage(lang, "مقبول", "accepted"),
                2 => CreatMessage(lang, "منتهى", "finished"),
                3 => CreatMessage(lang, "مرفوض", "refused"),
                4 => CreatMessage(lang, "ملغى", "canceled"),
                _ => CreatMessage(lang, "خطأ", "error")
            };
        }

        public static double GetDistance(string sLatitude, string sLongitude, string eLatitude, string eLongitude)
        {
            try
            {
                double sLatitude1 = Convert.ToDouble(sLatitude);
                double sLongitude1 = Convert.ToDouble(sLongitude);
                double eLatitude1 = Convert.ToDouble(eLatitude);
                double eLongitude1 = Convert.ToDouble(eLongitude);

                var sCoord = new GeoCoordinate(sLatitude1, sLongitude1);
                var eCoord = new GeoCoordinate(eLatitude1, eLongitude1);

                double dd = sCoord.GetDistanceTo(eCoord) / 1000;
                double distance = Math.Round(dd, 1, MidpointRounding.ToEven);
                return distance;

            }
            catch (Exception)
            {

                return 10000;
            }
        }


        static public async Task<string> SendMessage(string msg, string numbers, int branch)
        {
            if (branch == 1)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://www.4jawaly.net/");
                    //HTTP GET
                    var response = await client.GetAsync($"api/sendsms.php?username=Salony&password=565656&numbers={numbers}&sender=Salony&message={msg}&&return=string");
                    var responseString = await response.Content.ReadAsStringAsync();

                    return responseString;
                }
            }
            else if (branch == 3)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://www.4jawaly.net/");
                    //HTTP GET
                    var response = await client.GetAsync($"api/sendsms.php?username=care-time&password=care635241&numbers={numbers}&sender=Care Time&message={msg}&&return=string");
                    var responseString = await response.Content.ReadAsStringAsync();

                    return responseString;
                }
            }
            else if (branch == 4)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://www.4jawaly.net/");
                    //HTTP GET
                    var response = await client.GetAsync($"api/sendsms.php?username=gamaly&password=741523&numbers={numbers}&sender=Gamaly&message={msg}&&return=string");
                    var responseString = await response.Content.ReadAsStringAsync();

                    return responseString;
                }
            }
            else if (branch == 5)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://www.4jawaly.net/");
                    //HTTP GET
                    //var response = await client.GetAsync($"api/sendsms.php?username=salony&password=SS-147852&numbers={numbers}&sender=Salony&message={msg}&&return=string");
                    var response = await client.GetAsync($"api/sendsms.php?username=salony&password=450eXqA145266&numbers={numbers}&sender=Salony&message={msg}&&return=string");
                    var responseString = await response.Content.ReadAsStringAsync();

                    return responseString;
                }
            }
            else if (branch == 6)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://www.4jawaly.net/");
                    //HTTP GET
                    var response = await client.GetAsync($"api/sendsms.php?username=Ekleel&password=90807050&numbers={numbers}&sender=EKLEEL&message={msg}&&return=string");
                    var responseString = await response.Content.ReadAsStringAsync();

                    return responseString;
                }
            }
            else if (branch == 8)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://www.4jawaly.net/");
                    //HTTP GET
                    var response = await client.GetAsync($"api/sendsms.php?username=myspa&password=638541&numbers={numbers}&sender=MYSPA&message={msg}&&return=string");
                    var responseString = await response.Content.ReadAsStringAsync();

                    return responseString;
                }
            }
            else if (branch == 9)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://www.4jawaly.net/");
                    //HTTP GET
                    var response = await client.GetAsync($"api/sendsms.php?username=ladieclub&password=ladi6385&numbers={numbers}&sender=LadiesClub&message={msg}&&return=string");
                    var responseString = await response.Content.ReadAsStringAsync();

                    return responseString;
                }
            }
            else
            {
                return string.Empty;
            }
        }

        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    var domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                    @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        public static UserInfoViewModel GetUserInfo(ApplicationDbUser user, string lang = "ar")
        {
            if (user.typeUser == (int)Enums.AllEnums.TypeUser.provider)
            {
                var UserDate = new UserInfoViewModel
                {
                    id = user.Id,
                    user_name = user.fullName,
                    img = BaseUrlHoste + user.img,
                    phone = user.PhoneNumber,
                    //email = user.Email,
                    lang = user.lang,
                    active = user.isActive,
                    close_notify = user.closeNotification,
                    type = user.typeUser,
                    boutique_name_ar = user.ProviderAditionalData.nameAr,
                    boutique_name_en = user.ProviderAditionalData.nameEn,
                    email = user.Email,
                    commercial_register_number = user.ProviderAditionalData.commercialRegister,
                    city = user.ProviderAditionalData.FK_District.FK_CityID,
                    district = user.ProviderAditionalData.FK_DistrictID,
                    address = user.ProviderAditionalData.address,
                    lat = user.ProviderAditionalData.lat,
                    lng = user.ProviderAditionalData.lng,
                    time_from = user.ProviderAditionalData.timeForm.ToString("HH:mm"),
                    time_to = user.ProviderAditionalData.timeTo.ToString("HH:mm"),
                    days = GetDays(user.ProviderAditionalData.dayWorks, lang),
                    salonType = user.ProviderAditionalData.salonType,
                    description_ar = user.ProviderAditionalData.descriptionAr,
                    description_en = user.ProviderAditionalData.descriptionEn,
                    iDPhoto = BaseUrlHoste + user.iDPhoto,
                    certificatePhoto = BaseUrlHoste + user.certificatePhoto,
                    ibanNumber = user.ibanNumber,
                    categoryId = user.ProviderAditionalData.FK_CategoryID,
                    categoryName = Helper.CreatMessage(lang, user.ProviderAditionalData.FK_Category.nameAr, user.ProviderAditionalData.FK_Category.nameEn),
                    rate = user.ProviderAditionalData.rate,
                    idNumber = user.ProviderAditionalData.IdNumber,
                    bankName = user.ProviderAditionalData.BankName,
                    invitationCode = user.invitationCode,

                    isAvailable = user.IsAvailable

                };

                return UserDate;

            }
            else
            {
                var UserDate = new UserInfoViewModel
                {
                    id = user.Id,
                    user_name = user.fullName,
                    email = user.Email,
                    img = BaseUrlHoste + user.img,
                    phone = user.PhoneNumber,
                    lang = user.lang,
                    active = user.isActive,
                    close_notify = user.closeNotification,
                    type = user.typeUser,
                    stableWallet = user.userWallet,
                    invitationCode = user.invitationCode
                };

                return UserDate;

            }
        }




        #region Save Image

        public static string ProcessUploadedFile(Microsoft.AspNetCore.Hosting.IWebHostEnvironment HostingEnvironment, Microsoft.AspNetCore.Http.IFormFile Photo, string Place)
        {
            string uniqueFileName = null;
            if (Photo != null)
            {
                string uploadsFolder = Path.Combine(HostingEnvironment.WebRootPath, $"images/{Place}");
                uniqueFileName = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(Photo.FileName).Replace(" ", string.Empty);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    Photo.CopyTo(fileStream);
                }
            }
            return $"Images/{Place}/" + uniqueFileName;
        }


        #endregion




        #region Roles

        public class AuthorizeRolesAttribute : Microsoft.AspNetCore.Authorization.AuthorizeAttribute
        {
            public AuthorizeRolesAttribute(params Enums.AllEnums.Roles[] roles) : base()
            {
                Roles = string.Join(",", roles);
            }

        }


        public static string GetRole(string role, string lang)
        {
            switch (role)
            {
                case "Admin":
                    return lang == "ar" ? "أدمن" : "Admin";

                case "Mobile":
                    return lang == "ar" ? "موبايل" : "Mobile";

                case "Sliders":
                    return lang == "ar" ? "الاعلانات" : "Sliders";

                case "Cities":
                    return lang == "ar" ? "المدن" : "Cities";

                case "Settings":
                    return lang == "ar" ? "الاعدادات" : "Settings";

                case "Users":
                    return lang == "ar" ? "مستخدمين التطبيق" : "Users";

                case "Categories":
                    return lang == "ar" ? "الاقسام" : "Categories";

                case "ContactUs":
                    return lang == "ar" ? "تواصل معنا" : "ContactUs";

                case "Copons":
                    return lang == "ar" ? "اكواد الخصم" : "Copons";
                case "Orders":
                    return lang == "ar" ? "الطلبات" : "Orders";
                case "BankAccounts":
                    return lang == "ar" ? "الحسابات البنكية" : "BankAccounts";
                case "Notifications":
                    return lang == "ar" ? "الاشعارات" : "Notifications";

                default:
                    return role;
            }

        }

        public static List<HelperDaysViewModel> GetDays(string days = "", string lang = "ar")
        {
            List<int> ListSelectedDays = new List<int>();
            try
            {
                ListSelectedDays = days.Split(',').Where(d => !String.IsNullOrEmpty(d)).Select(d => Convert.ToInt32(d)).ToList();
            }
            catch (Exception)
            {

                ListSelectedDays = new List<int>();
            }

            List<HelperDaysViewModel> AllDays = new List<HelperDaysViewModel>() {
                new HelperDaysViewModel{id= 6,name = CreatMessage(lang,"السبت","Satarday"),selected = ListSelectedDays.Contains(6) },
                new HelperDaysViewModel{id= 0,name = CreatMessage(lang,"الاحد","sunday"),selected = ListSelectedDays.Contains(0) },
                new HelperDaysViewModel{id= 1,name = CreatMessage(lang,"الاثنين","monday"),selected = ListSelectedDays.Contains(1) },
                new HelperDaysViewModel{id= 2,name = CreatMessage(lang,"الثلاثاء","tuesday"),selected = ListSelectedDays.Contains(2) },
                new HelperDaysViewModel{id= 3,name = CreatMessage(lang,"الاربعاء","wednesday"),selected = ListSelectedDays.Contains(3) },
                new HelperDaysViewModel{id= 4,name = CreatMessage(lang,"الخميس","thursday"),selected = ListSelectedDays.Contains(4) },
                new HelperDaysViewModel{id= 5,name = CreatMessage(lang,"الجمعة","friday"),selected = ListSelectedDays.Contains(5) },
            };

            return AllDays;
        }

        #endregion


        #region fcm
        public static void SendPushNotification(List<string> device_ids, string msg, int type = 0, int order_id = 0, int branchId = 0, string projectName = "Salony | رقة"/*, bool type, int order_id = 3*/)
        {
            try
            {

                //var devide_ids = (from st in db.Device_Ids where st.Fk_UserId == user_id select st).ToList();
                string applicationID = string.Empty;
                string senderId = String.Empty;
                
                
                applicationID = "AAAAzmyBsEU:APA91bEd-jR1vEpvm84il1XM8Z_V_WFNBrTt_I0pTOl6eVwj-cGKziuY28FFUURLkiy6P25fmykiFBJMDmeHvOT6pvltQAMAU72csp_bjVK6aR7U3GJPvrg94FDqI-dDsgkYTKwQzrXT";
                senderId = "886583701573";
                
                



                foreach (var item in device_ids)
                {
                    try
                    {

                        string deviceId = item;
                        WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                        tRequest.Method = "post";
                        tRequest.ContentType = "application/json";
                        if (true)
                        {
                            var data = new
                            {
                                to = deviceId,

                                notification = new
                                {
                                    body = msg,
                                    title = projectName,
                                    sound = "Enabled",
                                    priority = "high",
                                    type = type,
                                    order_id = order_id

                                },
                                data = new
                                {
                                    body = msg,
                                    title = projectName,
                                    sound = "Enabled",
                                    priority = "high",
                                    type = type,
                                    order_id = order_id

                                }
                            };
                            var serializer = new JavaScriptSerializer();
                            var json = serializer.Serialize(data);
                            Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                            tRequest.Headers.Add(string.Format("Authorization: key={0}", applicationID));
                            tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
                            tRequest.ContentLength = byteArray.Length;
                            using (Stream dataStream = tRequest.GetRequestStream())
                            {
                                dataStream.Write(byteArray, 0, byteArray.Length);
                                using (WebResponse tResponse = tRequest.GetResponse())
                                {
                                    using (Stream dataStreamResponse = tResponse.GetResponseStream())
                                    {
                                        using (StreamReader tReader = new StreamReader(dataStreamResponse))
                                        {
                                            String sResponseFromServer = tReader.ReadToEnd();
                                            string str = sResponseFromServer;
                                        }
                                    }
                                }
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        string str = ex.Message;

                    }

                }


            }
            catch (Exception ex)
            {
                string str = ex.Message;

            }
        }

        #endregion

        public static void SendPushNotification(List<Tuple<string, string>> device_ids, string msg = "", int type = 0, int order_id = 0, string user_id = "", string user_name = "", string user_img = "", bool closeNotify = false, int branchId = 0, string projectName = "Salony | رقة")
        {

            if (!closeNotify)
            {
                try
                {
                    string applicationID = string.Empty;
                    string senderId = String.Empty;
                    if (branchId == 2)
                    {
                        applicationID = "AAAAg9hDLmU:APA91bHD-yogcVtaSr0omn4D2_G6WPIZJ5Rbl-EES5TyK9OXrAKgsPuwfD-gcFYIYM3cOLR2ElCEmZKqNm_hOkEIWeEpsdjDsfCdHStqfTofT3yccyRNV5R_MtSSVFKYfHofR9gRiHCu";
                        senderId = "566268997221";
                    }
                    else
                    {
                        applicationID = "AAAAR3RsYyU:APA91bHf61lWIGOpGCq5NjXvFGIy679tzqmh-neS3SqAG-rE0vRZZTS7iiwXe8xnKmC2-WHDbco3aTPDN7IZg3SAkSmnlsoClAlkSy4jWCzkYmmJufUhiBV_rKa3RGBVg74hhafhzN1M";
                        senderId = "306895938341";
                    }

                    foreach (var item in device_ids)
                    {
                        try
                        {

                            string deviceId = item.Item1;
                            WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                            tRequest.Method = "post";
                            tRequest.ContentType = "application/json";
                            if (item.Item2 == "android")
                            {
                                var data = new
                                {
                                    to = deviceId,
                                    data = new
                                    {
                                        body = msg,
                                        title = projectName,
                                        sound = "Enabled",
                                        priority = "high",
                                        type = type,
                                        orderId = order_id,
                                        userId = user_id,
                                        userName = user_name,
                                        userImg = user_img,
                                    },
                                    notification = new
                                    {
                                        body = msg,
                                        title = projectName,
                                        sound = "Enabled",
                                        priority = "high",
                                        type = type,
                                        orderId = order_id,
                                        userId = user_id,
                                        userName = user_name,
                                        userImg = user_img,
                                    },

                                };
                                var serializer = new JavaScriptSerializer();
                                var json = serializer.Serialize(data);
                                Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                                tRequest.Headers.Add(string.Format("Authorization: key={0}", applicationID));
                                tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
                                tRequest.ContentLength = byteArray.Length;
                                using (Stream dataStream = tRequest.GetRequestStream())
                                {
                                    dataStream.Write(byteArray, 0, byteArray.Length);
                                    using (WebResponse tResponse = tRequest.GetResponse())
                                    {
                                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                                        {
                                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                                            {
                                                String sResponseFromServer = tReader.ReadToEnd();
                                                string str = sResponseFromServer;
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                var data = new
                                {
                                    to = deviceId,
                                    notification = new
                                    {
                                        body = msg,
                                        title = projectName,
                                        sound = "Enabled",
                                        priority = "high",
                                        type = type,
                                        orderId = order_id,
                                        userId = user_id,
                                        userName = user_name,
                                        userImg = user_img,
                                    },
                                };
                                var serializer = new JavaScriptSerializer();
                                var json = serializer.Serialize(data);
                                Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                                tRequest.Headers.Add(string.Format("Authorization: key={0}", applicationID));
                                tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
                                tRequest.ContentLength = byteArray.Length;
                                using (Stream dataStream = tRequest.GetRequestStream())
                                {
                                    dataStream.Write(byteArray, 0, byteArray.Length);
                                    using (WebResponse tResponse = tRequest.GetResponse())
                                    {
                                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                                        {
                                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                                            {
                                                String sResponseFromServer = tReader.ReadToEnd();
                                                string str = sResponseFromServer;
                                            }
                                        }
                                    }
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            string str = ex.Message;

                        }
                    }
                }
                catch (Exception ex)
                {
                    string str = ex.Message;
                }
            }
        }












        // methods static of ClientController
        public static double SaveOrder_Applicationpercentage(int branchId, int cartCount, double appPrecent, double orderPrice = 0)
        {

            //switch (branchId)
            //{
            //    case (int)BranchName.ToGo:
            //        {
            //            return (cartCount * ((appPrecent / 100) * orderPrice));
            //        }
            //    case (int)BranchName.Eleklil:
            //        {
            //            return ((appPrecent / 100) * orderPrice);
            //        }
            //    default:
            //        {
            //            return (cartCount * appPrecent);
            //        }
            //}


            //return branchId switch
            //{
            //    (int)BranchName.ToGo => (cartCount * ((appPrecent / 100) * orderPrice)),
            //    _ => cartCount * appPrecent
            //};

            return (cartCount * appPrecent);
        }
        public static double SaveOrder_Adminpercentage(int branchId, int cartCount, double appPrecent, double orderPrice = 0)
        {
            return (appPrecent + ((cartCount * appPrecent) / 2));
        }
        public static double SaveOrder_Providerpercentage(int branchId, int cartCount, double appPrecent, double orderPrice = 0)
        {
            //return branchId switch
            //{
            //    (int)BranchName.ToGo => ((cartCount * ((appPrecent / 100) * orderPrice)) / 2),
            //    _ => ((cartCount * appPrecent) / 2)
            //};

            return ((cartCount * appPrecent) / 2);
        }
        public static double SaveOrder_AppCommission(int branchId, int cartCount, double appPrecent, double orderPrice = 0)
        {
            //return branchId switch
            //{
            //    (int)BranchName.ToGo => ((appPrecent / 100) * orderPrice),
            //    _ => appPrecent
            //};

            return appPrecent;
        }




        ///////
        /// <summary>
        /// 
        /// </summary>
        public static double distanceBetweenThemInOrder(string address, string FkUserLat, string FkUserLng, string CurrentUserLat, string CurrentUserLng)
        {
            double result = !String.IsNullOrEmpty(address) ? Helper.GetDistance(FkUserLat, FkUserLng, CurrentUserLat, CurrentUserLng) : 0;
            return result;
        }

        //public static double GetPriceDueToBranch(int branchId, double orderPrice, Copons copon, )
        //{

        //    switch (branchId)
        //    {
        //        case (int)BranchName.Eleklil:
        //            {

        //            }
        //            break;
        //        case (int)BranchName.Show:
        //            {

        //            }
        //            break;

        //        default:
        //            {
        //                return orderPrice - ((orderPrice * copon?.discPercentage ?? 0) / 100));
        //            }
        //            break;
        //    }
        //}




    }
}

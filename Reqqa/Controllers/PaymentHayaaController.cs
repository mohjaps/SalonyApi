using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Core.Interfaces;
using Core.TableDb;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Salony.Controllers
{
    public class PaymentHayaaController : Controller
    {

        private readonly IUnitOfWork<Notifications> _notifications;
        private readonly IUnitOfWork<ProviderAditionalData> _providerAditionalData;
        private readonly IUnitOfWork<ApplicationDbUser> _users;
        private readonly IUnitOfWork<Branches> _branches;
        private readonly IUnitOfWork<Orders> _orders;

        private static string checkOutId = "";

        public PaymentHayaaController(IUnitOfWork<Notifications> notifications, IUnitOfWork<ProviderAditionalData> providerAditionalData, IUnitOfWork<ApplicationDbUser> users, IUnitOfWork<Branches> branches, IUnitOfWork<Orders> orders)
        {
            _notifications = notifications;
            _providerAditionalData = providerAditionalData;
            _users = users;
            _branches = branches;
            _orders = orders;
        }

        // GET: Payment
        public ActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> PayRequest(string userId, string amount)
        {

            string entityId,url, authorization = "";
            //test
            //entityId = "entityId=8ac7a4ca7d14ab59017d1daa7cbb0f38";
            //url = "https://test.oppwa.com/v1/checkouts";
            //authorization = "Bearer OGFjN2E0Y2E3ZDE0YWI1OTAxN2QxZGE5ODk2MTBmMzR8QUFSNEtHdEVyag==";

            //life
            entityId = "entityId=8acda4ca7d2dc441017d8ea688e52b9d";
            url = "https://oppwa.com/v1/checkouts";
            authorization = "Bearer OGFjZGE0Y2E3ZDJkYzQ0MTAxN2Q4ZWE1Y2I0ZTJiOTJ8SHQzM1FiQ3B5eQ==";



            var user = await _users.Entity.FindByIdAsync(userId);
            //if (user.is_paid == false)
            //{
            decimal parsed = decimal.Parse(amount);
            Dictionary<string, dynamic> responseData;
            string data = entityId +
                          "&amount=" + parsed.ToString("0.00") +
                          "&currency=SAR" +
                          "&paymentType=DB" +

                          // for test
                          #region for test
                          //"&testMode=EXTERNAL" + 
                          #endregion



                          "&merchantTransactionId=" + Guid.NewGuid().ToString() +
                          "&billing.street1=" + user.address + " address" +
                          "&billing.city=" + user.fullName + " city" +
                          "&billing.state=" + user.fullName + " state" +
                          "&billing.country=" + "SA" + //  Alpha-2 codes
                          "&billing.postcode=" + "11564" +
                          "&customer.email=" + user.Email +
                          "&customer.givenName=" + user.fullName +
                          "&customer.surname=" + user.fullName;
            //string url = "https://oppwa.com/v1/checkouts";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "POST";
            request.Headers["Authorization"] = authorization;
            request.ContentType = "application/x-www-form-urlencoded";
            Stream PostData = request.GetRequestStream();
            PostData.Write(buffer, 0, buffer.Length);
            PostData.Close();
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                responseData = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(reader.ReadToEnd());
                reader.Close();
                dataStream.Close();
            }

            var result = responseData;
            checkOutId = responseData["id"];
            ViewBag.Id = checkOutId;
            ViewBag.userId = userId;
            //ViewBag.orderId = orderId;
            //ViewBag.boutiqueId = boutiqueId;
            return View("Pay");
            //}
            //else
            //{
            //    return Json(new { key = 1, msg = "انت مشترك بالفعل" });
            //}



        }
        [HttpGet]
        public async Task<IActionResult> Submit(string userId)
        {


            string data, url, authorization = "";
            //test
            //data = "entityId=8ac7a4ca7d14ab59017d1daa7cbb0f38";
            //url = "https://test.oppwa.com/v1/checkouts/" + checkOutId + "/payment?" + data;
            //authorization = "Bearer OGFjN2E0Y2E3ZDE0YWI1OTAxN2QxZGE5ODk2MTBmMzR8QUFSNEtHdEVyag==";

            //life
            data = "entityId=8acda4ca7d2dc441017d8ea688e52b9d";
            url = "https://oppwa.com/v1/checkouts/" + checkOutId + "/payment?" + data;
            authorization = "Bearer OGFjZGE0Y2E3ZDJkYzQ0MTAxN2Q4ZWE1Y2I0ZTJiOTJ8SHQzM1FiQ3B5eQ==";




            //if (user.is_paid == false)
            //{
            Dictionary<string, dynamic> responseData;
            //string data = "entityId=8acda4ca7d2dc441017d8ea688e52b9d";
            //string url = "https://oppwa.com/v1/checkouts/" + checkOutId + "/payment?" + data;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "GET";
            //request.Headers["Authorization"] = "Bearer OGFjZGE0Y2E3ZDJkYzQ0MTAxN2Q4ZWE1Y2I0ZTJiOTJ8SHQzM1FiQ3B5eQ==";
            request.Headers["Authorization"] = authorization;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                responseData = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(reader.ReadToEnd());
                reader.Close();
                dataStream.Close();
            }

            var resultCode = responseData["result"]["code"].ToString();

            Regex successPattern = new Regex(@"(000\.000\.|000\.100\.1|000\.[36])");
            Regex successManuelPattern = new Regex(@"(000\.400\.0[^3]|000\.400\.100)");

            Match match1 = successPattern.Match(resultCode);
            Match match2 = successManuelPattern.Match(resultCode);



            if (match1.Success || match2.Success)
            {
                var user = await _users.Entity.FindByIdAsync(userId);
                user.TempPaymentId = responseData["id"];
                await _users.Save();
                //user.is_paid = true;

                return RedirectToAction("Success");
                //return Json(new { key = 1, msg = "تمت العملية بنجاح" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return RedirectToAction("Fail");
                //return Json(new { key = 0, msg = "فشلت العملية" }, JsonRequestBehavior.AllowGet);

            }
            //}
            //else
            //{
            //    return Json(new { key = 0, msg = "انت مشترك بالفعل" });
            //}







        }

        //public ActionResult ApplePayRequest(string userId, string amount)
        //{


        //    var user = _context.Users.FirstOrDefault(u => u.Id == userId);
        //    //if (user.is_paid == false)
        //    //{
        //    decimal parsed = decimal.Parse(amount);
        //    Dictionary<string, dynamic> responseData;
        //    string data = "entityId=8ac9a4cb77af31e50177c40d5f6257e3" +
        //                  "&amount=" + parsed.ToString("0.00") +
        //                  "&currency=SAR" +
        //                  "&paymentType=DB" +
        //                  "&merchantTransactionId=" + Guid.NewGuid().ToString() +
        //                  "&billing.street1=" + user.address + " address" +
        //                  "&billing.city=" + user.fullName + " city" +
        //                  "&billing.state=" + user.fullName + " state" +
        //                  "&billing.country=" + "SA" + //  Alpha-2 codes
        //                  "&billing.postcode=" + "11564" +
        //                  "&customer.email=" + user.Email +
        //                  "&customer.givenName=" + user.fullName +
        //                  "&customer.surname=" + user.fullName;
        //    string url = "https://oppwa.com/v1/checkouts";
        //    byte[] buffer = Encoding.ASCII.GetBytes(data);
        //    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
        //    request.Method = "POST";
        //    request.Headers["Authorization"] = "Bearer OGFjZGE0Y2E3ZDJkYzQ0MTAxN2Q4ZWE1Y2I0ZTJiOTJ8SHQzM1FiQ3B5eQ==";
        //    request.ContentType = "application/x-www-form-urlencoded";
        //    Stream PostData = request.GetRequestStream();
        //    PostData.Write(buffer, 0, buffer.Length);
        //    PostData.Close();
        //    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        //    {
        //        Stream dataStream = response.GetResponseStream();
        //        StreamReader reader = new StreamReader(dataStream);
        //        responseData = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(reader.ReadToEnd());
        //        reader.Close();
        //        dataStream.Close();
        //    }

        //    var result = responseData;
        //    checkOutId = responseData["id"];
        //    ViewBag.Id = checkOutId;
        //    ViewBag.userId = userId;
        //    return View("ApplePay");
        //    //}
        //    //else
        //    //{
        //    //    return Json(new { key = 1, msg = "انت مشترك بالفعل" });
        //    //}



        //}
        //[HttpGet]
        //public ActionResult AppleSubmit(string userId)
        //{

        //    var user = _context.Users.FirstOrDefault(u => u.Id == userId);
        //    //if (user.is_paid == false)
        //    //{
        //    Dictionary<string, dynamic> responseData;
        //    string data = "entityId=8ac9a4cb77af31e50177c40d5f6257e3";
        //    string url = "https://oppwa.com/v1/checkouts/" + checkOutId + "/payment?" + data;
        //    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
        //    request.Method = "GET";
        //    request.Headers["Authorization"] = "Bearer OGFjZGE0Y2E3ZDJkYzQ0MTAxN2Q4ZWE1Y2I0ZTJiOTJ8SHQzM1FiQ3B5eQ==";
        //    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        //    {
        //        Stream dataStream = response.GetResponseStream();
        //        StreamReader reader = new StreamReader(dataStream);
        //        responseData = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(reader.ReadToEnd());
        //        reader.Close();
        //        dataStream.Close();
        //    }

        //    var resultCode = responseData["result"]["code"].ToString();

        //    Regex successPattern = new Regex(@"(000\.000\.|000\.100\.1|000\.[36])");
        //    Regex successManuelPattern = new Regex(@"(000\.400\.0[^3]|000\.400\.100)");

        //    Match match1 = successPattern.Match(resultCode);
        //    Match match2 = successManuelPattern.Match(resultCode);



        //    if (match1.Success || match2.Success)
        //    {
        //        //user.is_paid = true;
        //        _context.SaveChanges();

        //        return RedirectToAction("Success");
        //        //return Json(new { key = 1, msg = "تمت العملية بنجاح" }, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        return RedirectToAction("Fail");
        //        //return Json(new { key = 0, msg = "فشلت العملية" }, JsonRequestBehavior.AllowGet);

        //    }
        //    //}
        //    //else
        //    //{
        //    //    return Json(new { key = 0, msg = "انت مشترك بالفعل" });
        //    //}







        //}

        public async Task<IActionResult> MadaPayRequest(string userId, string amount)
        {


            string entityId, url, authorization = "";
            //test
            //entityId = "entityId=8ac7a4ca7d14ab59017d1dab86860f3d";
            //url = "https://test.oppwa.com/v1/checkouts";
            //authorization = "Bearer OGFjN2E0Y2E3ZDE0YWI1OTAxN2QxZGE5ODk2MTBmMzR8QUFSNEtHdEVyag==";

            //life
            entityId = "entityId=8acda4ca7d2dc441017d8ea736a92ba9";
            url = "https://oppwa.com/v1/checkouts";
            authorization = "Bearer OGFjZGE0Y2E3ZDJkYzQ0MTAxN2Q4ZWE1Y2I0ZTJiOTJ8SHQzM1FiQ3B5eQ==";





            var user = await _users.Entity.FindByIdAsync(userId);
            //if (user.is_paid == false)
            //{
            decimal parsed = decimal.Parse(amount);
            Dictionary<string, dynamic> responseData;
            string data = entityId +
                          "&amount=" + parsed.ToString("0.00") +
                          "&currency=SAR" +
                          "&paymentType=DB" +

                          // for test
                          //"&testMode=INTERNAL" +


                          "&merchantTransactionId=" + Guid.NewGuid().ToString() +
                          "&billing.street1=" + user.address + " address" +
                          "&billing.city=" + user.fullName + " city" +
                          "&billing.state=" + user.fullName + " state" +
                          "&billing.country=" + "SA" + //  Alpha-2 codes
                          "&billing.postcode=" + "11564" +
                          "&customer.email=" + user.Email +
                          "&customer.givenName=" + user.fullName +
                          "&customer.surname=" + user.fullName;

            //string url = "https://oppwa.com/v1/checkouts";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "POST";
            //request.Headers["Authorization"] = "Bearer OGFjZGE0Y2E3ZDJkYzQ0MTAxN2Q4ZWE1Y2I0ZTJiOTJ8SHQzM1FiQ3B5eQ==";
            request.Headers["Authorization"] = authorization;
            request.ContentType = "application/x-www-form-urlencoded";
            Stream PostData = request.GetRequestStream();
            PostData.Write(buffer, 0, buffer.Length);
            PostData.Close();
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                responseData = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(reader.ReadToEnd());
                reader.Close();
                dataStream.Close();
            }

            var result = responseData;
            checkOutId = responseData["id"];
            ViewBag.Id = checkOutId;
            ViewBag.userId = userId;
            //ViewBag.orderId = orderId;
            //ViewBag.boutiqueId = boutiqueId;
            return View("MadaPay");
            //}
            //else
            //{
            //    return Json(new { key = 1, msg = "انت مشترك بالفعل" });
            //}



        }
        [HttpGet]
        public async Task<IActionResult> MadaSubmit(string userId)
        {


            string data, url, authorization = "";
            //test
            //data = "entityId=8ac7a4ca7d14ab59017d1dab86860f3d";
            //url = "https://test.oppwa.com/v1/checkouts/" + checkOutId + "/payment?" + data;
            //authorization = "Bearer OGFjN2E0Y2E3ZDE0YWI1OTAxN2QxZGE5ODk2MTBmMzR8QUFSNEtHdEVyag==";

            //life
            data = "entityId=8acda4ca7d2dc441017d8ea736a92ba9";
            url = "https://oppwa.com/v1/checkouts/" + checkOutId + "/payment?" + data;
            authorization = "Bearer OGFjZGE0Y2E3ZDJkYzQ0MTAxN2Q4ZWE1Y2I0ZTJiOTJ8SHQzM1FiQ3B5eQ==";





            //if (user.is_paid == false)
            //{
            Dictionary<string, dynamic> responseData;
            //string data = "entityId=8acda4ca7d2dc441017d8ea736a92ba9";
            //string url = "https://oppwa.com/v1/checkouts/" + checkOutId + "/payment?" + data;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "GET";
            //request.Headers["Authorization"] = "Bearer OGFjZGE0Y2E3ZDJkYzQ0MTAxN2Q4ZWE1Y2I0ZTJiOTJ8SHQzM1FiQ3B5eQ==";
            request.Headers["Authorization"] = authorization;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                responseData = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(reader.ReadToEnd());
                reader.Close();
                dataStream.Close();
            }

            var resultCode = responseData["result"]["code"].ToString();

            Regex successPattern = new Regex(@"(000\.000\.|000\.100\.1|000\.[36])");
            Regex successManuelPattern = new Regex(@"(000\.400\.0[^3]|000\.400\.100)");

            Match match1 = successPattern.Match(resultCode);
            Match match2 = successManuelPattern.Match(resultCode);



            if (match1.Success || match2.Success)
            {
                //user.is_paid = true;
                var user = await _users.Entity.FindByIdAsync(userId);
                user.TempPaymentId = responseData["id"];
                await _users.Save();
                return RedirectToAction("Success");
                //return Json(new { key = 1, msg = "تمت العملية بنجاح" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return RedirectToAction("Fail");
                //return Json(new { key = 0, msg = "فشلت العملية" }, JsonRequestBehavior.AllowGet);

            }
            //}
            //else
            //{
            //    return Json(new { key = 0, msg = "انت مشترك بالفعل" });
            //}







        }

        public async Task<IActionResult> ApplePayRequest(string userId, string amount)
        {


            string entityId, url, authorization = "";
            //test
            //entityId = "entityId=8ac7a4ca7ee948b7017ef76757850ff1";
            //url = "https://test.oppwa.com/v1/checkouts";
            //authorization = "Bearer OGFjN2E0Y2E3ZDE0YWI1OTAxN2QxZGE5ODk2MTBmMzR8QUFSNEtHdEVyag==";

            //life
            entityId = "entityId=8ac9a4cd7f924219017fabfc655569c3";
            url = "https://oppwa.com/v1/checkouts";
            authorization = "Bearer OGFjZGE0Y2E3ZDJkYzQ0MTAxN2Q4ZWE1Y2I0ZTJiOTJ8SHQzM1FiQ3B5eQ==";





            var user = await _users.Entity.FindByIdAsync(userId);
            //if (user.is_paid == false)
            //{
            decimal parsed = decimal.Parse(amount);
            Dictionary<string, dynamic> responseData;
            string data = entityId +
                          "&amount=" + parsed.ToString("0.00") +
                          "&currency=SAR" +
                          "&paymentType=DB" +
                          "&merchantTransactionId=" + Guid.NewGuid().ToString() +
                          "&billing.street1=" + user.address + " address" +
                          "&billing.city=" + user.fullName + " city" +
                          "&billing.state=" + user.fullName + " state" +
                          "&billing.country=" + "SA" + //  Alpha-2 codes
                          "&billing.postcode=" + "11564" +
                          "&customer.email=" + user.Email +
                          "&customer.givenName=" + user.fullName +
                          "&customer.surname=" + user.fullName;
            //string url = "https://oppwa.com/v1/checkouts";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "POST";
            //request.Headers["Authorization"] = "Bearer OGFjZGE0Y2E3ZDJkYzQ0MTAxN2Q4ZWE1Y2I0ZTJiOTJ8SHQzM1FiQ3B5eQ==";
            request.Headers["Authorization"] = authorization;
            request.ContentType = "application/x-www-form-urlencoded";
            Stream PostData = request.GetRequestStream();
            PostData.Write(buffer, 0, buffer.Length);
            PostData.Close();
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                responseData = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(reader.ReadToEnd());
                reader.Close();
                dataStream.Close();
            }

            var result = responseData;
            checkOutId = responseData["id"];
            ViewBag.Id = checkOutId;
            ViewBag.userId = userId;
            //ViewBag.orderId = orderId;
            //ViewBag.boutiqueId = boutiqueId;
            return View("ApplePay");
            //}
            //else
            //{
            //    return Json(new { key = 1, msg = "انت مشترك بالفعل" });
            //}



        }
        [HttpGet]
        public async Task<IActionResult> AppleSubmit(string userId)
        {


            string data, url, authorization = "";
            //test
            //data = "entityId=8ac7a4ca7ee948b7017ef76757850ff1";
            //url = "https://test.oppwa.com/v1/checkouts/" + checkOutId + "/payment?" + data;

            //authorization = "Bearer OGFjN2E0Y2E3ZDE0YWI1OTAxN2QxZGE5ODk2MTBmMzR8QUFSNEtHdEVyag==";

            //life
            data = "entityId=8ac9a4cd7f924219017fabfc655569c3";
            url = "https://oppwa.com/v1/checkouts/" + checkOutId + "/payment?" + data;
            authorization = "Bearer OGFjZGE0Y2E3ZDJkYzQ0MTAxN2Q4ZWE1Y2I0ZTJiOTJ8SHQzM1FiQ3B5eQ==";





            //if (user.is_paid == false)
            //{
            Dictionary<string, dynamic> responseData;
            //string data = "entityId=8ac9a4cd7f924219017fabfc655569c3";
            //string url = "https://oppwa.com/v1/checkouts/" + checkOutId + "/payment?" + data;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "GET";
            //request.Headers["Authorization"] = "Bearer OGFjZGE0Y2E3ZDJkYzQ0MTAxN2Q4ZWE1Y2I0ZTJiOTJ8SHQzM1FiQ3B5eQ==";
            request.Headers["Authorization"] = authorization;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                responseData = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(reader.ReadToEnd());
                reader.Close();
                dataStream.Close();
            }

            var resultCode = responseData["result"]["code"].ToString();

            Regex successPattern = new Regex(@"(000\.000\.|000\.100\.1|000\.[36])");
            Regex successManuelPattern = new Regex(@"(000\.400\.0[^3]|000\.400\.100)");

            Match match1 = successPattern.Match(resultCode);
            Match match2 = successManuelPattern.Match(resultCode);



            if (match1.Success || match2.Success)
            {
                //user.is_paid = true;
                var user = await _users.Entity.FindByIdAsync(userId);
                user.TempPaymentId = responseData["id"];
                await _users.Save();
                return RedirectToAction("Success");
                //return Json(new { key = 1, msg = "تمت العملية بنجاح" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return RedirectToAction("Fail");
                //return Json(new { key = 0, msg = "فشلت العملية" }, JsonRequestBehavior.AllowGet);

            }
            //}
            //else
            //{
            //    return Json(new { key = 0, msg = "انت مشترك بالفعل" });
            //}







        }
        //public async Task<bool> RefundRequest(int orderId, string amount)
        //{

        //    var order = await _orders.Entity.FindByIdAsync(orderId);
        //    var user = await _users.Entity.FindByIdAsync(order.FK_UserID);

        //    decimal parsed = decimal.Parse(amount);
        //    Dictionary<string, dynamic> responseData;
        //    string data = "entityId=8ac9a4cd7f924219017fabfc655569c3" +
        //                  "&amount=" + parsed.ToString("0.00") +
        //                  "&currency=SAR" +
        //                  "&paymentType=RF" +
        //                  "&merchantTransactionId=" + Guid.NewGuid().ToString() +
        //                  "&billing.street1=" + user.address + " address" +
        //                  "&billing.city=" + user.fullName + " city" +
        //                  "&billing.state=" + user.fullName + " state" +
        //                  "&billing.country=" + "SA" + //  Alpha-2 codes
        //                  "&billing.postcode=" + "11564" +
        //                  "&customer.email=" + user.Email +
        //                  "&customer.givenName=" + user.fullName +
        //                  "&customer.surname=" + user.fullName;
        //    string url = $"https://test.oppwa.com/v1/payments/{order.PaymentId}";
        //    byte[] buffer = Encoding.ASCII.GetBytes(data);
        //    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
        //    request.Method = "POST";
        //    request.Headers["Authorization"] = "Bearer OGFjZGE0Y2E3ZDJkYzQ0MTAxN2Q4ZWE1Y2I0ZTJiOTJ8SHQzM1FiQ3B5eQ==";
        //    request.ContentType = "application/x-www-form-urlencoded";
        //    Stream PostData = request.GetRequestStream();
        //    PostData.Write(buffer, 0, buffer.Length);
        //    PostData.Close();
        //    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        //    {
        //        Stream dataStream = response.GetResponseStream();
        //        StreamReader reader = new StreamReader(dataStream);
        //        responseData = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(reader.ReadToEnd());
        //        reader.Close();
        //        dataStream.Close();
        //    }

        //    var resultCode = responseData["result"]["code"].ToString();

        //    Regex successPattern = new Regex(@"(000\.000\.|000\.100\.1|000\.[36])");
        //    Regex successManuelPattern = new Regex(@"(000\.400\.0[^3]|000\.400\.100)");

        //    Match match1 = successPattern.Match(resultCode);
        //    Match match2 = successManuelPattern.Match(resultCode);

        //    if (match1.Success || match2.Success)
        //    {
        //        order.returnMoney = true;
        //        await _orders.Save();
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }

        //}
        //[HttpGet]
        //public async Task<IActionResult> AppleSubmit(string userId)
        //{


        //    //if (user.is_paid == false)
        //    //{
        //    Dictionary<string, dynamic> responseData;
        //    string data = "entityId=8ac9a4cd7f924219017fabfc655569c3";
        //    string url = "https://oppwa.com/v1/checkouts/" + checkOutId + "/payment?" + data;
        //    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
        //    request.Method = "GET";
        //    request.Headers["Authorization"] = "Bearer OGFjZGE0Y2E3ZDJkYzQ0MTAxN2Q4ZWE1Y2I0ZTJiOTJ8SHQzM1FiQ3B5eQ==";
        //    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        //    {
        //        Stream dataStream = response.GetResponseStream();
        //        StreamReader reader = new StreamReader(dataStream);
        //        responseData = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(reader.ReadToEnd());
        //        reader.Close();
        //        dataStream.Close();
        //    }

        //    var resultCode = responseData["result"]["code"].ToString();

        //    Regex successPattern = new Regex(@"(000\.000\.|000\.100\.1|000\.[36])");
        //    Regex successManuelPattern = new Regex(@"(000\.400\.0[^3]|000\.400\.100)");

        //    Match match1 = successPattern.Match(resultCode);
        //    Match match2 = successManuelPattern.Match(resultCode);



        //    if (match1.Success || match2.Success)
        //    {
        //        //user.is_paid = true;
        //        var user = await _users.Entity.FindByIdAsync(userId);
        //        user.TempPaymentId = responseData["id"];
        //        await _users.Save();
        //        return RedirectToAction("Success");
        //        //return Json(new { key = 1, msg = "تمت العملية بنجاح" }, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        return RedirectToAction("Fail");
        //        //return Json(new { key = 0, msg = "فشلت العملية" }, JsonRequestBehavior.AllowGet);

        //    }
        //    //}
        //    //else
        //    //{
        //    //    return Json(new { key = 0, msg = "انت مشترك بالفعل" });
        //    //}







        //}



        public ActionResult Success()
        {
            return View();
        }
        public ActionResult Fail()
        {
            return View();
        }

        public async Task<IActionResult> WebPayRequest(string userId, string amount, int orderId, int type)
        {


            var user = await _users.Entity.FindByIdAsync(userId);
            var order = await _orders.Entity.FindByIdAsync(orderId);

            if (user.Id == userId && order.paid == false)
            {



                string entityId, url, authorization = "";
                //test
                //entityId = "entityId=8ac7a4ca7d14ab59017d1daa7cbb0f38";
                //url = "https://test.oppwa.com/v1/checkouts";
                //authorization = "Bearer OGFjN2E0Y2E3ZDE0YWI1OTAxN2QxZGE5ODk2MTBmMzR8QUFSNEtHdEVyag==";

                //life
                entityId = "entityId=8acda4ca7d2dc441017d8ea688e52b9d";
                url = "https://oppwa.com/v1/checkouts";
                authorization = "Bearer OGFjZGE0Y2E3ZDJkYzQ0MTAxN2Q4ZWE1Y2I0ZTJiOTJ8SHQzM1FiQ3B5eQ==";



                decimal parsed = decimal.Parse(amount);
                Dictionary<string, dynamic> responseData;
                string data = entityId +
                              "&amount=" + parsed.ToString("0.00") +
                              "&currency=SAR" +
                              "&paymentType=DB" +
                              "&merchantTransactionId=" + Guid.NewGuid().ToString() +
                              "&billing.street1=" + user.address + " address" +
                              "&billing.city=" + user.fullName + " city" +
                              "&billing.state=" + user.fullName + " state" +
                              "&billing.country=" + "SA" + //  Alpha-2 codes
                              "&billing.postcode=" + "11564" +
                              "&customer.email=" + user.Email +
                              "&customer.givenName=" + user.fullName +
                              "&customer.surname=" + user.fullName;
                //string url = "https://oppwa.com/v1/checkouts";
                byte[] buffer = Encoding.ASCII.GetBytes(data);
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Method = "POST";
                //request.Headers["Authorization"] = "Bearer OGFjZGE0Y2E3ZDJkYzQ0MTAxN2Q4ZWE1Y2I0ZTJiOTJ8SHQzM1FiQ3B5eQ==";
                request.Headers["Authorization"] = authorization;
                request.ContentType = "application/x-www-form-urlencoded";
                Stream PostData = request.GetRequestStream();
                PostData.Write(buffer, 0, buffer.Length);
                PostData.Close();
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    responseData = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(reader.ReadToEnd());
                    reader.Close();
                    dataStream.Close();
                }

                var result = responseData;
                checkOutId = responseData["id"];
                ViewBag.Id = checkOutId;
                ViewBag.userId = userId;
                ViewBag.orderId = orderId;
                ViewBag.type = type;
                return View("WebPay");
            }
            else
            {
                ModelState.AddModelError(String.Empty, "تم دفع قيمة الطلب من قبل");
                return RedirectToAction("CreateOther", "OfficeOrders");
            }



        }
        [HttpGet]
        public async Task<IActionResult> WebSubmit(string userId, int orderId, int type)
        {

            var user = await _users.Entity.FindByIdAsync(userId);
            var order = await _orders.Entity.FindByIdAsync(orderId);
            if (user.Id == userId && order.paid == false)
            {


                string data, url, authorization = "";
                //test
                //data = "entityId=8ac7a4ca7d14ab59017d1daa7cbb0f38";
                //url = "https://test.oppwa.com/v1/checkouts/" + checkOutId + "/payment?" + data;

                //authorization = "Bearer OGFjN2E0Y2E3ZDE0YWI1OTAxN2QxZGE5ODk2MTBmMzR8QUFSNEtHdEVyag==";

                //life
                data = "entityid=8acda4ca7d2dc441017d8ea688e52b9d";
                url = "https://oppwa.com/v1/checkouts/" + checkOutId + "/payment?" + data;
                authorization = "bearer ogfjzge0y2e3zdjkyzq0mtaxn2q4zwe1y2i0ztjiotj8shqzm1fiq3b5eq==";


                Dictionary<string, dynamic> responseData;
                //string data = "entityId=8acda4ca7d2dc441017d8ea688e52b9d";
                //string url = "https://oppwa.com/v1/checkouts/" + checkOutId + "/payment?" + data;
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Method = "GET";
                //request.Headers["Authorization"] = "Bearer OGFjZGE0Y2E3ZDJkYzQ0MTAxN2Q4ZWE1Y2I0ZTJiOTJ8SHQzM1FiQ3B5eQ==";
                request.Headers["Authorization"] = authorization;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    responseData = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(reader.ReadToEnd());
                    reader.Close();
                    dataStream.Close();
                }

                var resultCode = responseData["result"]["code"].ToString();

                Regex successPattern = new Regex(@"(000\.000\.|000\.100\.1|000\.[36])");
                Regex successManuelPattern = new Regex(@"(000\.400\.0[^3]|000\.400\.100)");

                Match match1 = successPattern.Match(resultCode);
                Match match2 = successManuelPattern.Match(resultCode);


                ViewBag.type = type;
                if (match1.Success || match2.Success)
                {
                    order.paid = true;
                    //BaseController.SendMessage("تم ارسال طلبك رقم " + order.Id + " و كود التحقق هو " + order.CheckCode,
                    //    order.Phone);
                    await _orders.Save();

                    return RedirectToAction("WebSuccess", new { type = type });
                    //return Json(new { key = 1, msg = "تمت العملية بنجاح" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return RedirectToAction("WebFail", new { type = type });
                    //return Json(new { key = 0, msg = "فشلت العملية" }, JsonRequestBehavior.AllowGet);

                }
            }
            else
            {
                ModelState.AddModelError(String.Empty, "تم دفع قيمة الطلب من قبل");
                return RedirectToAction("CreateOther", "OfficeOrders");
            }







        }

        public async Task<IActionResult> MadaWebPayRequest(string userId, string amount, int orderId, int type)
        {


            var user = await _users.Entity.FindByIdAsync(userId);
            var order = await _orders.Entity.FindByIdAsync(orderId);

            if (user.Id == userId && order.paid == false)
            {



                string entityId, url, authorization = "";
                //test
                //entityId = "entityId=8ac7a4ca7d14ab59017d1dab86860f3d";
                //url = "https://test.oppwa.com/v1/checkouts";
                //authorization = "Bearer OGFjN2E0Y2E3ZDE0YWI1OTAxN2QxZGE5ODk2MTBmMzR8QUFSNEtHdEVyag==";

                //life
                entityId = "entityId=8acda4ca7d2dc441017d8ea736a92ba9";
                url = "https://oppwa.com/v1/checkouts";
                authorization = "Bearer OGFjZGE0Y2E3ZDJkYzQ0MTAxN2Q4ZWE1Y2I0ZTJiOTJ8SHQzM1FiQ3B5eQ==";




                decimal parsed = decimal.Parse(amount);
                Dictionary<string, dynamic> responseData;
                string data = entityId +
                              "&amount=" + parsed.ToString("0.00") +
                              "&currency=SAR" +
                              "&paymentType=DB" +
                              "&merchantTransactionId=" + Guid.NewGuid().ToString() +
                              "&billing.street1=" + user.address + " address" +
                              "&billing.city=" + user.fullName + " city" +
                              "&billing.state=" + user.fullName + " state" +
                              "&billing.country=" + "SA" + //  Alpha-2 codes
                              "&billing.postcode=" + "11564" +
                              "&customer.email=" + user.Email +
                              "&customer.givenName=" + user.fullName +
                              "&customer.surname=" + user.fullName;

                //string url = "https://oppwa.com/v1/checkouts";
                byte[] buffer = Encoding.ASCII.GetBytes(data);
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Method = "POST";
                //request.Headers["Authorization"] = "Bearer OGFjZGE0Y2E3ZDJkYzQ0MTAxN2Q4ZWE1Y2I0ZTJiOTJ8SHQzM1FiQ3B5eQ==";
                request.Headers["Authorization"] = authorization;
                request.ContentType = "application/x-www-form-urlencoded";
                Stream PostData = request.GetRequestStream();
                PostData.Write(buffer, 0, buffer.Length);
                PostData.Close();
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    responseData = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(reader.ReadToEnd());
                    reader.Close();
                    dataStream.Close();
                }

                var result = responseData;
                checkOutId = responseData["id"];
                ViewBag.Id = checkOutId;
                ViewBag.userId = userId;
                ViewBag.orderId = orderId;
                ViewBag.type = type;
                return View("MadaWebPay");
            }
            else
            {
                ModelState.AddModelError(String.Empty, "تم دفع قيمة الطلب من قبل");
                return RedirectToAction("CreateOther", "OfficeOrders");
            }



        }
        [HttpGet]
        public async Task<IActionResult> MadaWebSubmit(string userId, int orderId, int type)
        {

            var user = await _users.Entity.FindByIdAsync(userId);
            var order = await _orders.Entity.FindByIdAsync(orderId);
            if (user.Id == userId && order.paid == false)
            {





                string data, url, authorization = "";
                //test
                //data = "entityId=8ac7a4ca7d14ab59017d1dab86860f3d";
                //url = "https://test.oppwa.com/v1/checkouts/" + checkOutId + "/payment?" + data;
                //authorization = "Bearer OGFjN2E0Y2E3ZDE0YWI1OTAxN2QxZGE5ODk2MTBmMzR8QUFSNEtHdEVyag==";

                //life
                data = "entityId=8acda4ca7d2dc441017d8ea736a92ba9";
                url = "https://oppwa.com/v1/checkouts/" + checkOutId + "/payment?" + data;
                authorization = "Bearer OGFjZGE0Y2E3ZDJkYzQ0MTAxN2Q4ZWE1Y2I0ZTJiOTJ8SHQzM1FiQ3B5eQ==";


                Dictionary<string, dynamic> responseData;
                //string data = "entityId=8acda4ca7d2dc441017d8ea736a92ba9";
                //string url = "https://oppwa.com/v1/checkouts/" + checkOutId + "/payment?" + data;
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Method = "GET";
                //request.Headers["Authorization"] = "Bearer OGFjZGE0Y2E3ZDJkYzQ0MTAxN2Q4ZWE1Y2I0ZTJiOTJ8SHQzM1FiQ3B5eQ==";
                request.Headers["Authorization"] = authorization;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    responseData = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(reader.ReadToEnd());
                    reader.Close();
                    dataStream.Close();
                }

                var resultCode = responseData["result"]["code"].ToString();

                Regex successPattern = new Regex(@"(000\.000\.|000\.100\.1|000\.[36])");
                Regex successManuelPattern = new Regex(@"(000\.400\.0[^3]|000\.400\.100)");

                Match match1 = successPattern.Match(resultCode);
                Match match2 = successManuelPattern.Match(resultCode);


                ViewBag.type = type;
                if (match1.Success || match2.Success)
                {
                    order.paid = true;
                    //BaseController.SendMessage("تم ارسال طلبك رقم " + order.Id + " و كود التحقق هو " + order.CheckCode,
                    //    order.Phone);
                    await _orders.Save();


                    return RedirectToAction("WebSuccess", new { type = type });
                    //return Json(new { key = 1, msg = "تمت العملية بنجاح" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return RedirectToAction("WebFail", new { type = type });
                    //return Json(new { key = 0, msg = "فشلت العملية" }, JsonRequestBehavior.AllowGet);

                }
            }
            else
            {
                ModelState.AddModelError(String.Empty, "تم دفع قيمة الطلب من قبل");
                return RedirectToAction("CreateOther", "OfficeOrders");
            }







        }


        public ActionResult WebSuccess(int type)
        {
            ViewBag.type = type;
            return View();
        }
        public ActionResult WebFail(int type)
        {
            ViewBag.type = type;
            return View();
        }

        private async Task<bool> SendProviderNotifications(int orderId, string userId, int boutiqueId)
        {

            var order = await _orders.Entity.FindByIdAsync(orderId);
            var user = await _users.Entity.FindByIdAsync(userId);
            var provider = await _providerAditionalData.Entity.FindByIdAsync(boutiqueId);

            await _notifications.Entity.InsertRangeAsync(new List<Notifications>() {
                                                            new Notifications(){
                                                                FK_OrderID = order.ID,
                                                                FK_UserID = userId,
                                                                msgAr=$"تم ارسال طلبك رقم {order.ID} بنجاح",
                                                                msgEn=$"your order number {order.ID} sent successfully",
                                                                show = false,
                                                                date = Helper.Helper.GetCurrentDate()
                                                            },
                                                            new Notifications(){
                                                                FK_OrderID = order.ID,
                                                                FK_UserID = provider.FK_UserID,
                                                                msgAr=$"لديك طلب جديد برقم {order.ID} وف انتظار دفع العميل",
                                                                msgEn=$"you have new order number {order.ID},waitting provider to pay",
                                                                show = false,
                                                                date = Helper.Helper.GetCurrentDate()
                                                            }
                                                        });
            await _notifications.Save();

            var userDataAndIds = await _users.Entity.GetCustomAll(predicate: x => x.ProviderAditionalData.ID == boutiqueId).Select(x => new
            {
                lang = x.lang,
                ids = x.DeviceIds.Select(y => y.deviceID).ToList(),
                branchId = x.FK_BranchID,
            }).FirstOrDefaultAsync();

            var projectName = await _branches.Entity.GetCustomAll(x => x.ID == userDataAndIds.branchId).Select(x => x.nameAr + " | " + x.nameEn).FirstOrDefaultAsync();
            Helper.Helper.SendPushNotification(device_ids: userDataAndIds.ids, msg: Helper.Helper.CreatMessage(userDataAndIds.lang, $"لديك طلب جديد برقم {order.ID}", $"you have new order number {order.ID}"), order_id: order.ID,branchId:user.FK_BranchID, projectName: projectName);

            order.paid = true;
            await _orders.Save();

            return order.paid;
        }
    }
}
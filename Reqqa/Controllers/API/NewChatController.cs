using Core.Interfaces;
using Core.TableDb;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using Salony.Models.ApiDTO;
using Salony.PathUrl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Salony.Helper.Helper;
using static Salony.Enums.AllEnums;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Infrastructure;
using Salony.Models.NewChatDTO;
using Salony.Enums;
using Microsoft.AspNetCore.Http;

namespace Salony.Controllers.Api
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiExplorerSettings(GroupName = "MobileApi")]
    public class NewChatController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _HostingEnvironment;
        private readonly ApplicationDbContext _dbContext;


        public NewChatController(IWebHostEnvironment HostingEnvironment, IConfiguration configuration,
            ApplicationDbContext dbContext)
        {
            this._HostingEnvironment = HostingEnvironment;
            this._configuration = configuration;
            this._dbContext = dbContext;
        }


        [HttpPost(ApiRoutes.NewChat.ListChatUsers)]
        public async Task<ActionResult> ListChatUsers(ListChatUsersDTO model)
        {
            string userId = User.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;

            //var ListUsers = (await _dbContext.Messages.Where(x => x.SenderId == userId || x.ReceiverId == userId)
            //    .OrderByDescending(x => x.Id)
            //    .Select(x => new
            //    {
            //        Id = x.Id,
            //        OrderNumber = x.FK_OrderId,
            //        lastMsg = x.TypeMessage == (int)AllEnums.FileTypeChat.text ? x.Text : "file ...",
            //        //OfferNumber = x.FK_OfferId,
            //        UserId = x.SenderId == userId ? x.ReceiverId : x.SenderId,
            //        UserImg = x.SenderId == userId ? Helper.Helper.BaseUrlHoste + x.Receiver.Img : Helper.Helper.BaseUrlHoste + x.Sender.Img,
            //        UserName = x.SenderId == userId ? x.Receiver.FullName : x.Sender.FullName,
            //        Date = x.DateSend.ToString("dd/MM/yyyy"),
            //    }).ToListAsync()).GroupBy(x => x.OrderNumber).Select(x => x.LastOrDefault()).ToList();

            var ListUsers = await (from order in _dbContext.Orders
                                   where order.Messages.Where(m => m.SenderId == userId || m.ReceiverId == userId).Any()
                                   let message = order.Messages.OrderByDescending(m => m.Id).FirstOrDefault()
                                   select new
                                   {
                                       Id = message.Id,
                                       OrderNumber = message.FK_OrderId,
                                       lastMsg = message.TypeMessage == (int)AllEnums.FileTypeChat.text ? message.Text : "file ...",
                                       UserId = message.SenderId == userId ? message.ReceiverId : message.SenderId,
                                       UserImg = message.SenderId == userId ? Helper.Helper.BaseUrlHoste + message.Receiver.img : Helper.Helper.BaseUrlHoste + message.Sender.img,
                                       UserName = message.SenderId == userId ? message.Receiver.fullName : message.Sender.fullName,
                                       Date = message.DateSend.ToString("dd/MM/yyyy"),
                                       counter = order.Messages.Where(x => (userId == x.ReceiverId ? !x.ReceiverSeen : x.SenderSeen)).Count()
                                   }).OrderByDescending(o => o.OrderNumber).ToListAsync();

            return Json(new { key = 1, data = ListUsers });

        }

        [HttpPost(ApiRoutes.NewChat.ListMessagesUser)]
        public async Task<ActionResult> ListMessagesUser(ListMessagesUserDTO model)
        {
            string userId = User.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;
            await _dbContext.Messages.Where(x => x.ReceiverId == userId && !x.ReceiverSeen).ForEachAsync(x => x.ReceiverSeen = true);
            await _dbContext.SaveChangesAsync();

            const int maxRows = 50;

            var ListMessages = await _dbContext.Messages.Where(x => x.FK_OrderId == model.OrderId)
                .OrderByDescending(x => x.Id).Skip((model.pageNumber - 1) * maxRows).Take(maxRows)
               .Select(x => new
               {
                   Id = x.Id,
                   Type = x.TypeMessage,
                   SenderId = x.SenderId,
                   ReceiverId = x.ReceiverId,
                   Message = x.TypeMessage == (int)AllEnums.FileTypeChat.text ? x.Text : /*Helper.Helper.BaseUrlHoste +*/ x.Text,
                   Date = x.DateSend.ToString("hh:mm tt")
               }).OrderBy(x => x.Id).ToListAsync();

            return Json(new { key = 1, data = ListMessages });

        }

        [HttpPost(ApiRoutes.NewChat.UploadNewFile)]
        public ActionResult UploadFile(IFormFile File)
        {
            string FileName = Helper.Helper.ProcessUploadedFile(_HostingEnvironment, File, "ChatFiles");
            return Json(new { key = 1, data = Helper.Helper.BaseUrlHoste + FileName });
        }




    }
}
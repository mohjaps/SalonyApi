using Salony.Services;
using Salony.ViewModels.ChatViewModels;
using Infrastructure;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using static Salony.Helper.Helper;

namespace Salony.Hubs
{
    public class ChatHub : Hub
    {

        private readonly IChatServices _chatServices;
        private readonly ApplicationDbContext _dbContext;
        public static List<Listgroups> listgroups = new List<Listgroups>();
        public ChatHub(IChatServices chatServices, ApplicationDbContext dbContext)
        {
            this._chatServices = chatServices;
            this._dbContext = dbContext;
        }

        //public async Task SendMessage(NewMessageViewModel model)
        public async Task SendMessage(string SenderId, string ReceiverId, string Text, int OrderId, int Type = 0, int Duration = 0)
        {
            NewMessageViewModel model = new NewMessageViewModel()
            {
                SenderId = SenderId,
                ReceiverId = ReceiverId,
                Text = Text,
                OrderId = OrderId,
                Type = Type,
                Duration = Duration
            };
            if (!string.IsNullOrEmpty(model.SenderId) && !string.IsNullOrEmpty(model.ReceiverId) && !string.IsNullOrEmpty(model.Text))
            {
                PushMessegeViewModel data = await _chatServices.AddNewMessage(model);
                await Clients.Group(ReceiverId).SendAsync("receiveMessage", data);

                List<string> listgroupsuser = listgroups.Where(x => x.userId == ReceiverId).Select(x => x.deviceId).ToList();

                List<Tuple<string, string>> ListIds = await _dbContext.DeviceIds.Where(x => x.FK_UserID == ReceiverId && !listgroupsuser.Contains(x.deviceID)).Select(x => new Tuple<string, string>(x.deviceID, x.deviceType)).ToListAsync(); ;
                //await Clients.All.SendAsync("receiveTest", ListIds);

                if (ListIds.Count() != 0)
                {
                    var reciverdata = await _dbContext.Users.Where(x => x.Id == ReceiverId).FirstOrDefaultAsync();
                    var projectName = await _dbContext.Branches.Where(x => x.ID == reciverdata.FK_BranchID).Select(x => x.nameAr + " | " + x.nameEn).FirstOrDefaultAsync() ?? "";

                    if (reciverdata != null)
                    {
                        Helper.Helper.SendPushNotification(device_ids: ListIds, msg: Type == (int)Enums.AllEnums.FileTypeChat.text ? Text : "لديك رسالة جديدة", type: (int)Enums.AllEnums.FcmType.chat, order_id: OrderId, user_id: SenderId, user_name: reciverdata.fullName, user_img: BaseUrlHoste + reciverdata.img, closeNotify: reciverdata.closeNotification,branchId: reciverdata.FK_BranchID, projectName: projectName);
                    }
                }

            }

        }

        //To Connect in Mobile
        public async Task Connect(string userId, string deviceId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, userId);
                await Clients.All.SendAsync("connected", true);


                var userFound = listgroups.Where(x => x.userId == userId && x.deviceId == deviceId).Any();
                if (!userFound)
                {
                    listgroups.Add(new Listgroups() { userId = userId, deviceId = deviceId, connectionId = Context.ConnectionId });
                }
            }

        }

        //To DisConnect in Mobile
        public async Task DisConnect(string userId, string deviceId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
                await Clients.All.SendAsync("disconnected", false);

                var userFound = listgroups.Where(x => x.userId == userId && x.deviceId == deviceId && x.connectionId == Context.ConnectionId).FirstOrDefault();
                if (userFound != null)
                {
                    listgroups.Remove(userFound);
                }
            }
        }

        //public override async Task OnDisconnectedAsync(Exception exception)
        //{
        //    //await Groups.RemoveFromGroupAsync(Context.ConnectionId, "SignalR Users");
        //    var userFound = listgroups.Where(x => x.connectionId == Context.ConnectionId).ToList();
        //    foreach (var item in userFound)
        //    {
        //        listgroups.Remove(item);
        //    }
        //    await base.OnDisconnectedAsync(exception);
        //}

        public async Task ConnectWeb(string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
            await Clients.All.SendAsync("connected", true);
        }
    }
}

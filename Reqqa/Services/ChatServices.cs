using Salony.Enums;
using Salony.Models.ChatDTO;
using Salony.ViewModels.ChatViewModels;
using Core.TableDb;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Salony.Services
{

    public class ChatServices : IChatServices
    {
        private readonly ApplicationDbContext _context;
        public ChatServices(ApplicationDbContext dbContext)
        {
            _context = dbContext;
        }

        public async Task<PushMessegeViewModel> AddNewMessage(NewMessageViewModel model)
        {
            try
            {
                Messages newMessage = new Messages
                {
                    SenderId = model.SenderId,
                    ReceiverId = model.ReceiverId,
                    FK_OrderId = model.OrderId,
                    Text = model.Text,
                    TypeMessage = model.Type,
                    DateSend = DateTime.UtcNow.AddHours(3),
                };

                await _context.Messages.AddAsync(newMessage);
                await _context.SaveChangesAsync();

                return _context.Messages.Where(x => x.Id == newMessage.Id).Select(m => new PushMessegeViewModel
                {
                    Id = m.Id,
                    OrderId = m.FK_OrderId,
                    Type = m.TypeMessage,
                    SenderId = m.SenderId,
                    ReceiverId = m.ReceiverId,
                    SenderImg = Helper.Helper.BaseUrlHoste + m.Sender.img,
                    ReceiverImg = Helper.Helper.BaseUrlHoste + m.Receiver.img,
                    Message = m.TypeMessage == (int)AllEnums.FileTypeChat.text ? m.Text : /*Helper.Helper.BaseUrlHoste +*/ m.Text,
                    duration = m.Duration,
                    Date = m.DateSend.ToString("hh:mm tt")

                }).FirstOrDefault() ?? new PushMessegeViewModel() { };
            }
            catch (Exception ex)
            {

                return new PushMessegeViewModel() { };
            }

        }


        public async Task<List<MessageTwoUsersDto>> GetAllMessageBetweenTwoUser(string ReceiverId, string SenderId, int OrderId ,int page_number)
        {
            List<MessageTwoUsersDto> Messages = await _context.Messages.Include(x=>x.Receiver).Include(x => x.Sender).Where(x => x.FK_OrderId == OrderId).Select(x => new MessageTwoUsersDto
            {
                SenderId  =x.Sender.fullName,
                ReceiverId  =x.Receiver.fullName,
                SenderImg  =x.Sender.img,
                ReceiverImg  =x.Receiver.img,
                Date = x.DateSend.ToString("dd/MM/yyyy h:mm tt"),
                Message = x.Text,
                Type = x.TypeMessage
            }).OrderByDescending(x=>x.Id).ToListAsync();

            return Messages;
        }



        public Task<List<ConversationsDto>> ListUsersMyChat(string UserId)
        {
            throw new NotImplementedException();
        }
    }
}

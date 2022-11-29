using Salony.Models.ChatDTO;
using Salony.ViewModels.ChatViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Salony.Services
{
    public interface IChatServices
    {
        Task<PushMessegeViewModel> AddNewMessage(NewMessageViewModel model);
        Task<List<ConversationsDto>> ListUsersMyChat(string UserId);
        Task<List<MessageTwoUsersDto>> GetAllMessageBetweenTwoUser(string ReceiverId, string SenderId, int OrderId, int page_number);

    }
}

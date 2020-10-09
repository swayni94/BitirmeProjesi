using System.Collections.Generic;
using Service.DTO;

namespace Service
{
    public interface IMessageService
    {
        (bool, string) SendMessage(string messageBody, string messageTitle, string parentId, string fromUserId, string toUserId, string documentURL = null);
        (List<UserMessagesDTOModel>, string) GetUserMessageList(string userId);
        (bool, string) ReadMessage(string messageId, string userId);
        (bool, string) DeleteMessage(string messageId, string userId);
    }
}

using System.Collections.Generic;

namespace Api.Models
{
    public class UserMessageModel
    {
        public string FromUserId { get; set; }
        public string MeesageTitle { get; set; }

        public List<MessageModel> MessageList { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace Service.DTO
{
    public class UserMessagesDTOModel
    {
        public string FromUserId { get; set; }
        public string ToUserId { get; set; }
        public string MeesageTitle { get; set; }
        public int NewMessageCount { get; set; }
        public int MessageCount { get; set; }
        public DateTime LastMessageDateTime { get; set; }
        public string Title { get; set; }

        public List<MessageDTOModel> MessageList { get; set; }
    }
}

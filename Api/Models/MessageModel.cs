using System;
namespace Api.Models
{
    public class MessageModel : BaseModel
    {
        public string FromUserId { get; set; }
        public string ToUserId { get; set; }
        public string MessageTitle { get; set; }
        public string MessageBody { get; set; }
        public string DocumentURL { get; set; }
        public bool IsRead { get; set; }
        public string ParentMessageId { get; set; }
    }
}

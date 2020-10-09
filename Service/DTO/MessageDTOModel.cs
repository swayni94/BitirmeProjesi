using System;

namespace Service.DTO
{
    public class MessageDTOModel : BaseDTOModel
    {
        public string FromUserId { get; set; }
        public string ToUserId { get; set; }
        public string MessageTitle { get; set; }
        public string MessageBody { get; set; }
        public bool IsRead { get; set; }
        public string ParentMessageId { get; set; }
        public string DocumentURL { get; set; }
        public DateTime MessageDateTime { get; set; }
    }
}

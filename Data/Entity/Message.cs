using MongoDB.Bson;

namespace Data.Entity
{
    public class Message : BaseEntity
    {
        public ObjectId FromUserId { get; set; }
        public ObjectId ToUserId { get; set; }
        public string MessageTitle { get; set; }
        public string MessageBody { get; set; }
        public string DocumentURL { get; set; }
        public bool IsRead { get; set; }
        public ObjectId? ParentMessageId { get; set; }
    }
}

using MongoDB.Bson;

namespace Data.Entity
{
    public class Friend : BaseEntity
    {
        public ObjectId FromUserId { get; set; }
        public ObjectId ToUserId { get; set; }
    }
}

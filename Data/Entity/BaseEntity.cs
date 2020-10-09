using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Data.Entity
{
    public class BaseEntity
    {
        [BsonId]
        [BsonElement("Id")]
        public ObjectId Id { get; set; }

        [BsonElement("IsDeleted")]
        public bool IsDeleted { get; set; }

        [BsonElement("IsActive")]
        public bool IsActive { get; set; }

        [BsonElement("CreatedUserId")]
        public ObjectId CreatedUserId { get; set; }

        [BsonElement("CreatedDateTime")]
        public DateTime CreatedDateTime { get; set; }

        [BsonElement("UpdatedUserId")]
        public ObjectId? UpdatedUserId { get; set; }

        [BsonElement("UpdatedDateTime")]
        public DateTime? UpdatedDateTime { get; set; }
    }
}

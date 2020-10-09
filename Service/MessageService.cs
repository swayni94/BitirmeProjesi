using System.Collections.Generic;
using System.Linq;
using Data;
using Data.Entity;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Service.DTO;

namespace Service
{
    public class MessageService : IMessageService
    {
        readonly IRepository<Message> _messageRepository;
        private ILogger<MessageService> _logger;

        public MessageService(IRepository<Message> messageRepository, ILogger<MessageService> logger)
        {
            _messageRepository = messageRepository;
            _logger = logger;
        }

        public (bool, string) DeleteMessage(string messageId, string userId)
        {
            _messageRepository.Delete(ObjectId.Parse(messageId), ObjectId.Parse(userId));
            return (true, string.Empty);
        }

        public (bool, string) ReadMessage(string messageId, string userId)
        {
            var message = _messageRepository.GetFirstById(ObjectId.Parse(messageId));
            message.IsRead = true;
            _messageRepository.AddOrUpdate(message, ObjectId.Parse(userId));
            return (true, string.Empty);
        }

        public (bool, string) SendMessage(string messageBody, string messageTitle, string parentId, string fromUserId, string toUserId, string documentURL)
        {
            var message = new Message { MessageBody = messageBody, IsRead = false, FromUserId = ObjectId.Parse(fromUserId), ToUserId = ObjectId.Parse(toUserId), DocumentURL = documentURL };
            if (parentId != string.Empty)
            {
                message.ParentMessageId = ObjectId.Parse(parentId);
            }
            else
            {
                message.MessageTitle = messageTitle;
            }
            _messageRepository.AddOrUpdate(message, ObjectId.Parse(fromUserId));

            return (true, string.Empty);
        }

        public (List<UserMessagesDTOModel>, string) GetUserMessageList(string userId)
        {
            var retval = new List<UserMessagesDTOModel>();
            var userObjectId = ObjectId.Parse(userId);
            var messages = _messageRepository.GetList(w => w.IsActive && !w.IsDeleted && (w.FromUserId == userObjectId || w.ToUserId == userObjectId)).ToList();
            var messagesParentList = messages.Where(w => w.ParentMessageId == null).OrderBy(w => w.CreatedDateTime).ToList();

            foreach (var parentMessage in messagesParentList)
            {
                var parentItem = new UserMessagesDTOModel { Title = parentMessage.MessageTitle, MessageList = new List<MessageDTOModel>() };
                parentItem.MessageList.Add(new MessageDTOModel { Id = parentMessage.Id.ToString(), FromUserId = parentMessage.FromUserId.ToString(), IsRead = parentMessage.IsRead, MessageBody = parentMessage.MessageBody, MessageDateTime = parentMessage.CreatedDateTime, ToUserId = parentMessage.ToUserId.ToString(), DocumentURL = parentMessage.DocumentURL });

                var childMessage = _messageRepository.GetList(w => w.ParentMessageId == parentMessage.ParentMessageId && w.ParentMessageId.HasValue).OrderByDescending(w => w.CreatedDateTime).ToList();
                if (childMessage.Count > 0)
                {
                    parentItem.NewMessageCount = childMessage.Count(w => !w.IsRead);
                    parentItem.MessageCount = childMessage.Count();
                    parentItem.LastMessageDateTime = childMessage.Max(w => w.CreatedDateTime);
                    parentItem.MessageList.AddRange(childMessage.Select(w => new MessageDTOModel { Id = w.Id.ToString(), FromUserId = w.FromUserId.ToString(), IsRead = w.IsRead, MessageBody = w.MessageBody, MessageDateTime = w.CreatedDateTime, ToUserId = w.ToUserId.ToString(), DocumentURL = parentMessage.DocumentURL }));
                }

                retval.Add(parentItem);
            }
            
            return (retval, string.Empty);
        }
    }
}

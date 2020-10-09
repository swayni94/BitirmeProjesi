using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Api.Engine;
using Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Service;

namespace Api.Controllers
{
    public class MessageController : BaseController
    {
        private ILogger<MessageController> _logging;
        private IMessageService _messageService;
        private IWorkContext _workContext;
        private readonly IHostingEnvironment _environment;
        private readonly IHttpContextAccessor _context;

        public MessageController(ILogger<MessageController> logging, IMessageService messageService, IWorkContext workContext, IHostingEnvironment environment, IHttpContextAccessor context)
        {
            _logging = logging;
            _messageService = messageService;
            _workContext = workContext;
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _context = context;
        }

        /*
        (bool, string) SendMessage(string messageBody, string messageTitle, string parentId, string fromUserId, string toUserId);
        (List<UserMessagesDTOModel>, string) GetUserMessageList(string userId);
        (bool, string) ReadMessage(string messageId, string userId);
        (bool, string) DeleteMessage(string messageId, string userId);
        */

        [Authorize]
        [HttpPost]
        public IActionResult ReadMessage([FromBody]BaseModel model)
        {
            var (result, errorMessage) = _messageService.ReadMessage(model.Id, _workContext.UserId);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                return BadRequest(errorMessage);
            }
            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        public IActionResult DeleteMessage([FromBody]BaseModel model)
        {
            var (result, errorMessage) = _messageService.DeleteMessage(model.Id, _workContext.UserId);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                return BadRequest(errorMessage);
            }
            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        public IActionResult SendMessage([FromBody]MessageModel model)
        {
            if (!string.IsNullOrEmpty(model.DocumentURL))
            {
                model.MessageBody = "Dosya paylaşıldı.";
            }

            var (result, errorMessage) = _messageService.SendMessage(model.MessageBody, model.MessageTitle, model.ParentMessageId, _workContext.UserId, model.ToUserId, model.DocumentURL);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                return BadRequest(errorMessage);
            }
            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        public IActionResult GetUserMessageList()
        {
            var (result, errorMessage) = _messageService.GetUserMessageList(_workContext.UserId);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                return BadRequest(errorMessage);
            }

            result.ForEach(parent => parent.MessageList.ForEach(message => message.DocumentURL= string.IsNullOrEmpty(message.DocumentURL)? string.Empty : Request.Scheme +"://" + Request.Host+ Url.Content("~/Uploads")+"/"+ message.DocumentURL ));

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            var docName = Guid.NewGuid().ToString().Replace("-", string.Empty);
            var uploads = Path.Combine(_environment.WebRootPath ?? "/app/wwwroot", "Uploads");
            if(file != null)
            if (file.Length > 0)
            {
                docName +="."+file.FileName.Split('.')[1];
                using (var fileStream = new FileStream(Path.Combine(uploads, docName), FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                return Ok(docName);
            }
            return BadRequest();
        }
    }
}

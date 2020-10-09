using System.Linq;
using Api.Engine;
using Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Service;

namespace Api.Controllers
{
    public class FriendController : BaseController
    {
        private ILogger<FriendController> _logging;
        private IUserService _userService;
        private IWorkContext _workContext;
        public FriendController(ILogger<FriendController> logging, IUserService userService, IWorkContext workContext)
        {
            _logging = logging;
            _userService = userService;
            _workContext = workContext;
        }

        [Authorize]
        [HttpPost]
        public IActionResult GetFriendList()
        {
            var (friendList, errorMessage) = _userService.GetUsersFirendList(_workContext.UserId);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                return BadRequest(errorMessage);
            }
            return Ok(friendList);
        }

        [Authorize]
        [HttpPost]
        public IActionResult GetBlockFriendList()
        {
            var (blockFriendList, errorMessage) = _userService.GetBlockFriendList(_workContext.UserId);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                return BadRequest(errorMessage);
            }
            return Ok(blockFriendList);
        }

        [Authorize]
        [HttpPost]
        public IActionResult AddFriend([FromBody]BaseModel model)
        {
            var (result, errorMessage) = _userService.AddFriendList(_workContext.UserId, model.Id);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                return BadRequest(errorMessage);
            }
            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        public IActionResult BlockFriend([FromBody]BaseModel model)
        {
            var (result, errorMessage) = _userService.BlockFriend(_workContext.UserId, model.Id);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                return BadRequest(errorMessage);
            }
            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        public IActionResult ActiveFriend([FromBody]BaseModel model)
        {
            var (result, errorMessage) = _userService.ActiveFriend(_workContext.UserId, model.Id);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                return BadRequest(errorMessage);
            }
            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        public IActionResult DeleteFriend([FromBody]BaseModel model)
        {
            var (result, errorMessage) = _userService.DeleteFriend(_workContext.UserId, model.Id);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                return BadRequest(errorMessage);
            }
            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        public IActionResult SearchUser([FromBody]SearchModel model)
        {
            var (result, errorMessage) = _userService.SearchUser(model.SearchText);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                return BadRequest(errorMessage);
            }
            return Ok(result.Where(w=>w.Id != _workContext.UserId).ToList());
        }
    }
}

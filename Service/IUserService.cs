using System.Collections.Generic;
using Service.DTO;
namespace Service
{
    public interface IUserService
    {
        (UserDTOModel, string) GetUserForLogin(string userName, string password);
        (bool, string) RegisterUser(string username, string password, string firstname, string lastname, string email);
        (UserDTOModel, string) GetUser(string userId);
        (List<UserDTOModel>, string) GetUsersFirendList(string userId);
        (bool, string) AddFriendList(string fromUserId, string toUserId);
        (bool, string) BlockFriend(string fromUserId, string toUserId);
        (List<UserDTOModel>, string) GetBlockFriendList(string userId);
        (bool, string) ActiveFriend(string fromUserId, string toUserId);
        (bool, string) DeleteFriend(string fromUserId, string toUserId);
        (List<UserDTOModel>, string) SearchUser(string userName);
        (bool, string) SetUserPublicClient(string clientPublicKey, string userId);
    }
}

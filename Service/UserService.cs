using MongoDB.Bson;
using Service.DTO;
using Data.Entity;
using Data;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Collections.Generic;

namespace Service
{
    public class UserService : IUserService
    {
        readonly IRepository<User> _userRepository;
        readonly IRepository<Friend> _friendRepository;
        private ILogger<UserService> _logger;

        public UserService(IRepository<User> userRepository, IRepository<Friend> friendRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
            _friendRepository = friendRepository;
        }

        public (UserDTOModel, string) GetUserForLogin(string userName, string password)
        {
            var data = _userRepository.GetFirst(w => w.IsActive && !w.IsDeleted && w.UserName == userName && w.Password == password);
            if (data != null)
            {
                return (new UserDTOModel { UserName = data.UserName, Password = data.Password, Email = data.Email, FirstName = data.FirstName, Id = data.Id.ToString(), LastName = data.LastName }, string.Empty);
            }
            return (null, "Kullanıcı bulunamadı. Kullanıcı adı ve şifrenizi kontrol ediniz.");
        }

        public (bool, string) RegisterUser(string username, string password, string firstname, string lastname, string email)
        {
            var result = _userRepository.GetFirst(w => w.IsActive && !w.IsDeleted && w.UserName == username);
            if (result != null)
            {
                return (false, "Kullanıcı adı başka bir kullanıcı tarafından kullanılmaktadır. Farklı bir kullanıcı adı seçiniz.");
            }
            var data = new User();
            data.UserName = username;
            data.Password = password;
            data.FirstName = firstname;
            data.LastName = lastname;
            data.Email = email;
            _userRepository.AddOrUpdate(data, ObjectId.GenerateNewId());
            return (true, string.Empty);
        }

        public (List<UserDTOModel>, string) GetUsersFirendList(string userId)
        {
            var retVal = new List<UserDTOModel>();
            var userFriends = _friendRepository.GetList(w => w.FromUserId == ObjectId.Parse(userId) && w.IsActive && !w.IsDeleted).ToList();
            userFriends.ForEach(item =>
            {
                var userData = _userRepository.GetFirstById(item.ToUserId);
                retVal.Add(new UserDTOModel { Id = userData.Id.ToString(), Email = userData.Email, FirstName = userData.FirstName, LastName = userData.LastName, UserName = userData.UserName });
            });

            return (retVal.OrderBy(w => w.UserName).ToList(), null);
        }

        public (bool, string) AddFriendList(string fromUserId, string toUserId)
        {
            var friend = _friendRepository.GetFirst(w => w.IsDeleted);
            if (friend != null)
            {
                return ActiveFriend(fromUserId, toUserId);
            }
            _friendRepository.AddOrUpdate(new Friend { ToUserId = ObjectId.Parse(toUserId), FromUserId = ObjectId.Parse(fromUserId) }, ObjectId.Parse(fromUserId));
            return (true, string.Empty);
        }

        public (bool, string) BlockFriend(string fromUserId, string toUserId)
        {
            var friend = _friendRepository.GetFirst(w => !w.IsDeleted && w.IsActive);
            if (friend != null)
            {
                friend.IsActive = false;
                _friendRepository.AddOrUpdate(friend, ObjectId.Parse(fromUserId));
                return (true, string.Empty);
            }
            return (false, "Kullanıcı arkadaş listenizde bulunmuyor.");

        }

        public (List<UserDTOModel>, string) GetBlockFriendList(string userId)
        {
            var retVal = new List<UserDTOModel>();
            var userFriends = _friendRepository.GetList(w => w.FromUserId == ObjectId.Parse(userId) && !w.IsActive && !w.IsDeleted).ToList();
            userFriends.ForEach(item =>
            {
                var userData = _userRepository.GetFirstById(item.ToUserId);
                retVal.Add(new UserDTOModel { Id = userData.Id.ToString(), Email = userData.Email, FirstName = userData.FirstName, LastName = userData.LastName, UserName = userData.UserName });
            });

            return (retVal.OrderBy(w => w.UserName).ToList(), null);
        }

        public (bool, string) ActiveFriend(string fromUserId, string toUserId)
        {
            var friend = _friendRepository.GetFirst(w => !w.IsDeleted && !w.IsActive);
            if (friend != null)
            {
                friend.IsActive = true;
                _friendRepository.AddOrUpdate(friend, ObjectId.Parse(fromUserId));
                return (true, string.Empty);
            }
            return (false, "Kullanıcı block listenizde bulunmuyor.");
        }

        public (List<UserDTOModel>, string) SearchUser(string userName)
        {
            var retVal = new List<UserDTOModel>();
            var users = _userRepository.GetList(w => w.UserName.ToLower().Contains(userName.ToLower()) && w.IsActive && !w.IsDeleted).OrderBy(w => w.UserName).ToList();
            users.ForEach(userData =>
            {
                retVal.Add(new UserDTOModel { Id = userData.Id.ToString(), Email = userData.Email, FirstName = userData.FirstName, LastName = userData.LastName, UserName = userData.UserName });
            });

            return (retVal, null);
        }

        public (bool, string) DeleteFriend(string fromUserId, string toUserId)
        {
            var friend = _friendRepository.GetFirst(w => !w.IsDeleted);
            if (friend != null)
            {
                _friendRepository.Delete(friend.Id, ObjectId.Parse(fromUserId));
                return (true, string.Empty);
            }
            return (false, "Kullanıcı arkadaş listenizde bulunmuyor.");
        }

        public (UserDTOModel, string) GetUser(string userId)
        {
            var user = _userRepository.GetFirstById(ObjectId.Parse(userId));
            if (user != null)
            {
                return (new UserDTOModel { Id = user.Id.ToString(), UserName = user.UserName, Email = user.Email, FirstName = user.FirstName, LastName = user.LastName, ClientPublicKey= user.ClientPublicKey }, string.Empty);
            }
            return (null, "Kullanıcı bulunamadı.");
        }

        public (bool, string) SetUserPublicClient(string clientPublicKey, string userId)
        {
            var user = _userRepository.GetFirstById(ObjectId.Parse(userId));
            if (user != null)
            {
                user.ClientPublicKey = clientPublicKey;
                _userRepository.AddOrUpdate(user, ObjectId.Parse(userId));
                return (true, string.Empty);
            }
            return (false, "Kullanıcı bulunamadı.");
        }
    }
}

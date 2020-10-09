namespace Service.DTO
{
    public class UserDTOModel : BaseDTOModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ClientPublicKey { get; set; }
    }
}

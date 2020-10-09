namespace Api.Models
{
    public class LoginModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsRemainMe { get; set; }
        public string ClientPublicKey { get; set; }
    }
}

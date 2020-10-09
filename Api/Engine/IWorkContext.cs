using System;
namespace Api.Engine
{
    public interface IWorkContext
    {
        string UserId { get;}
        string UserName { get;}
        string FirstName { get; }
        string LastName { get; }
        string Email { get; }
        string ClientPublicKey { get; }
    }
}

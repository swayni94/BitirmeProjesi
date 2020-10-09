using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Api.Engine
{
    public class WorkContext : IWorkContext
    {
        public WorkContext(IHttpContextAccessor context)
        {
            ////IPrincipal user c
            var user = context.HttpContext.User as ClaimsPrincipal;
            if (user.Identity.IsAuthenticated)
            {
                ClaimsIdentity claimsIdentity = user.Identity as ClaimsIdentity;
                foreach (var claim in claimsIdentity.Claims)
                {
                    switch (claim.Type)
                    {
                        case "UserId": { UserId = claim.Value; break; }
                        case "UserName": { UserName = claim.Value;  break; }
                        case "FirstName": { FirstName = claim.Value; break; }
                        case "LastName": { LastName = claim.Value; break; }
                        case "Email": { Email = claim.Value; break; }
                        case "ClientPublicKey": { ClientPublicKey = claim.Value; break; }
                        default:
                            break;
                    }
                }
            }
        }

        public string UserId { get; }

        public string UserName { get; }

        public string FirstName { get; }

        public string LastName { get; }

        public string Email { get; }

        public string ClientPublicKey { get; set; }
    }
}

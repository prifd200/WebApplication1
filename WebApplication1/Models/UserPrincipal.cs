using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
namespace WebApplication1.Models
{
    public class UserPrincipal : ClaimsPrincipal
    {
        public UserPrincipal(ClaimsIdentity identity) : base(identity)
        {
        }
    }
}
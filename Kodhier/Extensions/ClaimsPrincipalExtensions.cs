using System.Linq;
using System.Security.Claims;

namespace Kodhier.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Gets the Id of a user based on claims of the given user.
        /// </summary>
        /// <param name="principal">User</param>
        /// <returns>Guid as string (ClaimTypes.NameIdentifier)</returns>
        public static string GetId(this ClaimsPrincipal principal)
        {
            string res = null;
            try
            {
                res = principal.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
            }
            catch { /* Keep result null */ }

            return res;
        }
    }
}

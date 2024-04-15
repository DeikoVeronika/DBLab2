using System.Security.Claims;

namespace BeautySpaceInfrastructure.Models
{
    public class Accessibility
    {
        public static bool AllExceptUser(ClaimsPrincipal user)
        {
            if (!user.Identity.IsAuthenticated)
            {
                return false;
            }
            else
            {
                var userRoles = user.Claims
                                 .Where(c => c.Type == ClaimTypes.Role)
                                 .Select(c => c.Value);

                return !(userRoles.Count() == 1 && userRoles.Contains("user"));
            }

        }
    }
}

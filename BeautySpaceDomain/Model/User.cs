using Microsoft.AspNetCore.Identity;

namespace BeautySpaceDomain.Model
{
    public class User : IdentityUser
    {
        public byte[]? Avatar { get; set; }

    }
}

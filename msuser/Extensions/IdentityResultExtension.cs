using Microsoft.AspNetCore.Identity;

namespace msuser.Extensions
{
    public static class IdentityResultExtension
    {
        public static List<string> GetErrors(this IdentityResult identity)
        {
            var result = new List<string>();

            foreach (var item in identity.Errors)
                result.Add(item.Description);

            return result;
        }
    }
}

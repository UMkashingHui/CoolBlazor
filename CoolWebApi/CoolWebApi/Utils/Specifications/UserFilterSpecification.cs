

using CoolWebApi.Models.Identity;
using CoolWebApi.Utils.Specifications.Base;

namespace CoolWebApi.Utils.Specifications
{
    public class UserFilterSpecification : AppSpecification<CoolBlazorUser>
    {
        public UserFilterSpecification(string searchString)
        {
            if (!string.IsNullOrEmpty(searchString))
            {
                Criteria = p => p.FirstName.Contains(searchString) || p.LastName.Contains(searchString) || p.Email.Contains(searchString) || p.PhoneNumber.Contains(searchString) || p.UserName.Contains(searchString);
            }
            else
            {
                Criteria = p => true;
            }
        }
    }
}
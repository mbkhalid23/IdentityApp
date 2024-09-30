using Microsoft.AspNetCore.Authorization;

namespace IdentityApp.Pages.Identity.Admin
{
    [Authorize(Roles = "Dashboard")]
    public class AdminPageModel : UserPageModel
    {
    }
}

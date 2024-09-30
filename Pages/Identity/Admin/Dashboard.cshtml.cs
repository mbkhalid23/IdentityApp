using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityApp.Pages.Identity.Admin
{
    public class DashboardModel : AdminPageModel
    {
        public DashboardModel(UserManager<IdentityUser> usrMgr) => UserManager = usrMgr;
        public UserManager<IdentityUser> UserManager { get; set; }
        public int UsersCount { get; set; } = 0;
        public int UsersUnconfirmed { get; set; } = 0;
        public int UsersLockedout { get; set; } = 0;
        public int UsersTwoFactor { get; set; } = 0;

        private readonly string[] emails = {
            "alice@example.com", "bob@example.com", "charlie@example.com"
        };

        public void OnGet()
        {
            UsersCount = UserManager.Users.Count();
            UsersUnconfirmed = UserManager.Users.Count(u => !u.EmailConfirmed);
            UsersLockedout = UserManager.Users
                .Count(u => u.LockoutEnabled && u.LockoutEnd > DateTimeOffset.Now);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            foreach (IdentityUser existingUser in UserManager.Users.ToList())
            {
                IdentityResult result = await UserManager.DeleteAsync(existingUser);
                result.Process(ModelState);
            }
            foreach (string email in emails)
            {
                IdentityUser userObject = new IdentityUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };
                var result = await UserManager.CreateAsync(userObject);
                if (result.Process(ModelState))
                {
                    result = await UserManager.AddPasswordAsync(userObject, "mysecret");
                    result.Process(ModelState);
                }
                result.Process(ModelState);
            }
            if(ModelState.IsValid)
                return RedirectToPage();
            return Page();
        }
    }
}
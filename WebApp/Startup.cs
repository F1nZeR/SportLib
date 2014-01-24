using System.Globalization;
using System.Threading;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using WebApp.Data;
using WebApp.Models;

[assembly: OwinStartupAttribute(typeof(WebApp.Startup))]
namespace WebApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            InitAdmin();
            app.MapSignalR();
        }

        private static void InitAdmin()
        {
            var dbContext = new DataContext();
            using (var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(dbContext)))
            {
                const string adminName = "admin";
                string userId;
                var adminUser = userManager.FindByName(adminName);
                if (adminUser == null)
                {
                    var user = new ApplicationUser { UserName = adminName };
                    userManager.Create(user, "123456");
                    userId = user.Id;
                }
                else
                {
                    userId = adminUser.Id;
                }

                using (var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(dbContext)))
                {
                    // добавляем роль Admin и туда заносим юзера admin
                    const string roleName = "Admin";
                    if (!roleManager.RoleExists(roleName))
                    {
                        roleManager.Create(new IdentityRole(roleName));
                    }

                    if (!userManager.IsInRole(userId, roleName))
                    {
                        userManager.AddToRole(userId, roleName);
                    }

                    // добавляем роль Editor
                    const string editorRoleName = "Editor";
                    if (!roleManager.RoleExists(editorRoleName))
                    {
                        roleManager.Create(new IdentityRole(editorRoleName));
                    }
                }
            }
        }
    }
}

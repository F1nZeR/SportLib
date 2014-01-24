using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using WebApp.Controllers.Base;
using WebApp.Models;

namespace WebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : BaseController
    {
        //
        // GET: /Admin/
        public ActionResult Index()
        {
            return View();
        }

        [ChildActionOnly]
        private List<ApplicationUser> GetEditors()
        {
            return DataContext.Users.Where(x => x.Roles.Any(r => r.Role.Name == "Editor")).ToList();
        }

        public ActionResult CreateEditor()
        {
            ViewBag.Editors = GetEditors();
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CreateEditor(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser {UserName = model.UserName};
                var result = await UserManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await UserManager.AddToRoleAsync(user.Id, "Editor");
                    return RedirectToAction("Index");
                }
                
                AddErrors(result);
            }
            
            ViewBag.Editors = GetEditors();
            return View(model);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
	}
}
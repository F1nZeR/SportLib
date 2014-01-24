using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using WebApp.Data;
using WebApp.Data.Manager;
using WebApp.Models;

namespace WebApp.Controllers.Base
{
    public class BaseController : Controller
    {
        private DataContext _context;
        protected DataContext DataContext
        {
            get
            {
                return _context ?? (_context = new DataContext());
            }
        }

        private DataManager _dataManager;
        protected DataManager DataManager
        {
            get
            {
                return _dataManager ?? (_dataManager = new DataManager());
            }
        }
        
        private UserManager<ApplicationUser> _userContext;
        protected UserManager<ApplicationUser> UserManager
        {
            get
            {
                return _userContext ?? (_userContext = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(DataContext)));
            }
        }
    }
}
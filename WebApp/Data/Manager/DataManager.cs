using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.Data.Manager
{
    public class DataManager
    {
        private DataContext _dataContext;
        public DataContext DataContext { get { return _dataContext ?? (_dataContext = new DataContext()); } }
    }
}
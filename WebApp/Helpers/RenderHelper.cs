using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApp.Helpers
{
    public class RenderHelper
    {
        public static string RenderPartialToString(string view, object model, ControllerContext context)
        {
            if (string.IsNullOrEmpty(view))
            {
                view = context.RouteData.GetRequiredString("action");
            }

            var viewData = new ViewDataDictionary();
            var tempData = new TempDataDictionary();

            viewData.Model = model;

            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(context, view);
                var viewContext = new ViewContext(context, viewResult.View, viewData, tempData, sw);
                viewResult.View.Render(viewContext, sw);
                return sw.GetStringBuilder().ToString();
            }
        }
    }
}
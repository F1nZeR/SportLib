using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApp.Helpers
{
    public static class HtmlPostButtonExtension
    {
        public static MvcHtmlString PostActionButton<T>(this HtmlHelper<T> html, String buttonText, String actionName, String controllerName, Object routeValues, Object buttonHtmlAttributes)
        {
            var form = new TagBuilder("form");
            form.Attributes.Add("action", new UrlHelper(html.ViewContext.RequestContext).Action(actionName,controllerName,routeValues));
            form.Attributes.Add("method", "post");
            form.Attributes.Add("class", "inline-form");
            if (actionName.Contains("Delete"))
            {
                form.Attributes.Add("onsubmit", string.Format("return confirm('Вы точно хотите {0}?');", buttonText.ToLower()));
            }
            html.ViewContext.Writer.Write(form.ToString(TagRenderMode.StartTag));
            html.ViewContext.Writer.Write(html.AntiForgeryToken());

            var tag = new TagBuilder("input");
            tag.Attributes.Add("type", "submit");
            tag.Attributes.Add("value", buttonText);
            tag.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(buttonHtmlAttributes), true);

            html.ViewContext.Writer.Write(tag.ToString(TagRenderMode.SelfClosing));
            html.ViewContext.Writer.Write(form.ToString(TagRenderMode.EndTag));

            return MvcHtmlString.Create(String.Empty);
        }
    }
}
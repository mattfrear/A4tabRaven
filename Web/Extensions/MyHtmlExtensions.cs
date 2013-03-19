using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;

namespace Web.Extensions
{
    public static class MyHtmlExtensions
    {
        public static IHtmlString UniqueStartChar(this HtmlHelper helper, string word, ref string currentStartChar)
        {
            string openTag = "<h4>";
            string closeTag = "</h4>";

            var isMobile = helper.ViewContext.HttpContext.GetOverriddenBrowser().IsMobileDevice;
            if (isMobile)
            {
                openTag = "<li data-role=\"listdivider\">";
                closeTag = "</li>";
            }

            if (!word.StartsWith(currentStartChar, StringComparison.OrdinalIgnoreCase))
            {
                currentStartChar = word.Substring(0, 1).ToUpper();
                return MvcHtmlString.Create(openTag + currentStartChar + closeTag);
            }

            return MvcHtmlString.Empty;
        }
    }
}
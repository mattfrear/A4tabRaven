using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Extensions
{
    public static class MyHtmlExtensions
    {
        public static IHtmlString UniqueStartChar(this HtmlHelper helper, string word, ref string currentStartChar)
        {
            if (!word.StartsWith(currentStartChar, StringComparison.OrdinalIgnoreCase))
            {
                currentStartChar = word.Substring(0, 1).ToUpper();
                return MvcHtmlString.Create("<h4>" + currentStartChar + "</h4>");
            }

            return MvcHtmlString.Empty;
        }
    }
}
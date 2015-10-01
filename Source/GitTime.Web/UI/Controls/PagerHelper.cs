using System;
using System.Text;
using System.Web.Mvc;

namespace GitTime.Web.UI.Controls
{
    public static class PagerHelper
    {
        #region Extensions

        public static MvcHtmlString Pager<TModel>(this HtmlHelper<TModel> htmlHelper, String multiEntityName, Int32 startRow, Int32 endRow, Int32 rowCount, Int32 pageIndex, Int32 pageCount)
        {
            Int32 startPage = Constants.VisiblePageCount * (pageIndex / Constants.VisiblePageCount);

            StringBuilder html = new StringBuilder();
            html.AppendLine(@"<div class=""row"">");
            html.AppendLine(@"<div class=""col-md-6"">");

            if (pageCount > 1)
            {
                html.AppendLine(@"<nav>");
                html.AppendLine(@"<ul class=""pagination"">");

                AddPreviousLinks(pageIndex, html);

                for (Int32 i = startPage; i < pageCount && i < startPage + Constants.VisiblePageCount; i++)
                {
                    String className = i == pageIndex ? "active" : "";

                    html.AppendFormat(@"<li class=""{0}""><a href=""#"" onclick=""_finder.changePage({1}); return false;"">{2}</a></li>", className, i, i + 1);
                    html.AppendLine();
                }

                AddNextLinks(pageIndex, pageCount, html);

                html.AppendLine(@"</ul>");
                html.AppendLine(@"</nav>");
            }

            html.AppendLine(@"</div>");
            html.AppendFormat(@"<div class=""col-md-6 page-number"">{0} {1} - {2} of {3}</div>", multiEntityName, startRow, endRow, rowCount);
            html.AppendLine();
            html.AppendLine(@"</div>");

            return new MvcHtmlString(html.ToString());
        }

        #endregion

        #region Helper methods

        private static void AddPreviousLinks(Int32 pageIndex, StringBuilder html)
        {
            Int32 startPage = Constants.VisiblePageCount * (pageIndex / Constants.VisiblePageCount);

            if (pageIndex == 0)
            {
                html.AppendLine(@"<li class=""disabled""><a href=""#"" aria-label=""Previous"" title=""Previous"" onclick=""return false;""><span aria-hidden=""true"">&laquo;</span></a></li>");
            }
            else
            {
                html.AppendFormat(
                    @"<li><a href=""#"" aria-label=""Previous"" title=""Previous"" onclick=""_finder.changePage({0}); return false;""><span aria-hidden=""true"">&laquo;</span></a></li>",
                    pageIndex - 1
                    );

                html.AppendLine();
            }

            if (startPage > 0)
                html.AppendFormat(@"<li><a href=""#"" title=""Previous Pages"" onclick=""_finder.changePage({0}); return false;"">...</a></li>", startPage - 1);
        }

        private static void AddNextLinks(Int32 pageIndex, Int32 pageCount, StringBuilder html)
        {
            Int32 startPage = Constants.VisiblePageCount * (pageIndex / Constants.VisiblePageCount);

            if (startPage + Constants.VisiblePageCount < pageCount)
                html.AppendFormat(@"<li><a href=""#"" title=""Next Pages"" onclick=""_finder.changePage({0}); return false;"">...</a></li>", startPage + Constants.VisiblePageCount);

            if (pageIndex == pageCount - 1)
            {
                html.AppendLine(@"<li class=""disabled""><a href=""#"" aria-label=""Next"" title=""Next"" onclick=""return false;""><span aria-hidden=""true"">&raquo;</span></a></li>");
            }
            else
            {
                html.AppendFormat(
                    @"<li><a href=""#"" aria-label=""Next"" title=""Next"" onclick=""_finder.changePage({0}); return false;""><span aria-hidden=""true"">&raquo;</span></a></li>",
                    pageIndex + 1
                    );

                html.AppendLine();
            }
        }

        #endregion
    }
}
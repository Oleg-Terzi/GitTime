using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;

using GitTime.Web.Models;

namespace GitTime.Web.UI.Selectors
{
    public static class CompanySelector
    {
        public static MvcHtmlString CompanySelectorFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes = null, bool validate = true)
        {
            IEnumerable<DataItem> dataSource;

            using (var db = new GitTimeContext())
            {
                dataSource = db.Companies
                    .OrderBy(c => c.Name)
                    .Select(c => new DataItem { Text = c.Name, Value = c.ID.ToString() }).ToList();
            }

            return BaseSelector.GetSelector(htmlHelper, expression, htmlAttributes, validate, true, "Select a Company...", dataSource);
        }
    }
}
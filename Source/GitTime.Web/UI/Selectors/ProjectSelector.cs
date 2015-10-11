using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;

using GitTime.Web.Models;

namespace GitTime.Web.UI.Selectors
{
    public static class ProjectSelector
    {
        #region Extensions

        public static MvcHtmlString ProjectSelectorFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, Object htmlAttributes = null, Boolean validate = true)
        {
            IEnumerable<DataItem> dataSource;

            using (var db = new GitTimeContext())
            {
                dataSource = db.Projects
                    .OrderBy(c => c.Name)
                    .Select(c => new DataItem { Text = c.Name, Value = c.ID.ToString() }).ToList();
            }

            return BaseSelector.GetSelector(htmlHelper, expression, htmlAttributes, validate, true, "Select a Project...", dataSource);
        }

        public static MvcHtmlString ProjectRepositorySelectorFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, Object htmlAttributes = null, Boolean validate = true)
        {
            IEnumerable<DataItem> dataSource;

            using (var db = new GitTimeContext())
            {
                dataSource = db.Projects
                    .OrderBy(c => c.Name)
                    .Select(c => new DataItem { Text = c.Name, Value = c.Repository }).ToList();
            }

            return BaseSelector.GetSelector(htmlHelper, expression, htmlAttributes, validate, true, "Select a Project...", dataSource);
        }

        #endregion
    }
}
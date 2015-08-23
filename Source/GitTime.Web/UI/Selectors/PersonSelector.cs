using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;

using GitTime.Web.Models;

namespace GitTime.Web.UI.Selectors
{
    public static class PersonSelector
    {
        #region Extensions

        public static MvcHtmlString PersonSelectorFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            IEnumerable<DataItem> dataSource;

            using (var db = new GitTimeContext())
            {
                dataSource = db.Persons
                    .OrderBy(c => c.FirstName).ThenBy(c => c.LastName)
                    .Select(c => new DataItem { Text = c.FirstName + " " + c.LastName, Value = c.ID.ToString() }).ToList();
            }

            return BaseSelector.GetSelector(htmlHelper, expression, true, dataSource);
        }

        #endregion
    }
}
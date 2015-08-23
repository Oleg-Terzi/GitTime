using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;

using GitTime.Web.Models;

namespace GitTime.Web.UI.Selectors
{
    public static class BaseSelector
    {
        public static MvcHtmlString GetSelector<TModel, TProperty>(HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, bool allowNull, IEnumerable<DataItem> dataSource)
        {
            string name = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(ExpressionHelper.GetExpressionText(expression));
            string id = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(ExpressionHelper.GetExpressionText(expression));
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            string selectedValue = (metadata.Model ?? "").ToString();

            var select = new TagBuilder("select");
            select.Attributes["id"] = id;
            select.Attributes["name"] = name;
            select.Attributes["class"] = "form-control";

            var options = new StringBuilder();

            if(allowNull)
                options.Append(new TagBuilder("option").ToString(TagRenderMode.Normal));

            using (var db = new GitTimeContext())
            {
                foreach (DataItem item in dataSource)
                {
                    var option = new TagBuilder("option");
                    option.Attributes["value"] = item.Value;

                    if (string.Equals(item.Value, selectedValue, StringComparison.OrdinalIgnoreCase))
                        option.Attributes["selected"] = "selected";

                    option.InnerHtml = item.Text;

                    options.Append(option.ToString(TagRenderMode.Normal));
                }
            }

            select.InnerHtml = options.ToString();
            select.MergeAttributes(htmlHelper.GetUnobtrusiveValidationAttributes(name, metadata));

            return MvcHtmlString.Create(select.ToString(TagRenderMode.Normal));
        }
    }
}
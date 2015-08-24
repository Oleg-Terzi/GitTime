using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace GitTime.Web.UI.Selectors
{
    public static class BaseSelector
    {
        public static MvcHtmlString GetSelector<TModel, TProperty>(
            HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            object htmlAttributes,
            bool validate,
            bool addEmpty,
            string emptyMessage,
            IEnumerable<DataItem> dataSource
            )
        {
            string name = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(ExpressionHelper.GetExpressionText(expression));
            string id = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(ExpressionHelper.GetExpressionText(expression));

            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            RouteValueDictionary routeValueDictionary = htmlAttributes != null ? new RouteValueDictionary(htmlAttributes) : new RouteValueDictionary();

            string selectedValue = (metadata.Model ?? "").ToString();

            var select = new TagBuilder("select");

            if (!string.IsNullOrEmpty(id))
                select.Attributes.Add("id", id);
            
            select.Attributes["name"] = name;

            foreach (var routeValue in routeValueDictionary)
                select.MergeAttribute(routeValue.Key, routeValue.Value.ToString(), true);

            if(validate)
                select.MergeAttributes(htmlHelper.GetUnobtrusiveValidationAttributes(name, metadata));

            select.InnerHtml = GetOptions(selectedValue, addEmpty, emptyMessage, dataSource);

            return MvcHtmlString.Create(select.ToString(TagRenderMode.Normal));
        }

        private static string GetOptions(string selectedValue, bool allowNull, string emptyMessage, IEnumerable<DataItem> dataSource)
        {
            var options = new StringBuilder();

            if (allowNull)
            {
                var empty = new TagBuilder("option");
                empty.InnerHtml = emptyMessage;

                options.Append(empty.ToString(TagRenderMode.Normal));
            }

            foreach (DataItem item in dataSource)
            {
                var option = new TagBuilder("option");
                option.Attributes["value"] = item.Value;

                if (string.Equals(item.Value, selectedValue, StringComparison.OrdinalIgnoreCase))
                    option.Attributes["selected"] = "selected";

                option.InnerHtml = item.Text;

                options.Append(option.ToString(TagRenderMode.Normal));
            }

            return options.ToString();
        }
    }
}
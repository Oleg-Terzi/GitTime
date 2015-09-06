using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Routing;

namespace GitTime.Web.UI.Inputs
{
    public static class InputTextWithoutValidation
    {
        public static MvcHtmlString InputTextWithoutValidationFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes = null)
        {
            string name = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(ExpressionHelper.GetExpressionText(expression));
            string id = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(ExpressionHelper.GetExpressionText(expression));

            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            RouteValueDictionary routeValueDictionary = htmlAttributes != null ? new RouteValueDictionary(htmlAttributes) : new RouteValueDictionary();

            var input = new TagBuilder("input");

            if (!string.IsNullOrEmpty(id))
                input.Attributes.Add("id", id);

            input.Attributes["name"] = name;
            input.Attributes["type"] = "text";

            foreach (var routeValue in routeValueDictionary)
                input.MergeAttribute(routeValue.Key, routeValue.Value.ToString(), true);

            if (metadata.Model != null)
                input.Attributes.Add("value", metadata.Model.ToString());

            return new MvcHtmlString(input.ToString(TagRenderMode.SelfClosing));
        }
    }
}
using System;
using System.Globalization;
using System.Web.Mvc;

namespace GitTime.Web.Binders
{
    public class DecimalModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            ValueProviderResult value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (value != null)
            {
                decimal dec;

                if (decimal.TryParse(value.AttemptedValue, NumberStyles.Any, CultureInfo.InvariantCulture, out dec))
                    return dec;

                bindingContext.ModelState.AddModelError(bindingContext.ModelName, string.Format("{0} is an invalid numeric format", value.AttemptedValue));
            }

            return base.BindModel(controllerContext, bindingContext);
        }
    }
}
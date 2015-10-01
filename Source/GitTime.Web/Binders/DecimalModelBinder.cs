using System;
using System.Globalization;
using System.Web.Mvc;

namespace GitTime.Web.Binders
{
    public class DecimalModelBinder : DefaultModelBinder
    {
        public override Object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            ValueProviderResult value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (value != null)
            {
                Decimal dec;

                if (Decimal.TryParse(value.AttemptedValue, NumberStyles.Any, CultureInfo.InvariantCulture, out dec))
                    return dec;

                bindingContext.ModelState.AddModelError(bindingContext.ModelName, String.Format("{0} is an invalid numeric format", value.AttemptedValue));
            }

            return base.BindModel(controllerContext, bindingContext);
        }
    }
}
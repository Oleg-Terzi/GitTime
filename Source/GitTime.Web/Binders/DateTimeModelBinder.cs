﻿using System;
using System.Globalization;
using System.Web.Mvc;

namespace GitTime.Web.Binders
{
    public class DateTimeModelBinder: DefaultModelBinder
    {
        public static string FormatString = "M\\/d\\/yyyy";

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            ValueProviderResult value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (value != null)
            {
                DateTime date;

                if (DateTime.TryParseExact(value.AttemptedValue, FormatString, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                    return date;

                bindingContext.ModelState.AddModelError(bindingContext.ModelName, string.Format("{0} is an invalid date format", value.AttemptedValue));
            }

            return base.BindModel(controllerContext, bindingContext);
        }
    }
}
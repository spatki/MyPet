using System;
using System.Globalization;
using System.Web.Mvc;

namespace ProcessAccelerator.WebUI.Dto
{
    public class DateTimeModelBinder : DefaultModelBinder
    {
        private string _customFormat;

        public DateTimeModelBinder(string customFormat)
        {
            _customFormat = customFormat;
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            try
            {
                DateTime dt = new DateTime();
                if (value.AttemptedValue == null || value.AttemptedValue == "")
                {
                    return null;
                }
                else
                {
                    dt = DateTime.Parse(value.AttemptedValue, CultureInfo.GetCultureInfo(_customFormat), DateTimeStyles.None);
                    return dt;
                }
            }
            catch (Exception)
            {
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, " Value '" + value.AttemptedValue + "' is not a valid date format");
                bindingContext.ModelState.SetModelValue(bindingContext.ModelName, value);
                return bindingContext.Model;
            }
        }
    }
}
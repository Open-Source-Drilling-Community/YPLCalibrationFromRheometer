using System;
using System.Threading;
using System.Threading.Tasks;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace OSDC.YPL.ModelCalibration.FromRheometer.Service
{
    public class ModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.ModelType == typeof(Double))
            {
                return new BinderTypeModelBinder(typeof(DoubleModelProvider));
            }

            return null;
        }
    }

    public class DoubleModelProvider : IModelBinder
    {
        /// <summary>
        /// Shorthand for System.Globalization.CultureInfo("en-US")
        /// </summary>
        public static CultureInfo US_CULTURE = new("en-US");

        /// <summary>
        /// Shorthand for System.Globalization.CultureInfo("nb-NO")
        /// </summary>
        public static CultureInfo NO_CULTURE = new("nb-NO");

        /// <summary>
        /// Shorthand for System.Globalization.CultureInfo("fr-FR")
        /// </summary>
        public static CultureInfo FR_CULTURE = new("fr-FR");

        /// <summary>
        /// Shorthand for System.Globalization.CultureInfo("it-IT")
        /// </summary>
        public static CultureInfo IT_CULTURE = new("it-IT");

        /// <summary>
        /// Shorthand for System.Globalization.CultureInfo("es-ES")
        /// </summary>
        public static CultureInfo SP_CULTURE = new("es-ES");

        /// <summary>
        /// Shorthand for System.Globalization.CultureInfo("en-GB")
        /// </summary>
        public static CultureInfo UK_CULTURE = new("en-GB");

        /// <summary>
        /// Shorthand for System.Globalization.CultureInfo("pt-PT")
        /// </summary>
        public static CultureInfo PT_CULTURE = new("pt-PT");

        /// <summary>
        /// Shorthand for US_CULTURE.NumberFormat
        /// </summary>
        public static NumberFormatInfo US_NUMBER_FORMAT = US_CULTURE.NumberFormat;

        /// <summary>
        /// Shorthand for NO_CULTURE.NumberFormat
        /// </summary>
        public static NumberFormatInfo NO_NUMBER_FORMAT = NO_CULTURE.NumberFormat;

        /// <summary>
        /// Shorthand for FR_CULTURE.NumberFormat
        /// </summary>
        public static NumberFormatInfo FR_NUMBER_FORMAT = FR_CULTURE.NumberFormat;

        /// <summary>
        /// Shorthand for IT_CULTURE.NumberFormat
        /// </summary>
        public static NumberFormatInfo IT_NUMBER_FORMAT = IT_CULTURE.NumberFormat;

        /// <summary>
        /// Shorthand for SP_CULTURE.NumberFormat
        /// </summary>
        public static NumberFormatInfo SP_NUMBER_FORMAT = SP_CULTURE.NumberFormat;

        /// <summary>
        /// Shorthand for UK_CULTURE.NumberFormat
        /// </summary>
        public static NumberFormatInfo UK_NUMBER_FORMAT = UK_CULTURE.NumberFormat;

        /// <summary>
        /// Shorthand for PT_CULTURE.NumberFormat
        /// </summary>
        public static NumberFormatInfo PT_NUMBER_FORMAT = PT_CULTURE.NumberFormat;

 
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }
            var modelName = bindingContext.ModelName;
            var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);
            if (valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }
            var modelState = bindingContext.ModelState;
            modelState.SetModelValue(modelName, valueProviderResult);
            var metadata = bindingContext.ModelMetadata;

            try
            {
                var culture = Thread.CurrentThread.CurrentCulture;
                string? svalue = valueProviderResult.FirstValue;
                if (svalue != null) {
                    //This is needed to convert in the right manner decimal values coming from UI, as seems they always represent the decimal separator as a period("."). 
                    //Maybe depends from browsers interpretation of decimal values posted-back to the server when they receive response without any http content-language specific indication.
                    if (TryParse((string)svalue, out double dval)) {
                        object model = dval.ToString();
                        bindingContext.Result = ModelBindingResult.Success(model);
                    }
                    else
                    {
                        modelState.TryAddModelError(
                            modelName,
                            metadata.ModelBindingMessageProvider.ValueMustNotBeNullAccessor(
                                valueProviderResult.ToString()));
                    }
                } else
                {
                    modelState.TryAddModelError(
                            modelName,
                            metadata.ModelBindingMessageProvider.ValueMustNotBeNullAccessor(
                                valueProviderResult.ToString()));
                }
            }
            catch (Exception e)
            {
                // Conversion failed.
                modelState.TryAddModelError(modelName, e, metadata);
            }
            return Task.CompletedTask;
        }

        private static bool TryParse(string value, out double result)
        {
            return double.TryParse(value, out result) ||
                   double.TryParse(value, NumberStyles.Float, US_NUMBER_FORMAT, out result) ||
                   double.TryParse(value, NumberStyles.Float, NO_NUMBER_FORMAT, out result) ||
                   double.TryParse(value, NumberStyles.Float, FR_NUMBER_FORMAT, out result) ||
                   double.TryParse(value, NumberStyles.Float, IT_NUMBER_FORMAT, out result) ||
                   double.TryParse(value, NumberStyles.Float, SP_NUMBER_FORMAT, out result) ||
                   double.TryParse(value, NumberStyles.Float, PT_NUMBER_FORMAT, out result) ||
                   double.TryParse(value, NumberStyles.Float, UK_NUMBER_FORMAT, out result);
        }

    }
}

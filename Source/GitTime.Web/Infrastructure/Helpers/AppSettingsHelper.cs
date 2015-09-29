using System;
using System.Configuration;

namespace GitTime.Web.Infrastructure.Helpers
{
    internal static class AppSettingsHelper
    {
        #region Constants

        private static class Errors
        {
            public const String DataTypeMismatchOnApplicationSetting = "The data type for the application setting value named \"{0}\" is unexpected: {2} is not {1}";
            public const String MissingApplicationSetting = "Missing application setting: {0}";
        }

        #endregion

        #region Delegates

        public delegate String GetValueHandler(String key);

        #endregion

        #region Static fields

        public static GetValueHandler GetCustomValue = null;

        #endregion

        #region Get String value

        public static String GetValue(String key, Boolean required, String defaultValue)
        {
            if (GetCustomValue != null)
            {
                var customValue = GetCustomValue(key);

                if (!String.IsNullOrEmpty(customValue))
                    return customValue;
            }

            var actualValue = ConfigurationManager.AppSettings[key];

            if (!String.IsNullOrEmpty(actualValue))
                return actualValue;

            if (required)
                throw GitTimeException.Create(Errors.MissingApplicationSetting, key);

            return defaultValue;
        }

        public static String GetValue(String key, Boolean required)
        {
            return GetValue(key, required, null);
        }

        public static String GetValue(String key)
        {
            return GetValue(key, true, null);
        }

        #endregion

        #region Get Boolean value

        public static Boolean GetValueAsBoolean(String key, Boolean required, Boolean defaultValue)
        {
            String actualValue = GetValue(key, required, null);

            if (String.IsNullOrEmpty(actualValue))
                return defaultValue;

            Boolean value;
            if (Boolean.TryParse(actualValue, out value))
                return value;

            throw GitTimeException.Create(Errors.DataTypeMismatchOnApplicationSetting, key, typeof(Boolean).Name, actualValue);
        }

        public static Boolean GetValueAsBoolean(String key, Boolean required)
        {
            return GetValueAsBoolean(key, required, false);
        }

        public static Boolean GetValueAsBoolean(String key)
        {
            return GetValueAsBoolean(key, true, false);
        }

        #endregion

        #region Get DateTime value

        public static DateTime? GetValueAsDateTime(String key, Boolean required, DateTime? defaultValue)
        {
            String actualValue = GetValue(key, required, null);

            if (String.IsNullOrEmpty(actualValue))
                return defaultValue;

            DateTime value;
            if (DateTime.TryParse(actualValue, out value))
                return value;

            throw GitTimeException.Create(Errors.DataTypeMismatchOnApplicationSetting, key, typeof(DateTime).Name, actualValue);
        }

        public static DateTime? GetValueAsDateTime(String key, Boolean required)
        {
            return GetValueAsDateTime(key, required, null);
        }

        public static DateTime? GetValueAsDateTime(String key)
        {
            return GetValueAsDateTime(key, true, null);
        }

        #endregion

        #region Get Decimal value

        public static Decimal? GetValueAsDecimal(String key, Boolean required, Decimal? defaultValue)
        {
            String actualValue = GetValue(key, required, null);

            if (String.IsNullOrEmpty(actualValue))
                return defaultValue;

            Decimal value;
            if (Decimal.TryParse(actualValue, out value))
                return value;

            throw GitTimeException.Create(Errors.DataTypeMismatchOnApplicationSetting, key, typeof(Decimal).Name, actualValue);
        }

        public static Decimal? GetValueAsDecimal(String key, Boolean required)
        {
            return GetValueAsDecimal(key, required, null);
        }

        public static Decimal? GetValueAsDecimal(String key)
        {
            return GetValueAsDecimal(key, true, null);
        }

        #endregion

        #region Get Int32 value

        public static Int32 GetValueAsInt32(String key, Boolean required, Int32 defaultValue)
        {
            String actualValue = GetValue(key, required, null);

            if (String.IsNullOrEmpty(actualValue))
                return defaultValue;

            Int32 value;
            if (Int32.TryParse(actualValue, out value))
                return value;

            throw GitTimeException.Create(Errors.DataTypeMismatchOnApplicationSetting, key, typeof(Int32).Name, actualValue);
        }

        public static Int32? GetValueAsInt32(String key, Int32? defaultValue)
        {
            var value = GetValueAsInt32(key, false, NumberHelper.Int32NotApplicable);
            if (value == NumberHelper.Int32NotApplicable)
                return defaultValue;
            return value;
        }

        public static Int32 GetValueAsInt32(String key)
        {
            return GetValueAsInt32(key, true, NumberHelper.Int32NotApplicable);
        }

        #endregion
    }
}
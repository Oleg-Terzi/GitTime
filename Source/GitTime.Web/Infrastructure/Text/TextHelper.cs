using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace GitTime.Web.Infrastructure.Text
{
    public static class TextHelper
    {
        public static String WrapSqlSpecialSymbols(String value)
        {
            return value
                .Replace("[", "[[]")
                .Replace("%", "[%]")
                .Replace("_", "[_]");
        }

        public static String ConvertListToCsvText(IEnumerable list, Boolean addQuotes)
        {
            if (list == null)
                return null;

            var result = new StringBuilder();

            foreach (Object item in list)
            {
                if (result.Length != 0)
                    result.Append(',');

                if (addQuotes)
                    result.AppendFormat("'{0}'", item.ToString().Replace("'", "''"));
                else
                    result.Append(item);
            }

            return result.ToString();
        }

        public static String ListToStringList(IEnumerable list)
        {
            if (list == null)
                return null;

            var result = new StringBuilder();

            foreach (Object value in list)
            {
                if (result.Length > 0)
                    result.Append(',');

                result.Append(value);
            }

            return result.ToString();
        }

        public static String[] CsvTextToList(String csvText)
        {
            var list = new List<String>();

            if (!String.IsNullOrEmpty(csvText))
            {
                String[] parts = csvText.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);

                foreach (String s in parts)
                {
                    String trimmed = s.Trim();

                    if (!String.IsNullOrEmpty(trimmed))
                        list.Add(trimmed);
                }
            }

            return list.ToArray();
        }

        public static Int32[] CsvTextToIntList(String csvText)
        {
            if (String.IsNullOrEmpty(csvText))
                return new Int32[] {};

            var list = new List<Int32>();

            String[] parts = csvText.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);

            foreach (String s in parts)
            {
                String trimmed = s.Trim();

                if (String.IsNullOrEmpty(trimmed))
                    continue;

                Int32 temp;

                if (Int32.TryParse(trimmed, out temp))
                    list.Add(temp);
            }

            return list.ToArray();
        }
    }
}
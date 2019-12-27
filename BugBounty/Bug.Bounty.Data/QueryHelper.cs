namespace Bug.Bounty.Data
{
    using System;
    using System.Globalization;
    using System.Reflection;
    using System.ComponentModel;
    using System.Collections.Generic;
    using System.Text;
    using Bug.Bounty.DataClasses;

    public static class QueryHelper
    {
        /// <summary>
        /// Returns mathematical operator for the given condition
        /// </summary>
        /// <param name="condition">Conditions Enum</param>
        /// <returns>Mathematical operator as string</returns>
        public static string GetConditionOperator(Condition condition)
        {
            switch (condition)
            {
                case Condition.Equals:
                    return "=";

                case Condition.GreaterThan:
                    return ">";

                case Condition.GreaterThanOrEquals:
                    return ">=";

                case Condition.LessThan:
                    return "<";

                case Condition.LessThanOrEquals:
                    return "<=";

                case Condition.NotEquals:
                    return "!=";

                case Condition.IS:
                    return " IS ";

                case Condition.IN:
                    return " IN ";

                case Condition.LIKE:
                    return " LIKE ";

                case Condition.NOTIN:
                    return " NOT IN ";

                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Checks the data for number, date and string and returns the formatted string/integer value to be added in the query
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns></returns>
        public static object GetData(object value)
        {
            object data;

            if (IsNumber(value))
            {
                data = value;
            }
            else if (value == DBNull.Value)
            {
                data = "NULL";
            }
            else if (value is DateTime)
            {
                data = "CONVERT(datetime,'" + ((DateTime)value).ToString("yyyyMMdd HH:mm:ss", new CultureInfo("en-us")) + "',112)";
            }
            else
            {
                data = (value == null || string.IsNullOrWhiteSpace(value.ToString())) ? "N'" + value + "'" : "N'" + value.ToString().Replace("'", "''") + "'";
            }

            return data;
        }

        /// <summary>
        /// Checks the data for number, date and string and returns the formatted string/integer value to be added in the query
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns></returns>
        public static object GetDataForOracle(object value, bool toLower = false)
        {
            object data;

            if (IsNumber(value))
            {
                data = value;
            }
            else if (value == DBNull.Value)
            {
                data = "NULL";
            }
            else if (value is DateTime)
            {
                data = "TO_DATE('" + ((DateTime)value).ToString("yyyyMMdd hh:mm:ss tt") + "', 'yyyymmdd hh:mi:ss pm', 'nls_date_language=american')";
            }
            else if (value is bool)
            {
                data = bool.Parse(value.ToString()) ? 1 : 0;
            }
            else
            {
                if (toLower)
                {
                    data = (value == null || string.IsNullOrWhiteSpace(value.ToString())) ? "' '" : "'" + value.ToString().Replace("'", "''").ToLower() + "'";
                }
                else
                {
                    data = (value == null || string.IsNullOrWhiteSpace(value.ToString())) ? "' '" : "'" + value.ToString().Replace("'", "''") + "'";
                }
            }

            return data;
        }

        /// <summary>
        ///     Checks if the value is a number type
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>True if the object is a number type</returns>
        public static bool IsNumber(object value)
        {
            if (value == null)
            {
                return false;
            }

            return
                value is double ||
                value is int ||
                value is short ||
                value is long ||
                value is decimal;
        }

        public static string GetDescription(Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());

            DescriptionAttribute attribute
                = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute))
                    as DescriptionAttribute;

            return attribute == null ? value.ToString() : attribute.Description;
        }

        public static string GetArrayAsString(List<string> arrValue)
        {
            var querystring = new StringBuilder();
            querystring.Append("(");

            for (var j = 0; j < arrValue.Count; j++)
            {
                if (j != 0)
                {
                    querystring.Append(",");
                }

                querystring.Append(GetData(arrValue[j]));
            }

            querystring.Append(")");

            return querystring.ToString();
        }
    }
}
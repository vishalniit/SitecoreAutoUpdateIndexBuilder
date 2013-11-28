using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AutoUpdateIndexer
{
    /// <summary>
    /// Do general validation used casually in application
    /// </summary>
    public static class ValidationHelper
    {
        /// <summary>
        /// Convert the specified object into string.
        /// if fails then return the default object
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <param name="defaultArg">The default arg.</param>
        /// <returns>Converted object into string</returns>
        public static string ValidateToString(Object argument, string defaultArg)
        {
            if (defaultArg == null)
            {
                throw new Exception("Please send default argument");
            }
            string final = defaultArg;
            try
            {
                if (argument != null)
                {
                    final = Convert.ToString(argument);
                }
            }
            catch (Exception)
            { }
            return final;
        }

        /// <summary>
        /// Convert the specified object into string.
        /// if fails then return the default object
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <param name="defaultArg">The default arg.</param>
        /// <returns>Converted object into string</returns>
        public static string ValidateToString(Object argument, string defaultArg, Boolean isLengthZeroAllowed)
        {
            string final = defaultArg;
            if (defaultArg == null)
            {
                throw new Exception("Please send default argument");
            }
            try
            {
                if (argument != null)
                {
                    final = Convert.ToString(argument);
                }
            }
            catch (Exception)
            { }
            if (final.Length == 0)
            {
                if (isLengthZeroAllowed)
                {
                    final = defaultArg;
                }
            }
            return final;
        }

        /// <summary>
        /// Convert the specified object into datetime.
        /// if fails then return the default object
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <param name="defaultArg">The default arg.</param>
        /// <returns>Converted object into datetime</returns>
        public static DateTime ValidateToDateTime(Object argument, DateTime defaultArg)
        {
            if (defaultArg == null)
            {
                throw new Exception("Please send default argument");
            }
            DateTime final = defaultArg;
            try
            {
                if (argument != null)
                {
                    final = Convert.ToDateTime(argument);
                }
            }
            catch (Exception)
            { }
            return final;
        }

        /// <summary>
        /// Convert the specified object into boolean.
        /// if fails then return the default object
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <param name="defaultArg">The default arg.</param>
        /// <returns>Converted object into boolean</returns>
        public static Boolean ValidateToBoolean(Object argument, Boolean defaultArg)
        {
            Boolean final = defaultArg;
            try
            {
                if (argument != null)
                {
                    final = Convert.ToBoolean(argument);
                }
            }
            catch (Exception)
            { }
            return final;
        }

        /// <summary>
        /// Convert the specified object into integer.
        /// if fails then return the default object
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <param name="defaultArg">The default arg.</param>
        /// <returns>Converted object into integer</returns>
        public static int ValidateToInt(Object argument, int defaultArg)
        {
            int final = defaultArg;
            try
            {
                if (argument != null)
                {
                    final = Convert.ToInt32(argument);
                }
            }
            catch (Exception)
            { }
            return final;
        }

        /// <summary>
        /// Convert the specified object into float.
        /// if fails then return the default object
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <param name="defaultArg">The default arg.</param>
        /// <returns>Converted object into flaot</returns>
        public static float ValidateToFloat(Object argument, float defaultArg)
        {
            float final = defaultArg;
            try
            {
                if (argument != null)
                {
                    final = float.Parse(argument.ToString());
                }
            }
            catch (Exception)
            { }
            return final;
        }

        /// <summary>
        /// Convert the specified object into decimal.
        /// if fails then return the default object
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <param name="defaultArg">The default arg.</param>
        /// <returns>Converted object into decimal</returns>
        public static Decimal ValidateToDecimal(Object argument, int defaultArg)
        {
            decimal final = defaultArg;
            try
            {
                if (argument != null)
                {
                    final = Convert.ToDecimal(argument);
                }
            }
            catch (Exception)
            { }
            return final;
        }

        /// <summary>
        /// Convert the specified object into double.
        /// if fails then return the default object
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <param name="defaultArg">The default arg.</param>
        /// <returns>Converted object into double</returns>
        public static Double ValidateToDouble(Object argument, double defaultArg)
        {
            double final = defaultArg;
            try
            {
                if (argument != null)
                {
                    final = Convert.ToDouble(argument);
                }
            }
            catch (Exception)
            { }
            return final;
        }

        /// <summary>
        /// Convert the specified array of string value object into list of string.       
        /// </summary>
        /// <param name="argument">The argument.</param>       
        /// <returns>Converted object into item array</returns>
        public static List<string> ConvertToList(string[] values)
        {
            List<string> list = new List<string>();
            foreach (string str in values)
            {
                list.Add(str);
            }
            return list;
        }

        /// <summary>
        /// Determines whether [is valid email address] [the specified val].
        /// </summary>
        /// <param name="val">The val.</param>
        /// <returns>
        /// 	<c>true</c> if [is valid email address] [the specified val]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValidEmailAddress(string val)
        {
            Regex regex = new Regex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
            return regex.Match(val).Success;
        }

        /// <summary>
        /// Removes the ID brace from given GUID as string.
        /// </summary>
        /// <param name="id">The GUID as string.</param>
        /// <returns>guid without braces as string</returns>
        public static string RemoveIDBrace(string id)
        {
            id = id.Replace("{", "");
            id = id.Replace("}", "");
            id = id.Replace(".x", "");
            id = id.Replace(".y", "");
            return id;
        }

        /// <summary>
        /// Converts Date in MM/DD/YYYY format
        /// Vishal Gupta 22 July 2013
        /// </summary>
        /// <param name="dt">string date in DD/MM/YYYY format</param>
        /// <returns>DateTime in MM/DD/YYYY</returns>
        public static DateTime ConvertDateStringTOMMDDYYYY(string dateString)
        {
            try
            {
                DateTime dt1;
                if (dateString.Length > 0)
                {
                    if (Convert.ToInt32(dateString.Split('/')[1]) <= 12)
                    {
                        dt1 = new DateTime(Convert.ToInt32(dateString.Split('/')[2]), Convert.ToInt32(dateString.Split('/')[1]), Convert.ToInt32(dateString.Split('/')[0]));
                        return dt1;
                    }
                    else
                    {
                        dt1 = new DateTime(Convert.ToInt32(dateString.Split('/')[2]), Convert.ToInt32(dateString.Split('/')[0]), Convert.ToInt32(dateString.Split('/')[1]));
                        return dt1;
                    }
                }
                else
                {
                    dt1 = new DateTime();
                    return dt1;
                }
            }
            catch
            {
                throw new Exception("Date format is not valid.");
            }
        }


    }
}

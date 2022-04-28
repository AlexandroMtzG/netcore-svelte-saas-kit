using System;

namespace NetcoreSaas.Domain.Extensions
{
    public static class GlobalExtensions
    {
        public const string DateFormat = "dd/MM/yyyy";

        public static DateTime GetDate<T>(this T obj)
        {
            var date = DateTime.MinValue;
            try
            {
                if (obj != null && obj.Equals(DBNull.Value) == false) return Convert.ToDateTime(obj);
            }
            catch
            {
                // ignored
            }

            return date;
        }

        public static int GetInt<T>(this T obj)
        {
            try
            {
                if (obj is decimal) return Convert.ToInt32(obj);

                if (obj is int) return Convert.ToInt32(obj);

                if (obj is string)
                {
                    var x = obj.ToString().Replace(",", "");
                    if (string.IsNullOrEmpty(obj.ToString())) return 0;
                    switch (obj.ToString())
                    {
                        case "0": return 0;
                        case "1": return 1;
                        default: return Convert.ToInt32(x);
                    }
                    //string str = new string(obj.ToString().Where(c => char.IsDigit(c)).ToArray());
                }

                if (obj != null) return obj.Equals(DBNull.Value) ? 0 : Convert.ToInt32(obj);
            }
            catch
            {
                // ignored
            }

            return 0;
        }
        
        public static decimal GetDecimal<T>(this T obj)
        {
            try
            {
                if (obj is decimal) return Convert.ToDecimal(obj);

                if (obj is int) return Convert.ToDecimal(obj);

                if (obj != null)
                {
                    if (obj.Equals(DBNull.Value)) return 0;
                    return Convert.ToDecimal(obj);
                }
            }
            catch
            {
                // ignored
            }

            return 0;
        }

        public static string GetFormattedInt<T>(this T obj)
        {
            try
            {
                var intValue = GetInt(obj);
                return intValue.ToString("###,##0");
            }
            catch
            {
                // ignored
            }

            return 0.ToString("###,##0");
        }

        public static string GetFormattedDecimal<T>(this T obj)
        {
            var intValue = GetDecimal(obj);
            try
            {
                return intValue.ToString("###,##0.00");
            }
            catch
            {
                // ignored
            }

            return 0.ToString("###,##0.00");
        }

        public static decimal ConvertToDecimal(object o)
        {
            decimal n = 0;
            try
            {
                if (o is string)
                    decimal.TryParse(o.ToString(), out n);
                else if (o != null) decimal.TryParse(o.ToString(), out n);
            }
            catch
            {
                // ignored
            }

            return n;
        }
    }
}
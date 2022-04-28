// using System;
// using System.Globalization;
// using NetcoreSaas.Domain.Enums.App.Common;
//
// namespace NetcoreSaas.Application.Helpers
// {
//     public static class DateTimeExtensions
//     {
//         public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
//         {
//             var diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
//             return dt.AddDays(-1 * diff).Date;
//         }
//     }
//     public static class DateHelper
//     {
//         public static int GetWeekNumber(DateTime time)
//         {
//             // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll 
//             // be the same week# as whatever Thursday, Friday or Saturday are,
//             // and we always get those right
//             DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
//             if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
//             {
//                 time = time.AddDays(3);
//             }
//
//             // Return the week of our adjusted day
//             return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
//         }
//
//         public static string GetDaysAgoDescription(DateTime date)
//         {
//             var strDaysAgo = "";
//             var diffDays = Convert.ToInt32((DateTime.Now - date).TotalDays);
//             var diffHours = Convert.ToInt32((DateTime.Now - date).TotalHours);
//             switch (diffDays)
//             {
//                 case 0 when diffHours > 1:
//                     return $"hace {diffHours} horas";
//                 case 0 when diffHours == 1:
//                     return $"hace 1 hora";
//                 case 1:
//                     return "Ayer";
//                 case 2:
//                     return "Antier";
//                 case -1:
//                     return "Mañana";
//                 case -2:
//                     return "Pasado mañana";
//             }
//
//             string strDescription = "Hace";
//             if (diffDays < 0)
//                 strDescription = "En";
//
//
//             // ReSharper disable once ConditionIsAlwaysTrueOrFalse
//             if (Math.Abs(diffDays) == 1)
//             {
//                 strDaysAgo = strDescription + " 1 día ";
//             }
//             else if (Math.Abs(diffDays) > 1)
//             {
//                 strDaysAgo = strDescription + " " + Math.Abs(diffDays) + " días ";
//             }
//             return strDaysAgo.Trim();
//         }
//
//         
//         public static string MonthName(int mes)
//         {
//             return mes switch
//             {
//                 1 => "Enero",
//                 2 => "Febrero",
//                 3 => "Marzo",
//                 4 => "Abril",
//                 5 => "Mayo",
//                 6 => "Junio",
//                 7 => "Julio",
//                 8 => "Agosto",
//                 9 => "Septiembre",
//                 10 => "Octubre",
//                 11 => "Noviembre",
//                 12 => "Diciembre",
//                 _ => mes.ToString()
//             };
//         }
//         public static Tuple<DateTime?, DateTime?> DatesInPeriod(DateTime date, Period period)
//         {
//             DateTime? startDate = null;
//             DateTime? endDate = null;
//
//             var lastMonth = date.AddMonths(-1);
//             var lastWeek = date.AddDays(-7);
//             switch (period)
//             {
//                 case Period.All:
//                     break;
//                 case Period.Year:
//                     startDate = new DateTime(date.Year, 1, 1);
//                     endDate = new DateTime(date.Year, 12, 31);
//                     break;
//                 case Period.Month:
//                     startDate = new DateTime(date.Year, date.Month, 1);
//                     endDate = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
//                     break;
//                 case Period.Week:
//                     startDate = date.StartOfWeek(DayOfWeek.Monday);
//                     endDate = date.StartOfWeek(DayOfWeek.Monday).AddDays(7);
//                     break;
//                 case Period.Day:
//                     startDate = date;
//                     endDate = date;
//                     break;
//                 case Period.LastYear:
//                     startDate = new DateTime(date.Year - 1, 1, 1);
//                     endDate = new DateTime(date.Year - 1, 12, 31);
//                     break;
//                 case Period.LastMonth:
//                     startDate = new DateTime(lastMonth.Year, lastMonth.Month, 1);
//                     endDate = new DateTime(lastMonth.Year, lastMonth.Month, DateTime.DaysInMonth(lastMonth.Year, lastMonth.Month));
//                     break;
//                 case Period.LastWeek:
//                     startDate = lastWeek.StartOfWeek(DayOfWeek.Monday);
//                     endDate = lastWeek.StartOfWeek(DayOfWeek.Monday).AddDays(7);
//                     break;
//                 case Period.LastDay:
//                     startDate = date.AddDays(-1);
//                     endDate = date.AddDays(-1);
//                     break;
//                 case Period.Last30Days:
//                     startDate = date.AddDays(-30);
//                     endDate = date;
//                     break;
//                 case Period.Last7Days:
//                     startDate = date.AddDays(-7);
//                     endDate = date;
//                     break;
//             }
//
//             startDate = startDate?.Date;
//             if (endDate != null)
//                 endDate = new DateTime(endDate.Value.Date.Year, endDate.Value.Date.Month, endDate.Value.Date.Day, 23,59, 59);
//             return new Tuple<DateTime?, DateTime?>(startDate, endDate);
//         }
//     }
// }
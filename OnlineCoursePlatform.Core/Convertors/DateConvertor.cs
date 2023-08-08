using System.Globalization;

namespace OnlineCoursePlatform.Core.Convertors
{
    public static class DateConvertor
    {
        public static string ToShamsi(this DateTime date)
        {
            PersianCalendar pc = new PersianCalendar();
            return pc.GetDayOfMonth(date).ToString("00") + " / " + pc.GetMonth(date).ToString("00")
                + " / " + pc.GetYear(date);
        }
    }
}

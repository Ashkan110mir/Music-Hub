using System.Globalization;
namespace Music_Website.utility

{
    public static class DateCuclutor
    {
        public static string Toshamsi(DateTime datetime)
        {
            PersianCalendar pc = new PersianCalendar();
            return pc.GetYear(datetime).ToString("0000") +"/"+ pc.GetMonth(datetime).ToString("00") +"/"+ pc.GetDayOfMonth(datetime).ToString("00");
        }
    }
}

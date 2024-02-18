namespace testCSharp.Extenstions
{
    public static class DateTimeExtensions
    {
        public static long CreateTimeStampFromDateTime(this DateTime dateTime)
        {
            return (long)(dateTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        }
    }
}

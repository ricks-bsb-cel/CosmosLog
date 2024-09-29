namespace CosmosLog.Helpers
{
    public static class DateTimeHelper
    {

        public static DateTime Now()
        {
            TimeZoneInfo sp = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");

            return TimeZoneInfo.ConvertTime(DateTime.UtcNow, sp);
        }
    }
}

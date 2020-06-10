using System;

namespace PDR.PatientBooking.Service.Date
{
    public class DateProvider : IDateProvider
    {
        public DateTimeOffset Now => DateTimeOffset.UtcNow;
    }
}

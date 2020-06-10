using System;
using System.Collections.Generic;
using System.Text;

namespace PDR.PatientBooking.Service.BookingServices.Requests
{
    public class BookingRequest
    {
        public DateTimeOffset StartTime { get; set; }

        public DateTimeOffset EndTime { get; set; }

        public long DoctorId { get; set; }

        public long PatientId { get; set; }
    }
}

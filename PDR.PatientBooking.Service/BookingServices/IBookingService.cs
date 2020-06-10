using PDR.PatientBooking.Service.BookingServices.Requests;

namespace PDR.PatientBooking.Service.BookingServices
{
    /// <summary>
    /// Provides access to booking-related functionality.
    /// </summary>
    public interface IBookingService
    {
        /// <summary>
        /// Makes a booking.
        /// </summary>
        /// <param name="bookingRequest"></param>
        void MakeBooking(BookingRequest bookingRequest);
    }
}

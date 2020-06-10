using PDR.PatientBooking.Data;
using PDR.PatientBooking.Data.Models;
using PDR.PatientBooking.Service.BookingServices.Requests;
using PDR.PatientBooking.Service.Date;
using PDR.PatientBooking.Service.DoctorServices;
using PDR.PatientBooking.Service.Validation;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Xml;

namespace PDR.PatientBooking.Service.BookingServices.Validation
{
    public class BookingRequestValidator : IBookingRequestValidator
    {
        private readonly PatientBookingContext _context;
        private readonly IDateProvider _dateProvider;

        public BookingRequestValidator(
            PatientBookingContext context,
            IDateProvider dateProvider)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dateProvider = dateProvider ?? throw new ArgumentNullException(nameof(dateProvider));
        }

        public PdrValidationResult ValidateRequest(BookingRequest request)
        {
            var now = _dateProvider.Now;
            if (request.StartTime <= now)
            {
                return new PdrValidationResult(false, "Booking requests cannot start in the past");
            }
            else if (request.StartTime > request.EndTime)
            {
                return new PdrValidationResult(false, "EndTime cannot be before StartTime");
            }
            else if (_context.Order.Any(IsDoubleBooked(request)))
            {
                return new PdrValidationResult(false, "There is an existing booking");
            }
            else
            {
                return new PdrValidationResult(true);
            }
        }

        private Expression<Func<Order, bool>> IsDoubleBooked(BookingRequest request)
        {
            return order => order.DoctorId == request.DoctorId &&
                            ((order.StartTime <= request.StartTime && order.EndTime >= request.EndTime) ||
                             (order.StartTime >= request.StartTime && order.StartTime <= request.EndTime) ||
                             (order.EndTime >= request.StartTime && order.EndTime <= request.EndTime));
        }
    }
}

using PDR.PatientBooking.Data;
using PDR.PatientBooking.Data.Models;
using PDR.PatientBooking.Service.BookingServices.Requests;
using PDR.PatientBooking.Service.BookingServices.Validation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PDR.PatientBooking.Service.BookingServices
{
    public class BookingService : IBookingService
    {
        private readonly PatientBookingContext _context;
        private readonly IBookingRequestValidator _bookingRequestValidator;

        public BookingService(
            PatientBookingContext context,
            IBookingRequestValidator bookingRequestValidator)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _bookingRequestValidator = bookingRequestValidator ?? throw new ArgumentNullException(nameof(bookingRequestValidator));
        }

        public void MakeBooking(BookingRequest bookingRequest)
        {
            var validationResult = _bookingRequestValidator.ValidateRequest(bookingRequest);

            if (!validationResult.PassedValidation)
            {
                throw new ArgumentException(validationResult.Errors.First());
            }

            var bookingId = new Guid();
            var bookingStartTime = bookingRequest.StartTime;
            var bookingEndTime = bookingRequest.EndTime;
            var bookingPatientId = bookingRequest.PatientId;
            var bookingPatient = _context.Patient.FirstOrDefault(x => x.Id == bookingRequest.PatientId);
            var bookingDoctorId = bookingRequest.DoctorId;
            var bookingDoctor = _context.Doctor.FirstOrDefault(x => x.Id == bookingRequest.DoctorId);
            var bookingSurgeryType = _context.Patient.FirstOrDefault(x => x.Id == bookingPatientId).Clinic.SurgeryType;

            var myBooking = new Order
            {
                Id = bookingId,
                StartTime = bookingStartTime.DateTime,
                EndTime = bookingEndTime.DateTime,
                PatientId = bookingPatientId,
                DoctorId = bookingDoctorId,
                Patient = bookingPatient,
                Doctor = bookingDoctor,
                SurgeryType = (int)bookingSurgeryType
            };

            _context.Order.AddRange(new List<Order> { myBooking });
            _context.SaveChanges();
        }
    }
}

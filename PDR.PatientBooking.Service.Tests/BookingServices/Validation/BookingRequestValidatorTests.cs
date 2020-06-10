using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using PDR.PatientBooking.Data;
using PDR.PatientBooking.Data.Models;
using PDR.PatientBooking.Service.BookingServices.Requests;
using PDR.PatientBooking.Service.BookingServices.Validation;
using PDR.PatientBooking.Service.Date;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDR.PatientBooking.Service.Tests.BookingServices.Validation
{
    public class BookingRequestValidatorTests
    {
        private PatientBookingContext _context;
        private static readonly DateTimeOffset Now = DateTimeOffset.Now;
        private long _doctorId = 1L;
        private IDateProvider _dateProvider;

        [SetUp]
        public void SetUp()
        {
            _context = new PatientBookingContext(new DbContextOptionsBuilder<PatientBookingContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
           
            // Add a booking at 5 ticks to 10 ticks past Now.
            _context.Order.Add(new Order
            {
                DoctorId = _doctorId,
                StartTime = Now.AddTicks(5).DateTime,
                EndTime = Now.AddTicks(10).DateTime
            });

            _context.SaveChanges();

            var dateProvider = new Mock<IDateProvider>();
            dateProvider.Setup(d => d.Now).Returns(Now);
            _dateProvider = dateProvider.Object;
        }

        [TestCase(-2, -1, false, "Booking requests cannot start in the past")]
        [TestCase(-2, 1, false, "Booking requests cannot start in the past")]
        [TestCase(1, 2, true)]
        [TestCase(2, 1, false, "EndTime cannot be before StartTime")]
        public void BookingsCannotBeInThePast(int startOffset, int endOffset, bool pass, string expectedError = null)
        {
            var requestStartTime = Now.AddTicks(startOffset);
            var requestEndTime = Now.AddTicks(endOffset);

            var validator = new BookingRequestValidator(_context, _dateProvider);

            var bookingRequest = new BookingRequest
            {
                StartTime = requestStartTime,
                EndTime = requestEndTime
            };

            var result = validator.ValidateRequest(bookingRequest);

            Assert.That(result.PassedValidation, Is.EqualTo(pass));
            if (pass)
            {
                Assert.That(result.Errors, Is.Empty);
            }
            else
            {
                Assert.That(result.Errors.Single(), Is.EqualTo(expectedError));
            }
        }

        // All existing bookings start 5 ticks after now, and end 10 ticks after now.
        [TestCase(11, 12, true)] // Should pass - booking is later than existing booking
        [TestCase(1, 2, true)] // Should pass - booking is earlier than existing booking
        [TestCase(5, 6, false, "There is an existing booking")] // Should fail - booking is within an existing booking
        [TestCase(6, 7, false, "There is an existing booking")] // Should fail - booking is within an existing booking
        [TestCase(9, 10, false, "There is an existing booking")] // Should fail - booking is within an existing booking
        [TestCase(5, 10, false, "There is an existing booking")] // Should fail - booking is within an existing booking
        [TestCase(5, 10, false, "There is an existing booking")] // Should fail - booking starts within an existing booking
        [TestCase(5, 11, false, "There is an existing booking")] // Should fail - booking starts within an existing booking
        [TestCase(6, 10, false, "There is an existing booking")] // Should fail - booking starts within an existing booking
        [TestCase(6, 11, false, "There is an existing booking")] // Should fail - booking starts within an existing booking
        [TestCase(10, 11, false, "There is an existing booking")] // Should fail - booking starts within an existing booking
        [TestCase(4, 5, false, "There is an existing booking")] // Should fail - booking ends within an existing booking
        [TestCase(4, 9, false, "There is an existing booking")] // Should fail - booking ends within an existing booking
        [TestCase(5, 9, false, "There is an existing booking")] // Should fail - booking ends within an existing booking
        [TestCase(4, 10, false, "There is an existing booking")] // Should fail - booking ends within an existing booking
        [TestCase(5, 10, false, "There is an existing booking")] // Should fail - booking ends within an existing booking
        [TestCase(4, 11, false, "There is an existing booking")] // Should fail - booking encompasses an existing booking
        public void DoctorCannotBeDoubleBooked(
            int requestStartOffset,
            int requestEndOffset,
            bool pass,
            string expectedError = default)
        {
            var requestStartTime = Now.AddTicks(requestStartOffset);
            var requestEndTime = Now.AddTicks(requestEndOffset);

            var validator = new BookingRequestValidator(_context, _dateProvider);

            var bookingRequest = new BookingRequest
            {
                StartTime = requestStartTime,
                EndTime = requestEndTime,
                DoctorId = _doctorId
            };

            var result = validator.ValidateRequest(bookingRequest);

            Assert.That(result.PassedValidation, Is.EqualTo(pass));

            if (pass)
            {
                Assert.That(result.Errors, Is.Empty);
            }
            else
            {
                Assert.That(result.Errors.Single(), Is.EqualTo(expectedError));
            }
        }
    }
}

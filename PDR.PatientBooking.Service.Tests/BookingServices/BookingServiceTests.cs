using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using PDR.PatientBooking.Data;
using PDR.PatientBooking.Data.Models;
using PDR.PatientBooking.Service.BookingServices;
using PDR.PatientBooking.Service.BookingServices.Requests;
using PDR.PatientBooking.Service.BookingServices.Validation;
using PDR.PatientBooking.Service.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace PDR.PatientBooking.Service.Tests.BookingServices
{
    public class BookingServiceTests
    {
        private static readonly long _doctorId = 1;
        private static readonly long _patientId = 2;

        private PatientBookingContext _context;

        [SetUp]
        public void Setup()
        {
            _context = new PatientBookingContext(new DbContextOptionsBuilder<PatientBookingContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            _context.Doctor.Add(new Doctor
            {
                Id = _doctorId,
                FirstName = "John",
                LastName = "Dorian"
            });

            _context.Patient.Add(new Patient
            {
                Id = _patientId,
                Clinic = new Clinic
                {
                    Name = "The clinic"
                },
                FirstName = "Poorly",
                LastName = "Man"
            });

            _context.SaveChanges();
        }

        [Test]
        public void BookingServiceSavesBookingIfValidationPasses()
        {
            var now = DateTime.Now;
            var bookingRequest = new BookingRequest
            {
                DoctorId = _doctorId,
                PatientId = _patientId,
                StartTime = now
            };

            var validator = new Mock<IBookingRequestValidator>();
            validator.Setup(r => r.ValidateRequest(bookingRequest)).Returns(new PdrValidationResult(true));
            var service = new BookingService(_context, validator.Object);

            service.MakeBooking(bookingRequest);

            validator.Verify(r => r.ValidateRequest(bookingRequest), Times.Once);

            Assert.That(_context.Order.Count(b => b.StartTime == now), Is.EqualTo(1));
        }

        [Test]
        public void BookingServiceThrowsExceptionAndDoesNotSaveBookingIfValidationPasses()
        {
            var now = DateTime.Now;
            var bookingRequest = new BookingRequest
            {
                DoctorId = _doctorId,
                PatientId = _patientId,
                StartTime = now
            };

            var expectedError = "the validation error";
            var validator = new Mock<IBookingRequestValidator>();
            validator.Setup(r => r.ValidateRequest(bookingRequest)).Returns(new PdrValidationResult(false, expectedError));
            var service = new BookingService(_context, validator.Object);

            var exception = Assert.Throws<ArgumentException>(() => service.MakeBooking(bookingRequest));

            Assert.That(exception.Message, Is.EqualTo(expectedError));

            validator.Verify(r => r.ValidateRequest(bookingRequest), Times.Once);

            Assert.That(_context.Order.Where(b => b.StartTime == now), Is.Empty);
        }
    }
}

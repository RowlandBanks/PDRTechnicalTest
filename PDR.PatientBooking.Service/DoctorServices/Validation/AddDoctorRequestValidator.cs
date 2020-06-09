﻿using PDR.PatientBooking.Data;
using PDR.PatientBooking.Service.DoctorServices.Requests;
using PDR.PatientBooking.Service.Validation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PDR.PatientBooking.Service.DoctorServices.Validation
{
    public class AddDoctorRequestValidator : IAddDoctorRequestValidator
    {
        private readonly PatientBookingContext _context;
        private readonly IEmailValidator _emailValidator;

        public AddDoctorRequestValidator(PatientBookingContext context, IEmailValidator emailValidator)
        {
            _context = context;
            _emailValidator = emailValidator ?? throw new ArgumentNullException(nameof(emailValidator));
        }

        public PdrValidationResult ValidateRequest(AddDoctorRequest request)
        {
            var result = new PdrValidationResult(true);

            if (MissingRequiredFields(request, ref result))
                return result;

            if (DoctorAlreadyInDb(request, ref result))
                return result;

            if (EmailAddressNotValid(request, ref result))
                return result;

            return result;
        }

        private bool MissingRequiredFields(AddDoctorRequest request, ref PdrValidationResult result)
        {
            var errors = new List<string>();

            if (string.IsNullOrEmpty(request.FirstName))
                errors.Add("FirstName must be populated");

            if (string.IsNullOrEmpty(request.LastName))
                errors.Add("LastName must be populated");

            if (string.IsNullOrEmpty(request.Email))
                errors.Add("Email must be populated");

            if (errors.Any())
            {
                result.PassedValidation = false;
                result.Errors.AddRange(errors);
                return true;
            }

            return false;
        }

        private bool DoctorAlreadyInDb(AddDoctorRequest request, ref PdrValidationResult result)
        {
            if (_context.Doctor.Any(x => x.Email == request.Email))
            {
                result.PassedValidation = false;
                result.Errors.Add("A doctor with that email address already exists");
                return true;
            }

            return false;
        }

        private bool EmailAddressNotValid(AddDoctorRequest request, ref PdrValidationResult result)
        {
            if (!_emailValidator.EmailAddressIsValid(request.Email, out var failureReason))
            {
                result.PassedValidation = false;
                result.Errors.Add(failureReason);
                return true;
            }

            return false;
        }
    }
}

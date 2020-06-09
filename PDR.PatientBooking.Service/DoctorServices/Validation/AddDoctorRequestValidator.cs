using PDR.PatientBooking.Data;
using PDR.PatientBooking.Service.DoctorServices.Requests;
using PDR.PatientBooking.Service.Validation;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PDR.PatientBooking.Service.DoctorServices.Validation
{
    public class AddDoctorRequestValidator : IAddDoctorRequestValidator
    {
        private readonly PatientBookingContext _context;

        private static readonly Regex EmailValidationRegex = new Regex("[^@]+@[^@]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public AddDoctorRequestValidator(PatientBookingContext context)
        {
            _context = context;
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
            // RCB: Email validation is actually really hard - email addresses can contain comments, 
            //      IP address literals, off-beat domain names like .museum, and all sorts of wierd and 
            //      wonderful edge-cases. The gold-standard for email validation is to just send an email
            //      and have the user respond in some way to prove it is both a valid email address and a 
            //      live email address that the user owns.
            //
            //      However, when capturing an email address initially, a basic sanity check can be useful
            //      to capture the case where the user has input their phone number (for example) into the 
            //      email box.
            //
            //      For the purposes of the test, I have merely implemented sufficient code to make the
            //      tests pass.
            var errors = new List<string>();

            if (!EmailValidationRegex.IsMatch(request.Email))
            {
                result.PassedValidation = false;
                result.Errors.Add("Email must be a valid email address");
                return true;
            }


            return false;
        }
    }
}

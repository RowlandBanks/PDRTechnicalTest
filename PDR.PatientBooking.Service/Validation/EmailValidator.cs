using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace PDR.PatientBooking.Service.Validation
{
    public class EmailValidator : IEmailValidator
    {
        private static readonly Regex EmailValidationRegex = new Regex("[^@]+@[^@]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public bool EmailAddressIsValid(string emailAddress, out string failureReason)
        {
            // RCB: Email validation is actually really hard - email addresses can contain comments, 
            //      IP address literals, off-beat domain names like .museum, and all sorts of weird and 
            //      wonderful edge-cases. The gold-standard for email validation is to just send an email
            //      and have the user respond in some way to prove it is both a valid email address and a 
            //      live email address that the user owns.
            //
            //      However, when capturing an email address initially, a basic sanity check can be useful
            //      to capture the case where the user has input their phone number (for example) into the 
            //      email box.
            //
            //      For the purposes of the test, I have merely implemented sufficient code to make the
            //      relevant tests pass.
            if (!EmailValidationRegex.IsMatch(emailAddress))
            {
                failureReason = "Email must be a valid email address";
                return false;
            }
            else
            {
                failureReason = null;
                return true;
            }
        }
    }
}

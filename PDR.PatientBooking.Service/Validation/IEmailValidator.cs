namespace PDR.PatientBooking.Service.Validation
{
    public interface IEmailValidator
    {
        bool EmailAddressIsValid(string emailAddress, out string failureReason);
    }
}

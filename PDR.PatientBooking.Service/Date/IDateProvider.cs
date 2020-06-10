using System;

namespace PDR.PatientBooking.Service.Date
{
    /// <summary>
    /// Provides access to the current date/time.
    /// </summary>
    public interface IDateProvider
    {
        /// <summary>
        /// Gets the current date/time as a <see cref="DateTimeOffset"/>.
        /// </summary>
        DateTimeOffset Now { get; }
    }
}

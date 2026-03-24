using System;

namespace PersonalFinanceTracker.Application.Exceptions
{
    public class ConflictException : Exception
    {
        public ConflictException(string message) : base(message) { }
    }
}

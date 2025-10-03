using System;

namespace ExpenseTracker.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException() { }
        public NotFoundException(string message) : base(message) { }
    }

    public class ConflictException : Exception
    {
        public ConflictException() { }
        public ConflictException(string message) : base(message) { }
    }
}

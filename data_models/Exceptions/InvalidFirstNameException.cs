using System;

namespace data_models.Exceptions
{
    public class InvalidFirstNameException : Exception
    {
        public InvalidFirstNameException()
        {
        }

        public InvalidFirstNameException(string message)
            : base(message)
        {
        }

        public InvalidFirstNameException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}

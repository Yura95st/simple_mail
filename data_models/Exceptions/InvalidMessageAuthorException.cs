using System;

namespace data_models.Exceptions
{
    public class InvalidMessageAuthorException : Exception
    {
        public InvalidMessageAuthorException()
        {
        }

        public InvalidMessageAuthorException(string message)
            : base(message)
        {
        }

        public InvalidMessageAuthorException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
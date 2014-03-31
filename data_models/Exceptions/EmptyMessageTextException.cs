using System;

namespace data_models.Exceptions
{
    public class EmptyMessageTextException : Exception
    {
        public EmptyMessageTextException()
        {
        }

        public EmptyMessageTextException(string message)
            : base(message)
        {
        }

        public EmptyMessageTextException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
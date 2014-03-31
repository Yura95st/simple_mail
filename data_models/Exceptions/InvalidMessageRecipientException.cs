using System;

namespace data_models.Exceptions
{
    public class InvalidMessageRecipientException : Exception
    {
        public InvalidMessageRecipientException()
        {
        }

        public InvalidMessageRecipientException(string message)
            : base(message)
        {
        }

        public InvalidMessageRecipientException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
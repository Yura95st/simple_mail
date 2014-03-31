using System;

namespace data_models.Exceptions
{
    public class EmtpyMessageSubjectException : Exception
    {
        public EmtpyMessageSubjectException()
        {
        }

        public EmtpyMessageSubjectException(string message)
            : base(message)
        {
        }

        public EmtpyMessageSubjectException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
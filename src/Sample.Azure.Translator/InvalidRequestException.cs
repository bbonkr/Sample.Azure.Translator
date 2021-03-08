using System;

namespace Sample.Azure.Translator
{
    public abstract class InvalidRequestException : Exception
    {
        public InvalidRequestException(string message) : base(message) { }

        public abstract object GetDetails();
    }

    public class InvalidRequestException<T> : InvalidRequestException where T : class
    {
        public InvalidRequestException(string message, T details) : base(message)
        {
            this.Details = details;
        }

        public T Details { get; init; }

        public override T GetDetails()
        {
            return Details;
        }
    }
}

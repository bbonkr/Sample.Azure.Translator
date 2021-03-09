using System;
using System.Net;

namespace Sample.Azure.Translator
{
    public abstract class HttpStatusException : Exception
    {
        public HttpStatusException(HttpStatusCode httpStatusCode, string message)
            : base(message)
        {
            this.StatusCode = httpStatusCode;
        }
        public HttpStatusException(int httpStatusCode, string message)
            : this((HttpStatusCode)httpStatusCode, message) { }

        public HttpStatusCode StatusCode { get; init; }

        public abstract object GetDetails();

        public abstract T GetDetails<T>();
    }

    public class HttpStatusException<TDetail> : HttpStatusException
    {
        public HttpStatusException(HttpStatusCode httpStatusCode, string message, TDetail details)
            : base(httpStatusCode, message)
        {
            this.Details = details;
        }

        public HttpStatusException(int httpStatusCode, string message, TDetail details)
            : this((HttpStatusCode)httpStatusCode, message, details) { }

        public TDetail Details { get; init; }

        public override object GetDetails()
        {
            return Details;
        }

        public override T GetDetails<T>()
        {
            return (T)GetDetails();
        }
    }
}

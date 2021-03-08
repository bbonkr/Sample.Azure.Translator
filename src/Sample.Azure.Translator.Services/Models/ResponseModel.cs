using System.Net;

namespace Sample.Azure.Translator.Models
{
    public abstract class ResponseModel
    {
        public int StatusCode { get; init; }
        public string Message { get; init; }
    }

    public class ResponseModel<T> : ResponseModel
    {
        public T Data { get; init; }
    }

    public static class ResponseModelFactory
    {
        public static ResponseModel Create(int statusCode, string message)
        {
            //return new ResponseModel
            //{
            //    StatusCode = statusCode,
            //    Message = message,
            //};

            return Create<object>(statusCode, message, default(object));
        }

        public static ResponseModel Create(HttpStatusCode statusCode, string message)
        {
            return Create((int)statusCode, message);
        }

        public static ResponseModel<T> Create<T>(int statusCode, string message, T data)
        {
            return new ResponseModel<T>
            {
                StatusCode = statusCode,
                Message = message,
                Data = data,
            };
        }

        public static ResponseModel<T> Create<T>(HttpStatusCode statusCode, string message, T data)
        {
            return Create<T>((int)statusCode, message, data);
        }
    }
}

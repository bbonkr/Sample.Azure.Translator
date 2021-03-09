using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Azure.Translator
{
    public abstract class SomethingWrongException : Exception
    {
        public SomethingWrongException(string message) : base(message) { }

        public abstract object GetDetails();

        public abstract T GetDetails<T>();
    }

    public class SomethingWrongException<T> : SomethingWrongException
    {
        public SomethingWrongException(string message, T details) : base(message)
        {
            this.Details = details;
        }

        public T Details { get; init; }

        public override object GetDetails()
        {
            return Details;
        }
        public override TDetail GetDetails<TDetail>()
        {
            return (TDetail)GetDetails();
        }
    }
}

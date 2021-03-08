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
    }

    public class SomethingWrongException<T> : SomethingWrongException where T : class
    {
        public SomethingWrongException(string message, T details) : base(message)
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

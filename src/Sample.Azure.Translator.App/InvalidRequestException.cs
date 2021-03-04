using System;

namespace Sample.Azure.Translator.App
{
    public class InvalidRequestException:Exception
    {
        public InvalidRequestException(string message) :base(message) { }
    }
}

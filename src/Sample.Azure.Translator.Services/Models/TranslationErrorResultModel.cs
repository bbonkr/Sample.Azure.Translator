namespace Sample.Azure.Translator.Models
{
    public class TranslationErrorResultModel
    {
        public ErrorModel Error { get; init; }
    }

    public class ErrorModel
    {
        public int Code { get; init; }

        public string Message { get; init; }
    }
}

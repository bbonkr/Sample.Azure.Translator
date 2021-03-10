namespace Sample.Azure.Translator.Services.Models
{
    public class DocumentTranslationResponseModel
    {
        /// <summary>
        /// Translation job id
        /// </summary>
        public string Id { get; set; }
    }

    public class DocumentTranslationErrorResponseModel
    {
        public ErrorModel<string> Error { get; set; }
    }
}

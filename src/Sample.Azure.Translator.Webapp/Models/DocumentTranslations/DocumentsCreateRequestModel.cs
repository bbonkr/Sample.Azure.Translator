using System;
using System.Collections.Generic;

namespace Sample.Azure.Translator.Webapp.Models.DocumentTranslations
{
    public record TranslateRequestModel(string Name, string CriteriaLanguage, IEnumerable<string> TargetLanguages);

    public record TranslateResponseModel(string Url);
}

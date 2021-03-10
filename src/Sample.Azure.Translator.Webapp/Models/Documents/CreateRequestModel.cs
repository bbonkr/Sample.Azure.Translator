using System.Collections.Generic;

using Microsoft.AspNetCore.Http;

namespace Sample.Azure.Translator.Webapp.Models.Documents
{
    public record CreateRequestModel(string Name, string ContentType, string Contents, IList<IFormFile> Files);
}

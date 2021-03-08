using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

namespace Sample.Azure.Translator.Webapp.Models
{
    public class Documents
    {
        public record CreateRequestModel(string Name, string ContentType, string Contents, IList<IFormFile> Files);
    }
}

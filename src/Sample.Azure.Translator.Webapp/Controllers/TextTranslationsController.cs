
using kr.bbon.AspNetCore.Mvc;

using Microsoft.AspNetCore.Mvc;
using kr.bbon.AspNetCore;
using kr.bbon.AspNetCore.Filters;
using kr.bbon.Azure.Translator.Services;
using System.Threading.Tasks;
using kr.bbon.Azure.Translator.Services.Models.TextTranslation.TranslationRequest;
using System.Collections.Generic;

namespace Sample.Azure.Translator.Webapp.Controllers
{
    [ApiVersion("1.1")]
    [ApiController]
    [Area(DefaultValues.AreaName)]
    [Route(DefaultValues.RouteTemplate)]
    [ApiExceptionHandlerFilter]
    public class TextTranslationsController : ApiControllerBase
    {
        public TextTranslationsController(ITextTranslatorService translatorService)
        {
            this.translatorService = translatorService;
        }

        
        [HttpPost]
        [Produces(typeof(IEnumerable<kr.bbon.Azure.Translator.Services.Models.TextTranslation.TranslationRequest.ResponseModel>))]
        public async Task<IActionResult> Translate(RequestModel model)
        {
            var result = await translatorService.TranslateAsync(model);

            return StatusCode(System.Net.HttpStatusCode.OK, result);
        }

        private readonly ITextTranslatorService translatorService;
    }
}

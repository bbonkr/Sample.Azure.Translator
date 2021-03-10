using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sample.Azure.Translator
{
    public static class ObjectExtensions
    {
        public static string ToJson<T>(this T obj, JsonSerializerOptions options = null)
        {
            var actualOptions = options ?? new JsonSerializerOptions
            {
                WriteIndented = true,
                //Encoder = JavaScriptEncoder.Create(UnicodeRanges.All, UnicodeRanges.Cyrillic),
                // ! Caution
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };

            return JsonSerializer.Serialize<T>(obj, actualOptions);
        }
    }
}

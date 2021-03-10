using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using kr.bbon.Azure.Translator.Services.Models;
using kr.bbon.Azure.Translator.Services.Models.AzureStorage.Blob;

namespace Sample.Azure.Translator.Webapp
{
    public static class BlobCreateResultModelExtension
    {
        public static string GetNameWithoutExtension(this BlobCreateResultModel source)
        {
            var tokens = source.BlobName.Split('.');

            return string.Join('.', tokens.Take(tokens.Length - 1));
        }

        public static string GetExtension(this BlobCreateResultModel source)
        {
            return $".{source.BlobName.Split('.').Last()}";
        }
    }
}

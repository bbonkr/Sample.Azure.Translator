using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Azure.Translator.Services.Strategies
{
    public interface ITranslatedDocumentNamingStrategy
    {
        //string GetUrl(string uri, string languageCode);

        //Uri GetUri(string uri, string languageCode);

        string GetTranslatedDocumentName(string name, string languageCode);
    }

    public class TranslatedDocumentNamingStrategy: ITranslatedDocumentNamingStrategy
    {
        //public string GetUrl(string uri, string languageCode)
        //{
        //    var p = new Uri(uri);
        //    var baseUrl = $"{p.Scheme}://{p.Host}:{p.Port}";
        //    var tokens = p.AbsolutePath.Split('/');

        //    var pathWithoutFileName = string.Join('/', tokens.Take(tokens.Length - 1));
        //    var fileName = tokens.Last();

        //    var newFileName = $"{GetNameWithoutExtension(fileName)}.{languageCode}{GetExtension(fileName)}";

        //    return $"{baseUrl}/{pathWithoutFileName}/{newFileName}";
        //}

        //public Uri GetUri(string uri, string languageCode)
        //{
        //    return new Uri(GetUrl(uri, languageCode));
        //}

        public string GetTranslatedDocumentName(string name, string languageCode)
        {
            var (fileName, extension) = GetNameToken(name);
            var delimiter = string.IsNullOrWhiteSpace(extension) ? "" : ".";
            
            return $"{fileName}{delimiter}{extension}";
        }

        private (string FileName, string Extension) GetNameToken(string name)
        {
            var tokens = name.Split('.');

            return (
                FileName: tokens.Length > 1 ? string.Join('.', tokens.Take(tokens.Length - 1)) : tokens.FirstOrDefault(),
                Extension: tokens.Length > 1 ? tokens.Last() : ""
                );
        }

        //private string GetNameWithoutExtension(string fileName)
        //{
        //    var tokens = fileName.Split('.');

        //    return string.Join('.', tokens.Take(tokens.Length - 1));
        //}

        //private string GetExtension(string fileName)
        //{
        //    return $"{fileName.Split('.').Last()}";
        //}
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Azure.Translator.App
{
    public class AzureTranslatorConnectionOptions
    {
        public static string Name = "Translator";

        public string Endpoint { get; set; }

        public string SubscriptionKey { get; set; }

        public string Region { get; set; }
    }
}

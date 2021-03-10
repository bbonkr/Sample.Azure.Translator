using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Azure.Translator.Services.Models
{
    public class BlobCreateResultModel
    {
        public string ContainerName { get; set; }

        public string BlobName { get; set; }

        public string Uri { get; set; }
    }
}

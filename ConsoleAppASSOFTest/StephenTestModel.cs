using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using Microsoft.Azure.Search;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Configuration;
using System.Configuration.Assemblies;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Azure.Search.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsoleAppASSOFTest
{
    [SerializePropertyNamesAsCamelCase]
    public class StephenTestModel
    {
        [System.ComponentModel.DataAnnotations.Key]
        [IsFilterable]
        public string fileId { get; set; }
        [IsSearchable]
        public string fileText { get; set; }

        public string blobURL { get; set; }
        [IsSearchable]
        public string keyPhrases { get; set; }

    }
}

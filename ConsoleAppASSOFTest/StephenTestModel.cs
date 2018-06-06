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

namespace ConsoleAppASSOFTest
{
    public class StephenTestModel
    {
        [System.ComponentModel.DataAnnotations.Key]
        [IsFilterable]
        public string fieldId { get; set; }
        [IsSearchable]
        public string fileText { get; set; }

        public string blobURl { get; set; }
        [IsSearchable]
        public string keyPhrase { get; set; }

        string searchServiceName = ConfigurationManager.AppSettings["SearchServiceName"];
        string adminApiKey = ConfigurationManager.AppSettings["SearchServiceAdminApiKey"];
        SearchServiceClient serviceClient = new SearchServiceClient(searchServiceName, new SearchCredentials(adminApiKey));

        var dataSource = DataSource.AzureBlobStorage("storage name", "connectstrong", "container name");
        //create data source
        if (serviceClient.DataSources.Exists(dataSource.Name))
        {
            serviceClient.DataSources.Delete(dataSource.Name);
        }

        serviceClient.DataSources.Create(dataSource);


        var definition = new Index()
        {
            Name = "stephensindex",
            Fields = FieldBuilder.BuildForType<StephenTestModel>()
        };
    //create Index
    if (serviceClient.Indexes.Exists(definition.Name))
    {
        serviceClient.Indexes.Delete(definition.Name);
    }
var index = serviceClient.Indexes.Create(definition);
    }
}

using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppASSOFTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string searchServiceName = ConfigurationManager.AppSettings["SearchServiceName"];
            string adminApiKey = ConfigurationManager.AppSettings["SearchServiceAdminApiKey"];
            SearchServiceClient serviceClient = new SearchServiceClient(searchServiceName, new SearchCredentials(adminApiKey));

            var dataSource = DataSource.AzureBlobStorage("ronanblobstorage", );
            //create data source
            if (serviceClient.DataSources.Exists(dataSource.Name))
            {
                serviceClient.DataSources.Delete(dataSource.Name);
            }

            serviceClient.DataSources.Create(dataSource);

            var definition = new Index()
            {
                Name = "carindex",
                Fields = FieldBuilder.BuildForType<StephenTestModel>()
            };
            //create Index
            if (serviceClient.Indexes.Exists(definition.Name))
            {
                serviceClient.Indexes.Delete(definition.Name);
            }
            var index = serviceClient.Indexes.Create(definition);

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse();
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference("contactjson");
            var blobList = container.ListBlobs();

            var stephensIndexsList = blobList.Select(blob => new StephenTestModel
            {
                fileId = Guid.NewGuid().ToString(),
                blobURL = blob.Uri.ToString(),
                fileText = "Blob Content",
                keyPhrases = "key phrases",
            }).ToList();
            var batch = IndexBatch.Upload(stephensIndexsList);
            ISearchIndexClient indexClient = serviceClient.Indexes.GetClient("carindex");
            indexClient.Documents.Index(batch);
            Console.WriteLine("Index created....");
            Console.ReadLine();
        }
    }
}


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

            var dataSource = DataSource.AzureBlobStorage("ronanblobstorage", "DefaultEndpointsProtocol=https;AccountName=ronanblobstorage;AccountKey=Z8nPhGaeduop96WgvZ2FP6QKGNjXeN/G8RmyoJtX97Cycusq5WOaIymucItSNnrv31ChYMICh04nmyHbCmySYw==;EndpointSuffix=core.windows.net", "contactjson");
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

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=ronanblobstorage;AccountKey=Z8nPhGaeduop96WgvZ2FP6QKGNjXeN/G8RmyoJtX97Cycusq5WOaIymucItSNnrv31ChYMICh04nmyHbCmySYw==;EndpointSuffix=core.windows.net");
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
            ISearchIndexClient indexClient = serviceClient.Indexes.GetClient("index");
            indexClient.Documents.Index(batch);
        }
    }
}


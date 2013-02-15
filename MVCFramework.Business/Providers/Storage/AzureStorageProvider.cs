using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;
using MVCFramework.Business.Exceptions;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;

namespace MVCFramework.Business.Providers.Storage
{
    public class AzureStorageProvider : StorageProviderBase
    {
        private string StorageConnectionString { get; set; }
        private CloudStorageAccount StorageAccount { get; set; }

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            base.Initialize(name, config);

            if (config["storageConnectionString"] == null)
                throw new System.Configuration.Provider.ProviderException(
                    "AzureStorageLoggingProvider: No storage connection string specified.");

            StorageConnectionString = config["storageConnectionString"];

            StorageAccount = CloudStorageAccount.Parse(StorageConnectionString);
        }

        #region Queue

        private CloudQueue GetQueue(string name)
        {
            CloudQueueClient queueStorage = StorageAccount.CreateCloudQueueClient();
            var queue = queueStorage.GetQueueReference(name);
            queue.CreateIfNotExists();

            return queue;
        }

        public override void AddEventToQueue(string queue, IQueueMessage message)
        {
            CloudQueue cloudQueue = GetQueue(queue);

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            CloudQueueMessage cloudMessage = new CloudQueueMessage(serializer.Serialize(message));

            cloudQueue.AddMessage(cloudMessage);
        }

        public override T GetEventFromQueue<T>(string queue)
        {
            CloudQueue cloudQueue = GetQueue(queue);
            CloudQueueMessage cloudMessage = cloudQueue.GetMessage();

            if (cloudMessage != null)
            {
                T message;
                JavaScriptSerializer serializer = new JavaScriptSerializer();

                try
                {
                    message = serializer.Deserialize<T>(cloudMessage.AsString);
                }
                catch (Exception ex)
                {
                    throw new BusinessException("Could not add deserialize event message.", ex);
                }

                message.MessageId = cloudMessage.Id;
                message.PopReceipt = cloudMessage.PopReceipt;
                message.DequeueCount = cloudMessage.DequeueCount;

                return message;
            }

            return default(T);
        }

        public override void RemoveEventFromQueue(string queue, IQueueMessage message)
        {
            CloudQueue cloudQueue = GetQueue(queue);
            cloudQueue.DeleteMessage(message.MessageId, message.PopReceipt);
        }

        #endregion

        #region Files

        public override void UploadFile(Stream source, string path)
        {
            string containerName = GetStoragePathContainer(path);

            CloudBlobClient client = StorageAccount.CreateCloudBlobClient();

            CloudBlobContainer container = client.GetContainerReference(containerName);
            container.CreateIfNotExists();

            var blob = client.GetBlobReferenceFromServer(new Uri(path));
            source.Seek(0, SeekOrigin.Begin);
            blob.UploadFromStream(source);
        }

        public override Stream DownloadFile(string path)
        {
            CloudBlobClient client = StorageAccount.CreateCloudBlobClient();
            var blob = client.GetBlobReferenceFromServer(new Uri(path));

            Stream stream = new MemoryStream();
            blob.DownloadToStream(stream);

            return stream;
        }

        public override uint DeleteFiles(IEnumerable<string> paths)
        {
            uint deleted = 0;
            CloudBlobClient client = StorageAccount.CreateCloudBlobClient();

            foreach (var path in paths)
            {
                var blob = client.GetBlobReferenceFromServer(new Uri(path));
                if (blob.DeleteIfExists())
                    deleted++;
            }

            return deleted;
        }

        public override Uri GetFileUri(string path)
        {
            CloudBlobClient client = StorageAccount.CreateCloudBlobClient();
            var blob = client.GetBlobReferenceFromServer(new Uri(path));

            return blob.Uri;
        }

        public override Uri GetFileAccessUri(string path, AccessPolicy policy, TimeSpan valid)
        {
            CloudBlobClient client = StorageAccount.CreateCloudBlobClient();
            var blob = client.GetBlobReferenceFromServer(new Uri(path));
            string signiature = blob.GetSharedAccessSignature(new SharedAccessBlobPolicy()
            {
                SharedAccessStartTime = DateTime.Now,
                SharedAccessExpiryTime = DateTime.Now.Add(valid)
            }, policy.ToString());

            return new Uri(string.Format("{0}{1}", blob.Uri.AbsoluteUri, signiature));
        }

        #endregion

        public override bool EnsureAccessPolicy(string path, AccessPolicy policy)
        {
            CloudBlobClient client = StorageAccount.CreateCloudBlobClient();
            string containerName = GetStoragePathContainer(path);
            CloudBlobContainer container = client.GetContainerReference(containerName);

            try
            {
                bool persist = false;

                BlobContainerPermissions permissions = container.GetPermissions();
                if (policy == AccessPolicy.None && permissions.SharedAccessPolicies.Count > 0) // remove all policies if any was set
                {
                    permissions.SharedAccessPolicies.Clear();
                    persist = true;
                }
                else
                {
                    if ((policy & AccessPolicy.Read) == AccessPolicy.Read && !permissions.SharedAccessPolicies.ContainsKey(AccessPolicy.Read.ToString())) // add read policy if does not exist
                    {
                        permissions.SharedAccessPolicies.Add(AccessPolicy.Read.ToString(), new SharedAccessBlobPolicy() { Permissions = SharedAccessBlobPermissions.Read });
                        persist = true;
                    }

                    if ((policy & AccessPolicy.Write) == AccessPolicy.Write && !permissions.SharedAccessPolicies.ContainsKey(AccessPolicy.Write.ToString())) // add write policy if does not exist
                    {
                        permissions.SharedAccessPolicies.Add(AccessPolicy.Write.ToString(), new SharedAccessBlobPolicy() { Permissions = SharedAccessBlobPermissions.Write });
                        persist = true;
                    }

                    // other policies would be List and Delete, but for now they are not used by this provider
                }

                if (persist) // save shared accesspolicies
                    container.SetPermissions(permissions, AccessCondition.GenerateIfMatchCondition(container.Properties.ETag));
            }
            catch (Exception)
            {
                //TODO: log exception
                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns the container portion of the path or full path if path is already a container.
        /// </summary>
        private string GetStoragePathContainer(string path)
        {
            int end = path.IndexOf('/');

            if (end < 0) // the path is a container in itself
                return path;

            return path.Substring(0, end);
        }
    }
}

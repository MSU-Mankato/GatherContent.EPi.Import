using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Data;
using EPiServer.Data.Dynamic;

namespace GcEPiPlugin.modules.GatherContentImport.GcDynamicClasses
{
    [EPiServerDataStore(AutomaticallyRemapStore = true)]
    public class GcDynamicCredentials : IDynamicData
    {
        public Identity Id { get; set; }
        public string Email { get; set; }
        public string ApiKey { get; set; }
        public string AccountId { get; set; }

        //Parameter-less constructor required for the dynamic data store.
        public GcDynamicCredentials()
        {
            // Generate a new ID.
            Id = Identity.NewIdentity(Guid.NewGuid());
            Email = string.Empty;
            ApiKey = string.Empty;
            AccountId = string.Empty;
        }
        public GcDynamicCredentials(string email, string apiKey, string accountId)
        {
            //Assign the properties with actual values.
            Email = email;
            ApiKey = apiKey;
            Id = Identity.NewIdentity(Guid.NewGuid());
            AccountId = accountId;
        }
        //Save the Credentials.
        public static void SaveStore(GcDynamicCredentials dds)
        {
            // Create a data store (but only if one doesn't exist, we won't overwrite an existing one)
            var store = DynamicDataStoreFactory.Instance.CreateStore(typeof(GcDynamicCredentials));
            store.DeleteAll();
            store.Save(dds);
        }
        public static List<GcDynamicCredentials> RetrieveStore()
        {
            var store = DynamicDataStoreFactory.Instance.CreateStore(typeof(GcDynamicCredentials));
            var stores = store.Items<GcDynamicCredentials>();
            return stores.ToList();
        }
        //Deletes all the data in the data store.
        public static void ClearStore()
        {
            var store = DynamicDataStoreFactory.Instance.CreateStore(typeof(GcDynamicCredentials));
            store.DeleteAll();
        }
    }
}
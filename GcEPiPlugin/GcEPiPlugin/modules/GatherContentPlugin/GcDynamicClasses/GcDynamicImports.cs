using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Data;
using EPiServer.Data.Dynamic;

namespace GcEPiPlugin.modules.GatherContentPlugin.GcDynamicClasses
{
    [EPiServerDataStore(AutomaticallyRemapStore = true)]
    public class GcDynamicImports : IDynamicData
    {
        public Identity Id { get; set; }
        public Guid ContentGuid { get; set; }
        public int ItemId { get; set; }
        public DateTime ImportedAt { get; set; }

        //Parameter-less constructor required for the dynamic data store.
        public GcDynamicImports()
        {
            // Generate a new ID.
            Id = Identity.NewIdentity(Guid.NewGuid());
            ContentGuid = new Guid();
            ItemId = 0;
            ImportedAt = new DateTime();
        }

        public GcDynamicImports(Guid contentGuid, int itemId, DateTime importedAt)
        {
            //Assign the properties with actual values.
            Id = Identity.NewIdentity(Guid.NewGuid());
            ContentGuid = contentGuid;
            ItemId = itemId;
            ImportedAt = importedAt;
        }

        //Save the Credentials.
        public static void SaveStore(GcDynamicImports dds)
        {
            // Create a data store (but only if one doesn't exist, we won't overwrite an existing one)
            var store = DynamicDataStoreFactory.Instance.CreateStore(typeof(GcDynamicImports));
            store.Save(dds);
        }

        public static List<GcDynamicImports> RetrieveStore()
        {
            var store = DynamicDataStoreFactory.Instance.CreateStore(typeof(GcDynamicImports));
            var stores = store.Items<GcDynamicImports>();
            return stores.ToList();
        }

        //Deletes all the data in the data store.
        public static void ClearStore()
        {
            var store = DynamicDataStoreFactory.Instance.CreateStore(typeof(GcDynamicImports));
            store.DeleteAll();
        }
        //Delete a specific item from the data store.
        public static void DeleteItem(Identity id)
        {
            var store = DynamicDataStoreFactory.Instance.CreateStore(typeof(GcDynamicImports));
            store.Delete(id);
        }
    }
}
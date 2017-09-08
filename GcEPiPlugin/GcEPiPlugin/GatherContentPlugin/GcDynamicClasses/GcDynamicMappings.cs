using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Internal;
using EPiServer.Data;
using EPiServer.Data.Dynamic;

namespace GcEPiPlugin.GatherContentPlugin.GcDynamicClasses
{
    [EPiServerDataStore(AutomaticallyRemapStore = true)]
    public class GcDynamicMappings : IDynamicData
    {
        public Identity Id { get; set; }
        public List<GcDynamicSettings> Mappings { get; set; }

        //Parameter-less constructor required for the dynamic data store.
        public GcDynamicMappings()
        {
            // Generate a new ID.
            Id = Identity.NewIdentity(Guid.NewGuid());
            Mappings = new List<GcDynamicSettings>();
        }
        public GcDynamicMappings(List<GcDynamicSettings> mappings)
        {
            // Generate a new ID.
            Id = Identity.NewIdentity(Guid.NewGuid());
            Mappings = mappings.IsNullOrEmpty() ? new List<GcDynamicSettings>() : mappings;
        }
        public static void SaveStore(GcDynamicMappings dds)
        {
            // Create a data store (but only if one doesn't exist, we won't overwrite an existing one)
            var store = DynamicDataStoreFactory.Instance.CreateStore(typeof(GcDynamicMappings));
            store.Save(dds);
        }
        public static List<GcDynamicMappings> RetrieveStore()
        {
            var store = DynamicDataStoreFactory.Instance.CreateStore(typeof(GcDynamicMappings));
            var stores = store.Items<GcDynamicMappings>();
            return stores.ToList();
        }
        //Deletes all the data in the data store.
        public static void ClearStore()
        {
            var store = DynamicDataStoreFactory.Instance.CreateStore(typeof(GcDynamicMappings));
            store.DeleteAll();
        }
    }
}
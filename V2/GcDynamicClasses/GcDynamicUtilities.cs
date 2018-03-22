using System.Collections.Generic;
using System.Linq;
using EPiServer.Data;
using EPiServer.Data.Dynamic;

namespace GatherContentImport.GcDynamicClasses
{
    [EPiServerDataStore(AutomaticallyRemapStore = true)]
    public static class GcDynamicUtilities
    {
        //Save the Data Store.
        public static void SaveStore<T>(T dds)
        {
            // Create a data store (but only if one doesn't exist, we won't overwrite an existing one)
            var store = DynamicDataStoreFactory.Instance.CreateStore(typeof(T));

            // If the data store is for credentials, we want to replace the existing credentials with the new one.
            // Because, only one set of credentials need to be in the data store at a time.
            if (typeof(T) == typeof(GcDynamicCredentials)) ClearStore<GcDynamicCredentials>();
            store.Save(dds);
        }
        public static List<T> RetrieveStore<T>()
        {
            var store = DynamicDataStoreFactory.Instance.CreateStore(typeof(T));
            var stores = store.Items<T>();
            return stores.ToList();
        }
        //Deletes all the items in the data store.
        public static void ClearStore<T>()
        {
            var store = DynamicDataStoreFactory.Instance.CreateStore(typeof(T));
            store.DeleteAll();
        }
        //Delete a specific item from the data store.
        public static void DeleteItem<T>(Identity id)
        {
            var store = DynamicDataStoreFactory.Instance.CreateStore(typeof(T));
            store.Delete(id);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using EPiServer.Data;
using EPiServer.Data.Dynamic;

namespace GcEPiPlugin.GatherContentPlugin.GcDynamicClasses
{
    [EPiServerDataStore(AutomaticallyRemapStore = true)]
    public class GcDynamicSettings : IDynamicData
    {
        public Identity Id { get; set; }
        public string AccountId { get; set; }
        public string ProjectId { get; set; }
        public string TemplateId { get; set; }
        //Parameter-less constructor required for the dynamic data store.
        public GcDynamicSettings()
        {
            // Generate a new ID.
            Id = Identity.NewIdentity(Guid.NewGuid());
            AccountId = string.Empty;
            ProjectId = string.Empty;
            TemplateId = string.Empty;
        }                                                                                                                                                                                                                                                                                                                   

        public GcDynamicSettings([Optional] string accountId, [Optional] string projectId, [Optional] string templateId)
        {
            //Assign the properties with actual values.
            Id = Identity.NewIdentity(Guid.NewGuid());
            if (accountId != null)
                AccountId = accountId;
            else
            {
                AccountId = RetrieveStore().Count > 0 ? RetrieveStore().ToList()[0].AccountId : string.Empty;
            }
            if (projectId != null)
                ProjectId = projectId;
            else
            {
                ProjectId = RetrieveStore().Count > 0 ? RetrieveStore().ToList()[0].ProjectId : string.Empty;
            }
            if (templateId != null)
                TemplateId = templateId;
            else
            {
                TemplateId = RetrieveStore().Count > 0 ? RetrieveStore().ToList()[0].TemplateId : string.Empty;
            }
        }
        public static void SaveStore(GcDynamicSettings dds)
        {
            // Create a data store (but only if one doesn't exist, we won't overwrite an existing one)
            var store = DynamicDataStoreFactory.Instance.CreateStore(typeof(GcDynamicSettings));
            store.DeleteAll();
            store.Save(dds);
        }
        public static List<GcDynamicSettings> RetrieveStore()
        {
            var store = DynamicDataStoreFactory.Instance.CreateStore(typeof(GcDynamicSettings));
            var stores = store.Items<GcDynamicSettings>();
            return stores.ToList();
        }
        //Deletes all the data in the data store.
        public static void ClearStore()
        {
            var store = DynamicDataStoreFactory.Instance.CreateStore(typeof(GcDynamicSettings));
            store.DeleteAll();
        }
    }
}                                                                                                                                                                                                                                                                                                       
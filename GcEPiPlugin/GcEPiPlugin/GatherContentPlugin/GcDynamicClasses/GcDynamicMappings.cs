using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EPiServer.Data;
using EPiServer.Data.Dynamic;
using GcEPiPlugin.GatherContentPlugin.GcEpiObjects;

namespace GcEPiPlugin.GatherContentPlugin.GcDynamicClasses
{
    [EPiServerDataStore(AutomaticallyRemapStore = true)]
    public class GcDynamicMappings : IDynamicData
    {
        //getter and setter to store the mapped template settings.
        public Identity Id { get; set; }
        public string AccountId { get; set; }
        public string ProjectId { get; set; }
        public string TemplateId { get; set; }
        public string PostType { get; set; }
        public string Author { get; set; }
        public string DefaultStatus { get; set; }
        public string EpiContentType { get; set; }
        public List<GcEpiStatusMap> StatusMaps { get; set; }
        public List<string> EpiFieldMaps { get; set; }

        //Parameter-less constructor required for the dynamic data store.
        public GcDynamicMappings()
        {
            Id = Identity.NewIdentity(Guid.NewGuid());
            AccountId = string.Empty;
            ProjectId = string.Empty;
            TemplateId = string.Empty;
            PostType = string.Empty;
            Author = string.Empty;
            DefaultStatus = string.Empty;
            EpiContentType = string.Empty;
            StatusMaps = new List<GcEpiStatusMap>();
            EpiFieldMaps = new List<string>();
        }

        public GcDynamicMappings(string accountId, string projectId, string templateId, string postType, string author,
            string defaultStatus, string epiContentType, List<GcEpiStatusMap> statusMaps, List<string> epiFieldMaps)
        {
            Id = Identity.NewIdentity(Guid.NewGuid());
            AccountId = accountId;
            ProjectId = projectId;
            TemplateId = templateId;
            PostType = postType;
            Author = author;
            DefaultStatus = defaultStatus;
            EpiContentType = epiContentType;
            StatusMaps = statusMaps;
            EpiFieldMaps = epiFieldMaps;
        }

        //Save the Mappings.
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
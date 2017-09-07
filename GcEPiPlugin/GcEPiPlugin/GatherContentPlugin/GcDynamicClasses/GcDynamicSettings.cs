using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using EPiServer.Data;
using EPiServer.Data.Dynamic;
using GcEPiPlugin.GatherContentPlugin.GcEpiObjects;

namespace GcEPiPlugin.GatherContentPlugin.GcDynamicClasses
{
    [EPiServerDataStore(AutomaticallyRemapStore = true)]
    public class GcDynamicSettings : IDynamicData
    {
        public Identity Id { get; set; }
        public string AccountId { get; set; }
        public string ProjectId { get; set; }
        public string TemplateId { get; set; }
        public string PostType { get; set; }
        public string Author { get; set; }
        public string EPiStatus { get; set; }
        public List<GcEpiStatusMap> StatusMaps { get; set; }
        public List<GcEpiContentTypeMap> ContentTypeMaps { get; set; }
        //Parameter-less constructor required for the dynamic data store.
        public GcDynamicSettings()
        {
            // Generate a new ID.
            Id = Identity.NewIdentity(Guid.NewGuid());
            AccountId = string.Empty;
            ProjectId = string.Empty;
            TemplateId = string.Empty;
            PostType = string.Empty;
            Author = string.Empty;
            EPiStatus = string.Empty;
            StatusMaps = new List<GcEpiStatusMap>();
            ContentTypeMaps = new List<GcEpiContentTypeMap>();
        }                                                                                                                                                                                                                                                                                                                   

        public GcDynamicSettings([Optional] string accountId, [Optional] string projectId, [Optional] string templateId
            , [Optional] string postType, [Optional] string author, [Optional] string epiStatus, [Optional] List<GcEpiStatusMap> statusMaps
            , [Optional] List<GcEpiContentTypeMap> contentTypeMaps)
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
            if (postType != null)
                PostType = postType;
            else
            {
                PostType = RetrieveStore().Count > 0 ? RetrieveStore().ToList()[0].PostType : string.Empty;
            }
            if (author != null)
                Author = author;
            else
            {
                Author = RetrieveStore().Count > 0 ? RetrieveStore().ToList()[0].Author : string.Empty;
            }
            if (epiStatus != null)
                EPiStatus = epiStatus;
            else
            {
                EPiStatus = RetrieveStore().Count > 0 ? RetrieveStore().ToList()[0].EPiStatus : string.Empty;
            }
            if (statusMaps != null)
                StatusMaps = statusMaps;
            else
            {
                StatusMaps = RetrieveStore().Count > 0 ? RetrieveStore().ToList()[0].StatusMaps : new List<GcEpiStatusMap>();
            }
            if (contentTypeMaps != null)
                ContentTypeMaps = contentTypeMaps;
            else
            {
                ContentTypeMaps = RetrieveStore().Count > 0 ? RetrieveStore().ToList()[0].ContentTypeMaps : new List<GcEpiContentTypeMap>();
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
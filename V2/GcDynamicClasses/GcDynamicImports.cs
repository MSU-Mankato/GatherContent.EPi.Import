using System;
using EPiServer.Data;
using EPiServer.Data.Dynamic;

namespace GatherContentImport.GcDynamicClasses
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
    }
}
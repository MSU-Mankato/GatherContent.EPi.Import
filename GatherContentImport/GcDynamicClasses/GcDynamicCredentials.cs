using System;
using EPiServer.Data;
using EPiServer.Data.Dynamic;

namespace GatherContentImport.GcDynamicClasses
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
    }
}
using EPiServer.Data.Dynamic;

namespace GcEPiPlugin.GatherContentPlugin.GcEpiObjects
{
    [EPiServerDataStore(AutomaticallyRemapStore = true)]
    public class GcEpiContentTypeMap
    {
        //getter and setter for content type.
        public string ContentType { get; set; }
        //getter and setter for Content Type Metadata.
        public string Metadata { get; set; }
    }
}
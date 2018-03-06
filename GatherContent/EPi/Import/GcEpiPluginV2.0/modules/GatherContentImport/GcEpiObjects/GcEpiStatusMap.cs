using EPiServer.Data.Dynamic;

namespace GcEpiPluginV2._0.modules.GatherContentImport.GcEpiObjects
{
    [EPiServerDataStore(AutomaticallyRemapStore = true)]
    public class GcEpiStatusMap
    {
        //getter and setter for mapped EPiServer status.
        public string MappedEpiserverStatus { get; set; }
        //getter and setter for on import, change GatherContent status.
        public string OnImportChangeGcStatus { get; set; }
    }
}
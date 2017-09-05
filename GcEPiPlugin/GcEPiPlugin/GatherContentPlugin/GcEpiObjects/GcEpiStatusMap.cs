using EPiServer.Data.Dynamic;

namespace GcEPiPlugin.GatherContentPlugin.GcEpiObjects
{
    [EPiServerDataStore(AutomaticallyRemapStore = true)]
    public class GcEpiStatusMap
    {
        //getter and setter for mapped EPiServer status.
        public string MappedEpiserverStatus { get; set; }
        //getter and setter for on import, change GatherContent status.
        public string OnImportChangeGcStatus { get; set; }
        public string ProjectId { get; set; }
        public string TemplateId { get; set; }
    }
}
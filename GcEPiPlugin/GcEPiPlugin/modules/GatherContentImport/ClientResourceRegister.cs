using System.Web;
using EPiServer.Framework.Web.Resources;

namespace GcEPiPlugin.modules.GatherContentImport
{
    [ClientResourceRegister]
    public class ClientResourceRegister : IClientResourceRegister
    {
        public void RegisterResources(IRequiredClientResourceList requiredResources, HttpContextBase context)
        {
            requiredResources.Require("epi.samples.Module.Styles");
            requiredResources.Require("epi.samples.Module.FormHandler").AtFooter();
            requiredResources.RequireScript("http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.8.0.min.js").AtFooter();
            requiredResources.Require("jquery.ui").StylesOnly().AtHeader();
            requiredResources.Require("jquery.ui").ScriptsOnly().AtFooter();
        }
    }
}
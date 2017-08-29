using System;
using System.Collections.Generic;
using System.Web.Security;
using System.Web.UI.WebControls;
using EPiServer.Personalization;
using EPiServer.PlugIn;
using EPiServer.Security;
using EPiServer.Util.PlugIns;
using System.Web.UI;

namespace GcEPiPlugin.GatherContentPlugin
{
    [GuiPlugIn(DisplayName = "TestPlugin", Description = "GatherContent", Area = PlugInArea.AdminMenu, Url = "~/GatherContentPlugin/TestPlugin.aspx")]
    public partial class TestPlugin : System.Web.UI.Page
    {

        // TODO: Add your Plugin Control Code here.

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using EPiServer.PlugIn;
using EPiServer.Security;
using Castle.Core.Internal;
using EPiServer;
using GatherContentConnect;
using GatherContentImport.GcDynamicClasses;

namespace GatherContentImport.modules.GcEpiPlugin
{
    [GuiPlugIn(DisplayName = "New Gc Mapping Step 1", Description = "This is where the user sets the Project Id", Area = PlugInArea.AdminMenu, Url = "~/modules/GcEpiPlugin/NewGcMappingStep1.aspx")]
    public partial class NewGcMappingStep1 : SimplePage
    {
        private GcConnectClient _client;
        private readonly List<GcDynamicCredentials> _credentialsStore = GcDynamicUtilities.RetrieveStore<GcDynamicCredentials>();
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!PrincipalInfo.HasAdminAccess)
            {
                AccessDenied();
            }


            if (!IsPostBack)
            {
                PopulateForm();
            }
        }

        private void PopulateForm()
        {

            if (_credentialsStore.IsNullOrEmpty())
            {
                Response.Write("<script>alert('Please setup your GatherContent config first!');" +
                               "window.location='/modules/GcEpiPlugin/GatherContentConfigSetup.aspx'</script>");
                Visible = false;
                return;
            }

            _client = new GcConnectClient(_credentialsStore.ToList().First().ApiKey, _credentialsStore.ToList().First().Email);
            var accountId = Convert.ToInt32(_credentialsStore.ToList().First().AccountId);
            Session["AccountId"] = accountId;
            accountName.Text = _client.GetAccountById(accountId).Name;
            var projects = _client.GetProjectsByAccountId(accountId);
            projects.ToList().ForEach(i => rblGcProjects.Items.Add(new ListItem(i.Name, i.Id.ToString())));
            rblGcProjects.SelectedIndex = 0;
            Session["ProjectId"] = rblGcProjects.SelectedValue;
            Session["PostType"] = null;
            Session["Author"] = null;
            Session["DefaultStatus"] = null;
        }

        protected void BtnNextStep_OnClick(object sender, EventArgs e)
        {
            var selectedValue = Request.Form["rblGcProjects"];
            Session["ProjectId"] = selectedValue;
            Response.Redirect("~/modules/GcEpiPlugin/NewGcMappingStep2.aspx");
        }

    }
}
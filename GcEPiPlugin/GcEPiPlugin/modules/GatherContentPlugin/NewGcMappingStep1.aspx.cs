using System;
using System.Linq;
using System.Web.UI.WebControls;
using Castle.Core.Internal;
using EPiServer.PlugIn;
using GatherContentConnect;
using GcEPiPlugin.modules.GatherContentPlugin.GcDynamicClasses;
using EPiServer;
using EPiServer.Security;

namespace GcEPiPlugin.modules.GatherContentPlugin
{
    [GuiPlugIn(DisplayName = "New GC Mapping Step 1", Description = "This is where the new gather content mapping starts from.", Area = PlugInArea.AdminMenu, Url = "~/modules/GatherContentPlugin/NewGcMappingStep1.aspx")]
    public partial class NewGcMappingStep1 : SimplePage
    {
        private GcConnectClient _client;
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
            var credentialsStore = GcDynamicCredentials.RetrieveStore();
            if (credentialsStore.IsNullOrEmpty())
            {
                Visible = false;
                return;
            }
            _client = new GcConnectClient(credentialsStore.ToList().First().ApiKey,credentialsStore.ToList().First().Email);
            var accountId = Convert.ToInt32(credentialsStore.ToList().First().AccountId);
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
            Response.Redirect("~/modules/GatherContentPlugin/NewGcMappingStep2.aspx");
        }
    }
}
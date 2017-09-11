using System;
using System.Linq;
using System.Web.UI.WebControls;
using EPiServer.PlugIn;
using GatherContentConnect;
using GcEPiPlugin.GatherContentPlugin.GcDynamicClasses;
using EPiServer;
using EPiServer.Security;

namespace GcEPiPlugin.GatherContentPlugin
{
    [GuiPlugIn(DisplayName = "New Mapping", Description = "This is where the new gather content mapping starts from.", Area = PlugInArea.AdminMenu, Url = "~/GatherContentPlugin/NewGcMapping.aspx")]
    public partial class NewGcMapping : SimplePage
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
            var settingsStore = GcDynamicSettings.RetrieveStore();
            if (credentialsStore.Count <= 0 || settingsStore.Count <= 0)
            {
                Visible = false;
                return;
            }
            _client = new GcConnectClient(credentialsStore.ToList().First().ApiKey,credentialsStore.ToList().First().Email);
            var accountId = Convert.ToInt32(settingsStore.ToList().First().AccountId);
            accountName.Text = _client.GetAccountById(accountId).Name;
            var projects = _client.GetProjectsByAccountId(accountId);
            projects.ToList().ForEach(i => rblGcProjects.Items.Add(new ListItem(i.Name, i.Id.ToString())));
			if (Session["ProjectId"] == null)
            {
                rblGcProjects.SelectedIndex = 0;
				Session["ProjectId"] = rblGcProjects.SelectedValue;
            }
            var projectId = Session["ProjectId"].ToString();
            rblGcProjects.SelectedValue = projectId;
			
        }

        protected void BtnNextStep_OnClick(object sender, EventArgs e)
        {
			var selectedValue = Request.Form["rblGcProjects"];
			Session["ProjectId"] = selectedValue;
            Response.Redirect("~/GatherContentPlugin/NewGcMappingV2.aspx");
        }
    }
}
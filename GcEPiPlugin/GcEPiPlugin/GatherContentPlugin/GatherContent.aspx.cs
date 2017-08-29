using System;
using EPiServer.PlugIn;
using EPiServer;
using EPiServer.Security;
using GatherContentConnect;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using GcEPiPlugin.GatherContentPlugin.GcDynamicClasses;

namespace GcEPiPlugin.GatherContentPlugin
{
    [GuiPlugIn(DisplayName = "GatherContent", Description = "GatherContent Accounts and Settings", Area = PlugInArea.AdminMenu, Url = "~/GatherContentPlugin/GatherContent.aspx")]
    public partial class GatherContent : SimplePage
    {
        private GcConnectClient _client;
        private GcDynamicCredentials _credentials;
        private GcDynamicSettings _settings;
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
        protected void BtnSave_OnClick(object sender, EventArgs e)
        {
            var apiKey = HttpUtility.HtmlEncode(txtApiKey.Text);
            var emailAddress = HttpUtility.HtmlEncode(txtEmailAddress.Text);
            _client = new GcConnectClient(apiKey,emailAddress);
            if (_client.GetAccounts() != null && _client.GetAccounts().Count > 0 )
            {
                _credentials = new GcDynamicCredentials(emailAddress, apiKey);
                GcDynamicCredentials.SaveStore(_credentials);
                var selectedValue = Request.Form["ddlGcAccounts"];
                _settings = new GcDynamicSettings(selectedValue);
                GcDynamicSettings.SaveStore(_settings);
            }
            else
            {
                Response.Write("<script>alert('Invalid Email Address or ApiKey! Try again! ')</script>");
                GcDynamicCredentials.ClearStore();
                GcDynamicSettings.ClearStore();
            }
            PopulateForm();
        }
        private void PopulateForm()
        {
            var settingsStore = GcDynamicSettings.RetrieveStore();
            var credentialsStore = GcDynamicCredentials.RetrieveStore();
            if (credentialsStore.Count <= 0) return;
            var email = credentialsStore.ToList().First().Email;
            var apiKey = credentialsStore.ToList().First().ApiKey;
            txtEmailAddress.Text = email;
            txtApiKey.Text = apiKey;
            _client = new GcConnectClient(apiKey,email);
            var accounts = _client.GetAccounts();
            accounts.ToList().ForEach(i => ddlGcAccounts.Items.Add(new ListItem(i.Name, i.Id, true)));
            if (settingsStore.Count <= 0 || settingsStore.ToList().First().AccountId == "")
            {
                _settings = new GcDynamicSettings(ddlGcAccounts.SelectedValue);
                GcDynamicSettings.SaveStore(_settings);
                settingsStore = GcDynamicSettings.RetrieveStore();
            }
            ddlGcAccounts.SelectedValue = settingsStore.ToList().First().AccountId;
            txtPlatformUrl.Text = $"https://{_client.GetAccountById(Convert.ToInt32(settingsStore.ToList().First().AccountId)).Slug}.gathercontent.com";
        }
    }
}
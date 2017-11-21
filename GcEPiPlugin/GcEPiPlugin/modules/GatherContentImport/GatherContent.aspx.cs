using System;
using EPiServer.PlugIn;
using EPiServer;
using EPiServer.Security;
using GatherContentConnect;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Castle.Core.Internal;
using GcEPiPlugin.modules.GatherContentImport.GcDynamicClasses;

namespace GcEPiPlugin.modules.GatherContentImport
{
    [GuiPlugIn(DisplayName = "GatherContent Config Setup", Description = "GatherContent Accounts and Settings", Area = PlugInArea.AdminMenu, Url = "~/modules/GatherContentPlugin/GatherContent.aspx")]
    public partial class GatherContent : SimplePage
    {
        private GcConnectClient _client;
        private GcDynamicCredentials _credentials;
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
            if (!_client.GetAccounts().IsNullOrEmpty())
            {
                var selectedAccount = Request.Form["ddlGcAccounts"];
                if (selectedAccount.IsNullOrEmpty())
                {
                    selectedAccount = _client.GetAccounts().ToList().First().Id;
                }
                _credentials = new GcDynamicCredentials(emailAddress, apiKey, selectedAccount);
                GcDynamicCredentials.SaveStore(_credentials);
                Response.Write($"<script>alert('Hello {_client.GetMe().FirstName}! You have successfully connected to" +
                               " the GatherContent API')</script>");
            }
            else
            {
                Response.Write("<script>alert('Invalid Email Address or ApiKey! Try again!')</script>");
                //txtPlatformUrl.Text = "";
                GcDynamicCredentials.ClearStore();
            }
            PopulateForm();
        }
        private void PopulateForm()
        {
            var credentialsStore = GcDynamicCredentials.RetrieveStore();
            if (credentialsStore.IsNullOrEmpty()) return;
            txtEmailAddress.Text = credentialsStore.ToList().First().Email;
            txtApiKey.Text = credentialsStore.ToList().First().ApiKey;
            _client = new GcConnectClient(credentialsStore.ToList().First().ApiKey, credentialsStore.ToList().First().Email);
            var accounts = _client.GetAccounts();
            accounts.ToList().ForEach(i => ddlGcAccounts.Items.Add(new ListItem(i.Name, i.Id)));
            if (!credentialsStore.ToList().First().AccountId.IsNullOrEmpty())
            {
                ddlGcAccounts.SelectedValue = credentialsStore.ToList().First().AccountId;
                //txtPlatformUrl.Text = $"https://{_client.GetAccountById(Convert.ToInt32(credentialsStore.ToList().First().AccountId)).Slug}.gathercontent.com";
            }   
            else
            {
                ddlGcAccounts.SelectedIndex = 0;
                //txtPlatformUrl.Text = $"https://{_client.GetAccountById(Convert.ToInt32(ddlGcAccounts.SelectedValue)).Slug}.gathercontent.com";
            }
            
        }
    }
}
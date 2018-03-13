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
    [GuiPlugIn(DisplayName = "GatherContent Config Setup", Description = "This is where the user sets up the GatherContent config", Area = PlugInArea.AdminMenu, Url = "~/modules/GcEpiPlugin/GatherContentConfigSetup.aspx")]
    public partial class GatherContentConfigSetup : SimplePage
    {

        private GcConnectClient _client;
        private GcDynamicCredentials _credentials;
        private List<GcDynamicCredentials> _credentialsStore;
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
            _credentialsStore = GcDynamicUtilities.RetrieveStore<GcDynamicCredentials>();
            var apiKey = txtApiKey.Text;
            var emailAddress = txtEmailAddress.Text;
            _client = new GcConnectClient(apiKey, emailAddress);

            if (!_client.GetAccounts().IsNullOrEmpty())
            {
                var selectedAccount = Request.Form["ddlGcAccounts"];
                if (selectedAccount.IsNullOrEmpty())
                {
                    selectedAccount = _client.GetAccounts().ToList().First().Id;
                }
                _credentials = new GcDynamicCredentials(emailAddress, apiKey, selectedAccount);
                GcDynamicUtilities.SaveStore(_credentials);
                Response.Write($"<script>alert('Hello {_client.GetMe().FirstName}! You have successfully connected to" +
                               " the GatherContent API')</script>");
            }
            else
            {
                Response.Write("<script>alert('Invalid Email Address or ApiKey! Try again!')</script>");
                //txtPlatformUrl.Text = "";
                GcDynamicUtilities.ClearStore<GcDynamicCredentials>();
            }
            PopulateForm();
        }
        private void PopulateForm()
        {
            _credentialsStore = GcDynamicUtilities.RetrieveStore<GcDynamicCredentials>();
            ddlGcAccounts.Items.Clear();
            txtApiKey.Text = string.Empty;
            txtEmailAddress.Text = string.Empty;
            if (_credentialsStore.IsNullOrEmpty()) return;
            var email = _credentialsStore.ToList().First().Email;
            var apiKey = _credentialsStore.ToList().First().ApiKey;
            txtEmailAddress.Text = email;
            txtApiKey.Text = apiKey;
            _client = new GcConnectClient(apiKey, email);
            var accounts = _client.GetAccounts();
            accounts.ToList().ForEach(i => ddlGcAccounts.Items.Add(new ListItem(i.Name, i.Id)));
            if (!_credentialsStore.ToList().First().AccountId.IsNullOrEmpty())
            {
                ddlGcAccounts.SelectedValue = _credentialsStore.ToList().First().AccountId;
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
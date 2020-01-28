using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace XamarinLinkerApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TestPage : ContentPage
    {
        private const string EmptyResult = "Result:";
        private const string SuccessfulResult = "Result: Success";
        public IPublicClientApplication PublicClientApplication { get; set; }
        public TestPage()
        {
            InitializeComponent();
        }

        public void InitPublicClient()
        {
            var builder = PublicClientApplicationBuilder
                .Create(App.DefaultClientId)
                .WithAuthority(new Uri(App.DefaultAuthority), false)
                //.WithB2CAuthority(App.DefaultAuthority)
                .WithIosKeychainSecurityGroup("com.microsoft.adalcache");

            // Let Android set its own redirect uri
            switch (Device.RuntimePlatform)
            {
                case "iOS":
                    builder = builder.WithRedirectUri(App.s_redirectUriOnAndroid);
                    break;
                case "Android":
                    builder = builder.WithRedirectUri(App.s_redirectUriOnAndroid);
                    break;
            }

            PublicClientApplication = builder.Build();
        }

        private void OnPickerSelectedIndexChanged(object sender, EventArgs args)
        {
            var selectedTest = (Picker)sender;
            int selectedIndex = selectedTest.SelectedIndex;

            switch (selectedIndex)
            {
                case 0: // AT Interactive
                    InitPublicClient();
                    AcquireTokenInteractiveAsync(Prompt.ForceLogin).ConfigureAwait(false);
                    break;
                case 1: // AT Silent
                    InitPublicClient();
                    AcquireTokenSilentAsync().ConfigureAwait(false);
                    break;
            }
        }

        private async Task AcquireTokenInteractiveAsync(Prompt prompt)
        {
            try
            {
                AcquireTokenInteractiveParameterBuilder request = PublicClientApplication.AcquireTokenInteractive(App.s_defaultScopes)
                    .WithPrompt(prompt)
                    .WithParentActivityOrWindow(App.RootViewController);
                    //.WithUseEmbeddedWebView(true);

                AuthenticationResult result = await
                    request.ExecuteAsync().ConfigureAwait(true);

                var resText = GetResultDescription(result);

                if (result.AccessToken != null)
                {
                    acquireResponseTitleLabel.Text = SuccessfulResult;
                }

                acquireResponseLabel.Text = resText;
            }
            catch (Exception exception)
            {
                CreateExceptionMessage(exception);
            }
        }

        private async Task AcquireTokenSilentAsync()
        {
            try
            {
                AcquireTokenInteractiveParameterBuilder request = PublicClientApplication.AcquireTokenInteractive(App.s_defaultScopes)
                   .WithPrompt(Prompt.ForceLogin)
                   .WithParentActivityOrWindow(App.RootViewController);
                   //.WithUseEmbeddedWebView(true);

                AuthenticationResult result = await
                    request.ExecuteAsync().ConfigureAwait(true);

                AcquireTokenSilentParameterBuilder builder = PublicClientApplication.AcquireTokenSilent(
                    App.s_defaultScopes,
                    result.Account.Username);

                AuthenticationResult res = await builder
                    .WithForceRefresh(false)
                    .ExecuteAsync()
                    .ConfigureAwait(true);

                var resText = GetResultDescription(res);

                if (res.AccessToken != null)
                {
                    acquireResponseTitleLabel.Text = SuccessfulResult;
                }

                acquireResponseLabel.Text = "Acquire Token Silent Acquisition Result....\n" + resText;
            }
            catch (Exception exception)
            {
                CreateExceptionMessage(exception);
            }
        }


        private static string GetResultDescription(AuthenticationResult result)
        {
            var sb = new StringBuilder();

            sb.AppendLine("AccessToken : " + result.AccessToken);
            sb.AppendLine("IdToken : " + result.IdToken);
            sb.AppendLine("ExpiresOn : " + result.ExpiresOn);
            sb.AppendLine("TenantId : " + result.TenantId);
            sb.AppendLine("Scope : " + string.Join(",", result.Scopes));
            sb.AppendLine("User :");
            sb.Append(GetAccountDescription(result.Account));

            return sb.ToString();
        }

        private static string GetAccountDescription(IAccount user)
        {
            var sb = new StringBuilder();

            sb.AppendLine("user.DisplayableId : " + user.Username);
            sb.AppendLine("user.Environment : " + user.Environment);

            return sb.ToString();
        }

        private void CreateExceptionMessage(Exception exception)
        {
            if (exception is MsalException msalException)
            {
                acquireResponseLabel.Text = string.Format(CultureInfo.InvariantCulture, "MsalException -\nError Code: {0}\nMessage: {1}",
                    msalException.ErrorCode, msalException.Message);
            }
            else
            {
                acquireResponseLabel.Text = "Exception - " + exception.Message;
            }

            Console.WriteLine(exception.Message);
        }
    }
}
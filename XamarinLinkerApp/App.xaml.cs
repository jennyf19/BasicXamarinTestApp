using Microsoft.Identity.Client;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace XamarinLinkerApp
{
    public partial class App : Application
    {
        public static PublicClientApplication MsalPublicClient;

        public static object RootViewController { get; set; }

        public const string DefaultClientId = "4a1aa1d5-c567-49d0-ad0b-cd957a47f842";
        public const string DefaultAuthority = "https://login.microsoftonline.com/common";
        public static string s_redirectUriOnAndroid = "msal4a1aa1d5-c567-49d0-ad0b-cd957a47f842://auth";

        public static string[] s_defaultScopes = { "User.Read" };
        public App()
        {
            MainPage = new NavigationPage(new XamarinLinkerApp.MainPage());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}

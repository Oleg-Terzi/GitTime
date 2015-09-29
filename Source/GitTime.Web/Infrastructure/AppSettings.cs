using System;

using GitTime.Web.Infrastructure.Helpers;

namespace GitTime.Web.Infrastructure
{
    public static class AppSettings
    {
        public static class Names
        {
            public const String GitHubApplicationClientID = "GitHub.Application.ClientID";
            public const String GitHubApplicationSecretKey = "GitHub.Application.SecretKey";
        }

        public static class GitHub
        {
            public static String ClientID
            {
                get { return AppSettingsHelper.GetValue(Names.GitHubApplicationClientID, false); }
            }

            public static String SecretKey
            {
                get { return AppSettingsHelper.GetValue(Names.GitHubApplicationSecretKey, false); }
            }
        }
    }
}
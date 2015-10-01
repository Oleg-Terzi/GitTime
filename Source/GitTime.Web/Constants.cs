using System;

namespace GitTime.Web
{
    public class Constants
    {
        public const Int32 PageSize = 20;
        public const Int32 VisiblePageCount = 10;

        public static class ContactType
        {
            public const String Company = "Company";
            public const String Person = "Person";
        }

        public static class RoleType
        {
            public const String Administrator = "Administrator";
            public const String Developer = "Developer";
        }
    }
}
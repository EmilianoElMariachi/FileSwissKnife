using System;
using System.Reflection;

namespace FileSwissKnife.Utils
{
    public static class AppInfo
    {

        static AppInfo()
        {

            var assembly = Assembly.GetExecutingAssembly();
            var assemblyName = assembly.GetName();
            var version = assemblyName.Version;
            Version = version;
            DisplayVersion = version != null ? $"{version.Major}.{version.Minor}.{version.Revision}" : "???";

            var attributes = assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
            ProductName = attributes.Length > 0 ? ((AssemblyProductAttribute)attributes[0]).Product : "???";

            attributes = assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
            Company = attributes.Length > 0 ? ((AssemblyCompanyAttribute)attributes[0]).Company : "???";
        }

        public static string Company { get; }

        public static Version? Version { get; }

        public static string DisplayVersion { get; }

        public static string ProductName { get; }

    }
}

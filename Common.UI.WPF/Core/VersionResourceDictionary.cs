using System;
using System.Windows;

namespace Common.UI.WPF.Core
{
    public class VersionResourceDictionary : ResourceDictionary
    {
        public VersionResourceDictionary() { }

        public VersionResourceDictionary(string assemblyName, string sourcePath)
        : base(assemblyName, sourcePath)
        {

        }

        protected override Uri BuildUri()
        {
            var source = this.SourcePath;
            string uriStr = PackUriExtension.BuildAbsolutePackUriString(this.AssemblyName, UIWPFVersionInfo.Version, source);
            return new Uri(uriStr, UriKind.Absolute);
        }
    }
}

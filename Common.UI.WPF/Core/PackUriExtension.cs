using System;
using System.Windows.Markup;

namespace Common.UI.WPF.Core
{
    [MarkupExtensionReturnType(typeof(Uri))]
    public class PackUriExtension : MarkupExtension
    {
        #region Constructors
        public PackUriExtension()
          : this(UriKind.Relative)
        {
        }

        public PackUriExtension(UriKind uriKind)
        {
            this.uriKind = uriKind;
        }

        #endregion // Constructors

        #region AssemblyName Property

        public string AssemblyName
        {
            get { return this.assemblyName; }
            set { this.assemblyName = value; }
        }

        #endregion // AssemblyName Property

        #region Path Property

        public string Path
        {
            get { return this.path; }
            set { this.path = value; }
        }

        #endregion // Path Property

        #region Kind Property

        public UriKind Kind
        {
            get { return this.uriKind; }
            set { this.uriKind = value; }
        }

        #endregion // Kind Property

        #region PUBLIC METHODS

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (string.IsNullOrEmpty(this.path))
                throw new InvalidOperationException("Path must be set during initialization");

            string uriString;

            switch (this.uriKind)
            {
                case UriKind.RelativeOrAbsolute:
                case UriKind.Relative:
                    uriString = BuildRelativePackUriString(this.assemblyName, this.path);
                    break;

                case UriKind.Absolute:
                    uriString = BuildAbsolutePackUriString(this.assemblyName, this.path);
                    break;

                default:
                    throw new NotSupportedException();
            }

            return new Uri(uriString, this.uriKind);
        }

        #endregion PUBLIC METHODS

        #region INTERNAL METHODS

        internal static string BuildRelativePackUriString(string assemblyName, string path)
        {
            return BuildRelativePackUriString(assemblyName, String.Empty, path);
        }

        internal static string BuildRelativePackUriString(string assemblyName, string version, string path)
        {
            if (string.IsNullOrEmpty(assemblyName))
                throw new ArgumentException("assemblyName cannot be null or empty", assemblyName);

            string platformSuffix = String.Empty;



            // If we have version information
            if (!String.IsNullOrEmpty(version))
            {
                // Format it for the pack uri
                version = ";v" + version;
            }

            // Format a relative pack uri string
            string uriString = string.Format("/{0}{1}{2};component/{3}", assemblyName, platformSuffix, version, path);

            return uriString;
        }

        internal static string BuildAbsolutePackUriString(string assemblyName, string path)
        {
            return BuildAbsolutePackUriString(assemblyName, String.Empty, path);
        }

        internal static string BuildAbsolutePackUriString(string assemblyName, string version, string path)
        {
            string platformSuffix = String.Empty;
            bool hasAssemblyName = !String.IsNullOrEmpty(assemblyName);



            // If we have an assembly name and version information
            if (hasAssemblyName && !String.IsNullOrEmpty(version))
            {
                // Format it for the pack uri
                version = ";v" + version;
            }

            // Format an absolute pack uri string
            string uriString = string.Format("pack://application:,,,/{0}{1}{2};component/{3}", assemblyName, platformSuffix, version, path);

            return uriString;
        }

        #endregion INTERNAL METHODS

        #region PRIVATE FIELDS

        private string assemblyName;
        private string path;
        private UriKind uriKind;

        #endregion PRIVATE FIELDS
    }
}

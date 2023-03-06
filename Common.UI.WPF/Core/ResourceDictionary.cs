using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Common.UI.WPF.Core
{
    public class ResourceDictionary : System.Windows.ResourceDictionary, ISupportInitialize
    {
        private int initializingCount;
        private string assemblyName;
        private string sourcePath;


        public ResourceDictionary() { }

        public ResourceDictionary(string assemblyName, string sourcePath)
        {
            ((ISupportInitialize)this).BeginInit();
            this.AssemblyName = assemblyName;
            this.SourcePath = sourcePath;
            ((ISupportInitialize)this).EndInit();
        }

        public string AssemblyName
        {
            get { return assemblyName; }
            set
            {
                this.EnsureInitialization();
                assemblyName = value;
            }
        }

        public string SourcePath
        {
            get { return sourcePath; }
            set
            {
                this.EnsureInitialization();
                sourcePath = value;
            }
        }

        protected virtual Uri BuildUri()
        {
            // Build a pack uri relative to the root of the supplied assembly name
            string uriStr = PackUriExtension.BuildRelativePackUriString(this.AssemblyName, this.SourcePath);
            return new Uri(uriStr, UriKind.Relative);
        }

        private void EnsureInitialization()
        {
            if (initializingCount <= 0)
                throw new InvalidOperationException(this.GetType().Name + " properties can only be set while initializing.");
        }

        void ISupportInitialize.BeginInit()
        {
            base.BeginInit();
            initializingCount++;
        }

        void ISupportInitialize.EndInit()
        {
            initializingCount--;
            Debug.Assert(initializingCount >= 0);

            if (initializingCount <= 0)
            {
                if (this.Source != null)
                    throw new InvalidOperationException("Source property cannot be initialized on the " + this.GetType().Name);

                if (string.IsNullOrEmpty(this.AssemblyName) || string.IsNullOrEmpty(this.SourcePath))
                    throw new InvalidOperationException("AssemblyName and SourcePath must be set during initialization");

                // Build the pack uri based on the value of our properties
                Uri uri = this.BuildUri();

                // Load the resources
                this.Source = uri;
            }

            base.EndInit();
        }


        private enum InitState
        {
            NotInitialized,
            Initializing,
            Initialized
        };
    }
}

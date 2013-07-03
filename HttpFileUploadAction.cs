using System;
using System.Collections.Generic;
using System.Text;
using Inedo.BuildMaster.Extensibility.Actions;
using Inedo.BuildMaster.Web;
using System.Net;
using Inedo.BuildMaster;
using System.IO;

namespace Inedo.BuildMasterExtensions.HTTP
{
    [ActionProperties(
        "Upload File to URL",
        "Uploads a file to a specified URL using an HTTP POST.",
        "HTTP")]
    [CustomEditor(typeof(HttpFileUploadActionEditor))]
    public sealed class HttpFileUploadAction : RemoteActionBase
    {
        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        [Persistent]
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        [Persistent]
        public string Url { get; set; }

        protected override string ProcessRemoteCommand(string name, string[] args)
        {
            var client = new WebClient();

            string filePath = Path.Combine(this.RemoteConfiguration.SourceDirectory, this.FileName);

            if (!File.Exists(filePath))
            {
                LogError("The file \"{0}\" does not exist.", filePath);
                return null;
            }

            LogDebug("Uploading file from: {0} to {1}", filePath, this.Url);
            client.UploadFile(this.Url, "POST", filePath);

            return null;
        }

        protected override void Execute()
        {
            try
            {
                new Uri(this.Url);
            }
            catch (Exception ex)
            {
                LogError("Specified URL ({0}) is invalid: {1}", this.Url, ex.Message);
                return;
            }
            
            ExecuteRemoteCommand(null);
        }

        public override string ToString()
        {
            return String.Format(
                "Upload the file named \"{0}\" from {1} to the URL \"{2}\".",
                this.FileName,
                Util.CoalesceStr(this.OverriddenSourceDirectory, "the default directory"),
                this.Url
            );
        }
    }
}

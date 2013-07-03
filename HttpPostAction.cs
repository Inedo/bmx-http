using System;
using System.IO;
using System.Net;
using System.Text;
using Inedo.BuildMaster;
using Inedo.BuildMaster.Extensibility.Actions;
using Inedo.BuildMaster.Web;

namespace Inedo.BuildMasterExtensions.HTTP
{
    /// <summary>
    /// Executes an HTTP POST against a URL.
    /// </summary>
    [ActionProperties(
        "Post to URL",
        "Executes an HTTP POST request against a URL.",
        "HTTP")]
    [CustomEditor(typeof(HttpPostActionEditor))]
    public sealed class HttpPostAction : HttpActionBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpPostAction"/> class.
        /// </summary>
        public HttpPostAction()
        {
        }

        /// <summary>
        /// Gets or sets the URL for the HTTP GET request.
        /// </summary>
        [Persistent]
        public string Url { get; set; }
        /// <summary>
        /// Gets or sets the data to post.
        /// </summary>
        [Persistent]
        public string PostData { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(this.PostData))
            {
                return string.Format(
                    "HTTP POST to {0}",
                    this.Url
                );
            }
            else
            {
                return string.Format(
                    "HTTP POST \"{0}\" to {1}",
                    this.PostData,
                    this.Url
                );
            }
        }

        /// <summary>
        /// This method is called to execute the Action.
        /// </summary>
        protected override void Execute()
        {
            try
            {
                new Uri(this.Url);
            }
            catch (Exception ex)
            {
                this.LogError("Specified URL ({0}) is invalid: {1}", this.Url, ex.Message);
                return;
            }

            this.LogInformation("POSTing \"{0}\" to {1}", this.PostData, this.Url);
            this.ExecuteRemoteCommand("get");
        }
        /// <summary>
        /// When implemented in a derived class, processes an arbitrary command
        /// on the appropriate server.
        /// </summary>
        /// <param name="name">Name of command to process.</param>
        /// <param name="args">Optional command arguments.</param>
        /// <returns>
        /// Result of the command.
        /// </returns>
        protected override string ProcessRemoteCommand(string name, string[] args)
        {
            var request = (HttpWebRequest)WebRequest.Create(this.Url);
            request.Method = "POST";

            request.ContentType = "application/x-www-form-urlencoded";
            
            if (!string.IsNullOrEmpty(this.PostData))
            {
                var requestStream = request.GetRequestStream();
                // write POST data in UTF-8 without BOM
                using (var sw = new StreamWriter(requestStream, new UTF8Encoding(false)))
                {
                    sw.Write(this.PostData);
                }
            }

            this.PerformRequest(request);

            return string.Empty;
        }
    }
}

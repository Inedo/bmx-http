using System;
using System.Net;
using Inedo.BuildMaster;
using Inedo.BuildMaster.Extensibility.Actions;
using Inedo.BuildMaster.Web;

namespace Inedo.BuildMasterExtensions.HTTP
{
    /// <summary>
    /// Executes an HTTP GET against a URL.
    /// </summary>
    [ActionProperties(
        "Get URL",
        "Executes an HTTP GET request against a URL.",
        "HTTP")]
    [CustomEditor(typeof(HttpGetActionEditor))]
    public sealed class HttpGetAction : HttpActionBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpGetAction"/> class.
        /// </summary>
        public HttpGetAction()
        {
        }

        /// <summary>
        /// Gets or sets the URL for the HTTP GET request.
        /// </summary>
        [Persistent]
        public string Url { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                "Execute HTTP GET on {0}",
                this.Url
            );
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
            
            this.LogInformation("Performing HTTP GET on {0}", this.Url);
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
            this.PerformRequest(request);
            return string.Empty;
        }
    }
}

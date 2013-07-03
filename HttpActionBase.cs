using System;
using System.IO;
using System.Net;
using Inedo.BuildMaster;
using Inedo.BuildMaster.Extensibility.Actions;

namespace Inedo.BuildMasterExtensions.HTTP
{
    /// <summary>
    /// Base class for HTTP actions.
    /// </summary>
    public abstract class HttpActionBase : RemoteActionBase
    {
        /// <summary>
        /// The maximum number of characters to log in a response.
        /// </summary>
        private const int MaxResponseLength = 5000;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpActionBase"/> class.
        /// </summary>
        protected HttpActionBase()
        {
            this.ErrorIfBadStatus = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to log the Content-Body of the response.
        /// </summary>
        [Persistent]
        public bool LogResponseBody { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the action should fail if the response is not a 200.
        /// </summary>
        [Persistent]
        public bool ErrorIfBadStatus { get; set; }

        /// <summary>
        /// Makes the request and handles the response.
        /// </summary>
        /// <param name="request">The request.</param>
        protected void PerformRequest(HttpWebRequest request)
        {
            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
                this.ProcessResponse(response);
            }
            catch (WebException ex)
            {
                this.ProcessResponse((HttpWebResponse)ex.Response);
            }
            finally
            {
                if (response != null)
                    response.Close();
            }
        }

        private void ProcessResponse(HttpWebResponse response)
        {
            if (response.StatusCode != HttpStatusCode.OK)
            {
                var desc = string.Format("Server responded with ({0}): {1}", (int)response.StatusCode, Util.CoalesceStr(response.StatusDescription, response.StatusCode));

                if (this.ErrorIfBadStatus)
                    this.LogError(desc);
                else
                    this.LogWarning("Server responded with ({0}): {1}", (int)response.StatusCode, Util.CoalesceStr(response.StatusDescription, response.StatusCode));
            }
            else
            {
                this.LogInformation("Response received");
            }

            if (this.LogResponseBody)
            {
                if (response.ContentLength == 0)
                {
                    this.LogInformation("The Content Length of the response was 0.");
                    return;
                }

                if (response.ContentType.StartsWith("text/", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        var text = new StreamReader(response.GetResponseStream()).ReadToEnd();
                        bool trunc = false;
                        if (text.Length > MaxResponseLength)
                        {
                            text = text.Substring(0, MaxResponseLength);
                            trunc = true;
                        }

                        if (!string.IsNullOrEmpty(text))
                            this.LogInformation("Response Body: {0}", text);

                        if (trunc)
                            this.LogWarning("Response Body was truncated to {0} characters", MaxResponseLength);
                    }
                    catch (Exception ex)
                    {
                        this.LogWarning("Could not read response content body: {0}", ex.Message);
                    }
                }
                else
                {
                    this.LogWarning("Cannot log the Content Body of response - was expecting a text mime type: got {0}", response.ContentType);
                }
            }
        }
    }
}

using System.Web.UI.WebControls;
using Inedo.BuildMaster.Extensibility.Actions;
using Inedo.BuildMaster.Web.Controls;
using Inedo.BuildMaster.Web.Controls.Extensions;
using Inedo.Web.Controls;

namespace Inedo.BuildMasterExtensions.HTTP
{
    internal sealed class HttpPostActionEditor : ActionEditorBase
    {
        private ValidatingTextBox txtUrl;
        private TextBox txtBody;
        private CheckBox chkLogResponseBody;
        private CheckBox chkErrorIfBadStatus;

        public HttpPostActionEditor()
        {
        }

        public override void BindToForm(ActionBase extension)
        {
            this.EnsureChildControls();

            var action = (HttpPostAction)extension;
            this.txtUrl.Text = action.Url ?? string.Empty;
            this.txtBody.Text = action.PostData ?? string.Empty;
            this.chkLogResponseBody.Checked = action.LogResponseBody;
            this.chkErrorIfBadStatus.Checked = action.ErrorIfBadStatus;
        }
        public override ActionBase CreateFromForm()
        {
            return new HttpPostAction
            {
                Url = this.txtUrl.Text,
                PostData = this.txtBody.Text,
                LogResponseBody = this.chkLogResponseBody.Checked,
                ErrorIfBadStatus = this.chkErrorIfBadStatus.Checked
            };
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            this.txtUrl = new ValidatingTextBox
            {
                Required = true,
                Width = 300
            };

            this.txtBody = new TextBox
            {
                Width = 300,
                TextMode = TextBoxMode.MultiLine,
                Rows = 3
            };

            this.chkErrorIfBadStatus = new CheckBox
            {
                Checked = true,
                Text = "Fail if response status is bad"
            };

            this.chkLogResponseBody = new CheckBox
            {
                Text = "Log Content-Body of response"
            };

            CUtil.Add(this,
                new FormFieldGroup(
                    "URL",
                    "The URL of the resource to get. This should be fully specified; for example, <i>http://example.com/info?rel=%RELNO%</i>",
                    false,
                    new StandardFormField(
                        "URL:",
                        this.txtUrl
                    )
                ),
                new FormFieldGroup(
                    "Data",
                    "Optionally provide data to submit in the request body.",
                    false,
                    new StandardFormField(
                        "Data to Post:",
                        this.txtBody
                    )
                ),
                new FormFieldGroup(
                    "Options",
                    "Additional options which control the action's behavior.",
                    true,
                    new StandardFormField(
                        string.Empty,
                        this.chkErrorIfBadStatus
                    ),
                    new StandardFormField(
                        string.Empty,
                        this.chkLogResponseBody
                    )
                )
            );
        }
    }
}

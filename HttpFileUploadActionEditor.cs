using Inedo.BuildMaster.Extensibility.Actions;
using Inedo.BuildMaster.Web.Controls;
using Inedo.BuildMaster.Web.Controls.Extensions;
using Inedo.Web.Controls;

namespace Inedo.BuildMasterExtensions.HTTP
{
    internal sealed class HttpFileUploadActionEditor : ActionEditorBase
    {
        private ValidatingTextBox txtFileName;
        private ValidatingTextBox txtUrl;

        public override bool DisplaySourceDirectory { get { return true; } }

        protected override void CreateChildControls()
        {
            this.txtFileName = new ValidatingTextBox()
            {
                Width = 300,
                Required = true
            };

            this.txtUrl = new ValidatingTextBox()
            {
                Width = 300,
                Required = true
            };

            CUtil.Add(this,
                new FormFieldGroup(
                    "File Name",
                    "The name of the file to upload relative to the source directory.",
                    false,
                    new StandardFormField(
                        "File Name:",
                        this.txtFileName
                    )
                ),
                new FormFieldGroup(
                    "URL",
                    "The URL where the file will be POSTed. This should be fully specified; for example, <i>http://example.com/upload?app=%APPNAME%</i>",
                    true,
                    new StandardFormField(
                        "URL:",
                        this.txtUrl
                    )
                )
            );
        }

        public override void BindToForm(ActionBase extension)
        {
            var httpFileUploadAction = (HttpFileUploadAction)extension;

            this.txtFileName.Text = httpFileUploadAction.FileName;
            this.txtUrl.Text = httpFileUploadAction.Url;
        }

        public override ActionBase CreateFromForm()
        {
            return new HttpFileUploadAction()
            {
                FileName = this.txtFileName.Text.Trim(),
                Url = this.txtUrl.Text.Trim()
            };
        }
    }
}

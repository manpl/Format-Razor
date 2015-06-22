using System.Management.Automation;

namespace Format_Razor
{
    [Cmdlet(VerbsCommon.Format, "Razor")]
    public class FormatRazor : PSCmdlet
    {
        [Parameter(Mandatory = true,
            Position = 0,
            ValueFromPipeline = true,
            ParameterSetName = "Inline",
            HelpMessage = "Model object to be used within template")]
        [Parameter(ParameterSetName = "External")]
        [ValidateNotNullOrEmpty]
        public object Model { get; set; }

        [Parameter(ParameterSetName="External",
            HelpMessage="Path to external template file")]
        [ValidateNotNullOrEmpty]
        public string TemplatePath { get; set; }

        [Parameter(ParameterSetName="Inline",
            HelpMessage = "Inline template")]
        [ValidateNotNullOrEmpty]
        public string Template { get; set; }

        private bool IsDebug
        {
            get
            {
                return (bool)this.GetVariableValue("PSDebugContext");
            }
        }

        protected override void ProcessRecord()
        {
            WriteDebug("ProcessRecord");
            base.ProcessRecord();
        }
    }
}

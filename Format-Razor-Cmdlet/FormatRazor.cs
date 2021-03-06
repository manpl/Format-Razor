﻿using Format_Razor.RazorEngine;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using System.ComponentModel;
using System.IO;
using System.Management.Automation;

namespace Format_Razor
{
    [Cmdlet(VerbsCommon.Format, "Razor", DefaultParameterSetName = "Inline")]
    [Description("Creates the document using specified template and model")]
    public class FormatRazor : PSCmdlet
    {
        [Parameter(Mandatory = true,
            Position = 0,
            ValueFromPipeline = true,
            HelpMessage = "Model object to be used within template")]
        [ValidateNotNullOrEmpty]
        public object Model { get; set; }

        [Parameter(ParameterSetName = ParamSets.External,
            Mandatory = true,
            HelpMessage = "Path to external template file")]
        [ValidateNotNullOrEmpty]
        public string TemplatePath { get; set; }

        [Parameter(ParameterSetName = ParamSets.Inline,
            Mandatory = true,
            HelpMessage = "Inline template")]
        [ValidateNotNullOrEmpty]
        public string Template { get; set; }

        [Parameter, ValidateSet("C#", "VB")]
        public string Language { get; set; }

        private ITemplateSource templateSource;
        private ITemplateSource TemplateSource
        {
            get
            {
                if (templateSource == null)
                {
                    templateSource = this.ParameterSetName == ParamSets.Inline ?
                        (ITemplateSource)new LoadedTemplateSource(Template) :
                        new FileTemplate(this.TemplatePath);
                }

                return templateSource;
            }
        }

        protected override void BeginProcessing()
        {
            WriteDebug("BeginProcessing paramSet:" + this.ParameterSetName);
            var config = new TemplateServiceConfiguration();

            if ((this.Language ?? "").ToLower() == "vb")
                config.Language = global::RazorEngine.Language.VisualBasic; // VB.NET as template language.
            else
                config.Language = global::RazorEngine.Language.CSharp;

            WriteDebug("Selected language: " + config.Language);

            //config.EncodedStringFactory = new RawStringFactory(); // Raw string encoding.
            //config.EncodedStringFactory = new HtmlEncodedStringFactory(); // Html encoding.
            //config.Debug = IsDebug;
            config.CachingProvider = new DefaultCachingProvider(t => { });
            config.DisableTempFileLocking = true;
            Engine.Razor = RazorEngineService.Create(config); ;


            if (ParameterSetName == ParamSets.External)
            {
                ValidateExternal();
            }

            base.BeginProcessing();
        }

        private void ValidateExternal()
        {
            if (!File.Exists(this.TemplatePath))
            {
                var ex = new FileNotFoundException("template file cannot be found");
                var error = new ErrorRecord(ex, "PathNotFound", ErrorCategory.InvalidArgument, null);
                this.ThrowTerminatingError(error);
            }
        }

        protected override void ProcessRecord()
        {
            WriteDebug("ProcessRecord, paramSet:" + this.ParameterSetName);
            WriteObject(Engine.Razor.RunCompile(TemplateSource, "templateKey", null, Model));
            base.ProcessRecord();
        }
    }
}

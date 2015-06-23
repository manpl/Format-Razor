﻿using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using System;
using System.ComponentModel;
using System.IO;
using System.Management.Automation;

namespace Format_Razor
{
    [Cmdlet(VerbsCommon.Format, "Razor", DefaultParameterSetName="Inline")]
    [Description("Creates the document using specified template and model")]
    public class FormatRazor : PSCmdlet
    {
        [Parameter(Mandatory = true,
            Position = 0,
            ValueFromPipeline = true,
            ParameterSetName = ParamSets.Inline,
            HelpMessage = "Model object to be used within template")]
        [Parameter(ParameterSetName = ParamSets.External)]
        [ValidateNotNullOrEmpty]
        public object Model { get; set; }

        [Parameter(ParameterSetName = ParamSets.External,
            Mandatory=true,
            HelpMessage="Path to external template file")]
        [ValidateNotNullOrEmpty]
        public string TemplatePath { get; set; }

        [Parameter(ParameterSetName = ParamSets.Inline,
            Mandatory=true,
            HelpMessage = "Inline template")]
        [ValidateNotNullOrEmpty]
        public string Template { get; set; }

        protected override void BeginProcessing()
        {
            var config = new TemplateServiceConfiguration();
            //config.Language = Language.VisualBasic; // VB.NET as template language.
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
            WriteObject(Engine.Razor.RunCompile(Template, "templateKey", null, Model));
            base.ProcessRecord();
        }
    }
}

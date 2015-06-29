using RazorEngine.Templating;
using System.IO;

namespace Format_Razor.RazorEngine
{
    public class FileTemplate : ITemplateSource
    {
        public FileTemplate(string path)
        {
            this.TemplateFile = path;
            this.Template = File.ReadAllText(path);
        }

        public System.IO.TextReader GetTemplateReader()
        {
            return new StringReader(this.Template);
        }

        public string Template 
        { 
            get;
            private set;
        }

        public string TemplateFile
        {
            get;
            private set;
        }
    }
}

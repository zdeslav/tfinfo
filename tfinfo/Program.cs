using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotLiquid;
using System.IO;
using System.Reflection;
using CommandLine;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace tfinfo
{
    /*
     todo:
     * allow specifying changeset range
     * support labels
     * support code reviews
     * custom queries
     *   /wiq:"SELECT * FROM WorkItems WHERE [System.TeamProject] = '{project}' ORDER BY [System.Id]"
     *   /csq:""
    */
    class Program
    {
        static int Main(string[] args)
        {
            var options = Options.Parse(args);
            if(options.HaveErrors)
            {
                Console.Out.WriteLine(options.GetUsage());
                return 1;
            }

            try
            {
                var encoding = Encoding.UTF8;
                if (options.CodePage > 0) encoding = Encoding.GetEncoding(options.CodePage);
                Console.OutputEncoding = encoding;
                var templatePath = GetTemplatePath(options.TemplateFile);
                var data = TfsInfo.Collect(options);
                Dump(templatePath, data);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("ERROR: {0}", e.Message);
                return 1;
            }

            return 0;
        }

        private static string GetTemplatePath(string template)
        {
            if (!Path.IsPathRooted(template)) // relative path
            {
                var dir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                template = Path.Combine(dir, "templates", template);
            }

            if (!File.Exists(template))
            {
                throw new Exception(string.Format("Template file '{0}' was not found", template));
            }

            return template;
        }

        private static void Dump(string templatePath, TfsInfo data)
        {
            // otherwise item.SomeProperty would be item.some_property in templates
            Template.NamingConvention = new DotLiquid.NamingConventions.CSharpNamingConvention();
            Template.RegisterFilter(typeof(CustomFilters));

            var tplText = File.ReadAllText(templatePath);
            Template tpl = Template.Parse(tplText);

            var parameters = new RenderParameters() { Context = new Context() };
            parameters.Context.Push(Hash.FromAnonymousObject(new {
                WorkItems = data.WorkItems.ToArray(),
                Changes = data.Changes.ToArray(),
                Iterations = data.Iterations.ToArray()
            }));

            tpl.Render(Console.Out, parameters);
        }
    }
}

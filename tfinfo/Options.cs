using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;

namespace tfinfo
{
    /*
 todo:
 * allow specifying changeset range
 * support labels
 * support code reviews
 * limit history: -m/-max_age 3
 * custom queries
 *   /wiq:"SELECT * FROM WorkItems WHERE [System.TeamProject] = '{project}' ORDER BY [System.Id]"
 *   /csq:""
*/

    class Options
    {
        [Option('c', "collection",  Required = true, HelpText = "TFS project collection")]
        public string Collection { get; set; }

        [Option('p', "project", Required = true, HelpText = "Name of the TFS projects")]
        public string Project { get; set; }

        [Option('b', "branch", Required = true, HelpText = "Name of the TFS branch")]
        public string Branch { get; set; }

        [Option('t', "template", Required = false, DefaultValue = "summary.txt", HelpText = "template file for output")]
        public string TemplateFile { get; set; }

        [Option("nowi", MutuallyExclusiveSet="nowi|nocs", Required = false, HelpText="Skip retrieving work items")]
        public bool NoWorkItems { get; set; }

        [Option("nocs", MutuallyExclusiveSet="nowi|nocs", Required = false, HelpText = "Skip retrieving changesets")]
        public bool NoChangesets { get; set; }

        [Option('m', "max_age", Required = false, HelpText = "NOT IMPLEMENTED! Show only entries from last N days")]
        public int MaxAge { get; set; }

        [Option("cp", Required = false, HelpText = "Number of code page to be used to encode output. The default encoding is UTF-8")]
        public int CodePage { get; set; }

        [Option("wiq", Required = false, HelpText = "NOT IMPLEMENTED! Query to select work items to be shown")]
        public string WorkItemQuery { get; set; }

        [Option("csq", Required = false, HelpText = "NOT IMPLEMENTED! Query to select changesets to be shown")]
        public string ChangesetQuery { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            var help = new HelpText {
                Heading = HeadingInfo.Default,
                Copyright = CopyrightInfo.Default,
                AdditionalNewLineAfterOption = false,
                AddDashesToOption = true,
                MaximumDisplayWidth = 100
            };

            if(LastParserState.Errors.Count > 0)
            {
                HelpText.DefaultParsingErrorsHandler(this, help);
            }

            help.AddPreOptionsLine("Usage: tfinfo -c collection -p project -b branch [-t template] [-m N] [--cp N] [--nowi|nocs]");
            help.AddPreOptionsLine(" ");
            help.AddPreOptionsLine("Parameters:");
            help.AddOptions(this);
            return help;
        }

        internal bool HaveErrors { get { return LastParserState != null && LastParserState.Errors.Count > 0; } }

        internal static Options Parse(string[] args)
        {
            var options = new Options();
            var parser = new CommandLine.Parser(s =>
            {
                s.CaseSensitive = false;
                s.IgnoreUnknownArguments = true;
                s.MutuallyExclusive = true;
            });
            parser.ParseArguments(args, options);
            return options;
        }
    }
}

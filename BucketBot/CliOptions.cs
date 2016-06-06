using CommandLine;
using CommandLine.Text;

namespace BucketBot
{
    public class CliOptions
    {
        [Option('t', "token", HelpText = "The login token for the bot.  You need to get this from the Discord API dashboard.", Required = true)]
        public string LoginToken { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
              current => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}

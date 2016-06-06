using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;

namespace BucketBot
{
    /// <summary>
    /// A discord bot that tells the user of every 5th message that they're dumb.
    /// </summary>
    class Program
    {
        private static int _messageCount;

        static void Main(string[] args)
        {
            var options = new CliOptions();
            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                var configBuilder = new DiscordConfigBuilder();
                configBuilder.LogLevel = LogSeverity.Info;
                Console.WriteLine("Starting Bot... Press CTRL-c exit");
                Run(options.LoginToken, configBuilder.Build()).Wait();
            }
            else
            {
                Console.WriteLine("Press ENTER to exit");
                Console.ReadLine();
            }
        }

        static async Task Run(string loginToken, DiscordConfig config)
        {
            var client = new BucketDiscordClient(config);

            client.OnLogMessage.Subscribe(async e =>
            {
                await Console.Out.WriteLineAsync($"[{e.Severity}] {e.Source}: {e.Message}");
            });

            client.OnMessageReceived.Where(m => !m.User.IsBot)
                .Subscribe(async e =>
                {
                    client.LogInfo($"{e.User.Name} said {e.Message.Text}");
                    Interlocked.Increment(ref _messageCount);
                    if (_messageCount%5 == 0)
                    {
                        await e.Channel.SendMessage($"{e.User.Mention} You're dumb");
                    }
                });

            client.OnReady.Subscribe(e => { client.LogInfo("Client Ready"); });

            client.Connect(loginToken).Subscribe(e =>
            {
                client.LogInfo("Client Connected");
            });

            while (true)
            {
                await Task.Delay(10000);
            }
        }
    }
}
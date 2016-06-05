using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BucketBot
{
    internal class Program
    {
        private static readonly CancellationTokenSource Wtoken = new CancellationTokenSource();
        private static int _messageCount;

        private static void Main()
        {
            Run().Wait();
        }

        private static async Task Run()
        {
            var client = new BucketDiscordClient("MTg5MDg5NzEyNTEyODI3Mzkz.CjYIrA.SO48yZZZgUVAuCbK07EV-MJT7Fc");

            client.OnMessage.Where(m => !m.User.IsBot)
                .Subscribe(async e =>
                {
                    Interlocked.Increment(ref _messageCount);
                    if (_messageCount%5 == 0)
                    {
                        await e.Channel.SendMessage($"{e.User.Mention} You're dumb");
                    }
                });

            client.OnReady.Subscribe(async e => { await Console.Out.WriteLineAsync("ready"); });
            client.OnConnect.Subscribe(async e => { await Console.Out.WriteLineAsync("Connected"); });

            while (true)
            {
                await Task.Delay(10000, Wtoken.Token);
            }
        }
    }
}
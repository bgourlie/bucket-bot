using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Discord;

namespace BucketBot
{
    /// <summary>
    ///     Wrap the DiscordClient and provide IObservables instead of events
    /// </summary>
    public class BucketDiscordClient
    {
        private readonly DiscordClient _wrapped;
        public readonly IObservable<Unit> OnConnect;
        public readonly IObservable<MessageEventArgs> OnMessage;
        public readonly IObservable<object> OnReady;

        public BucketDiscordClient(string botToken)
        {
            _wrapped = new DiscordClient();
            OnConnect = _wrapped.Connect(botToken).ToObservable();
            OnReady = Observable.FromEventPattern(h => _wrapped.Ready += h, h => _wrapped.Ready -= h);
            OnMessage = Observable.FromEventPattern<MessageEventArgs>(h => _wrapped.MessageReceived += h,
                h => _wrapped.MessageReceived -= h).Select(o => o.EventArgs);
        }
    }
}
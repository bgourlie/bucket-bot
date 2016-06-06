using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Discord;

namespace BucketBot
{
    /// <summary>
    ///     Wrap the DiscordClient and provide a reactive api
    /// </summary>
    public class BucketDiscordClient
    {
        private readonly DiscordClient _wrapped;
        public readonly IObservable<LogMessageEventArgs> OnLogMessage;
        public readonly IObservable<MessageEventArgs> OnMessageReceived;
        public readonly IObservable<object> OnReady;
        public readonly IObservable<ServerEventArgs> OnServerJoined;
        public readonly IObservable<UserEventArgs> OnUserJoined;

        public BucketDiscordClient() : this(null)
        {
        }

        public BucketDiscordClient(DiscordConfig config)
        {
            _wrapped = config != null
                ? new DiscordClient(config)
                : new DiscordClient();

            OnReady = Observable.FromEventPattern(h => _wrapped.Ready += h, h => _wrapped.Ready -= h);

            OnMessageReceived = Observable.FromEventPattern<MessageEventArgs>(h => _wrapped.MessageReceived += h,
                h => _wrapped.MessageReceived -= h).Select(o => o.EventArgs);

            OnServerJoined =
                Observable.FromEventPattern<ServerEventArgs>(h => _wrapped.JoinedServer += h,
                    h => _wrapped.JoinedServer -= h).Select(o => o.EventArgs);

            OnUserJoined =
                Observable.FromEventPattern<UserEventArgs>(h => _wrapped.UserJoined += h, h => _wrapped.UserJoined -= h)
                    .Select(e => e.EventArgs);

            OnLogMessage =
                Observable.FromEventPattern<LogMessageEventArgs>(h => _wrapped.Log.Message += h,
                    h => _wrapped.Log.Message -= h).Select(e => e.EventArgs);
        }

        public IObservable<Unit> Connect(string botToken)
        {
            return _wrapped.Connect(botToken).ToObservable();
        }

        public void LogDebug(string message, Exception exception = null)
        {
            Log(LogSeverity.Debug, message, exception);
        }

        public void LogInfo(string message, Exception exception = null)
        {
            Log(LogSeverity.Info, message, exception);
        }

        public void LogWarning(string message, Exception exception = null)
        {
            Log(LogSeverity.Warning, message, exception);
        }

        public void LogError(string message, Exception exception = null)
        {
            Log(LogSeverity.Error, message, exception);
        }

        private void Log(LogSeverity severity, string message, Exception exception = null)
        {
            _wrapped.Log.Log(severity, "BucketDiscordClient", message, exception);
        }
    }
}
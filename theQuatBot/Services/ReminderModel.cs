using DSharpPlus.CommandsNext;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TheQuatBot.Services
{
    public class Remindermodel
    {
        private int _scnds;
        public string _msg { private set; get; }
        private CommandContext _ctx;
        public CancellationTokenSource cts { private set; get; }
        public bool IsCancelled { private set; get; } = false;
        public bool IsSet { private set; get; } = false;
        public DateTime startTime { private set; get; }

        public Remindermodel(int scnds, string msg, CommandContext ctx)
        {
            _scnds = scnds;
            _msg = msg;
            _ctx = ctx;
            startTime = DateTime.Now; // might wanna use to set up a time remaining thing
        }

        public void Set() //sets the reminder first
        {
            cts = new CancellationTokenSource();
            if (_msg == null)
                _msg = "No Message";
            RT(async () => {
                await _ctx.Channel.SendMessageAsync($"{_ctx.User.Mention}, you asked me to remind you to `{_msg}`").ConfigureAwait(false);
                cts.Cancel();
            }, _scnds, cts.Token);
            IsSet = true; // just to try and check if it is set and can notify the outside
        }

        public void Cancel() //cancels the task using the token,, so it can be called outside
        {
            cts.Cancel();
            IsCancelled = true;
        }

        private static void RT(Action action, int seconds, CancellationToken token)
        {
            if (action == null) // no action (no action)
                return;

            Task.Run(async () =>
            { 
                while (!token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                    await Task.Delay(TimeSpan.FromSeconds(seconds), token);
                    action();
                }
            }, token);
        }

    }
}

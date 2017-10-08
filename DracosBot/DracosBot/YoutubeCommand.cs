using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;
using Discord.WebSocket;
using Discord.Rest;
namespace DracosBot
{
    internal class YoutubeCommand : Program
    {
        private DiscordSocketClient _clientThis;
        private IVoiceChannel _voiceChannel;
        private ITextChannel _textChannel;
        private TaskCompletionSource<bool> _tsc;
        private IAudioClient _audio;
        private CancellationTokenSource _disposeToken;
        private Queue<Tuple<string, string, string, string>> _queue;

        public static string[] command = { "!youtube" };



        /// <summary>
        /// Tuple(FilePath, Video Name, Duration, Requested by)
        /// </summary>
        private bool Pause
        {
            get => _internal_Pause;
            set
            {
                new Thread(() => _tsc.TrySetResult(value)).Start();
                _internal_Pause = value;
            }
        }
        private bool _internal_Pause;
        private bool Skip
        {
            get
            {
                bool ret = _internal_Skip;
                _internal_Skip = false;
                return ret;
            }
            set => _internal_Skip = value;
        }
        private bool _internal_Skip;

        public bool isDisposed;

        public YoutubeCommand()
        {
            Initialize();
        }
        public async void Initialize()
        {
            _queue = new Queue<Tuple<string, string, string, string>>();
            _tsc = new TaskCompletionSource<bool>();
            _disposeToken = new CancellationTokenSource();
            _clientThis = _client;
        }

    }
}

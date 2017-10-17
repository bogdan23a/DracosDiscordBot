using Discord;
using Discord.Audio;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DracosBot
{  

    class Program  
    {
        private IVoiceChannel _voiceChannel;
        private ITextChannel _textChannel;
        private IAudioClient _audio;
        private DiscordSocketClient _client;
        private List<string> _permittedUsers;
        private TaskCompletionSource<bool> _tcs;
        private CancellationTokenSource _disposeToken;
        private const string ImABot = " *I'm a Bot, beep boop blop*";
        private readonly string[] _commands = { "!help", "!queue", "!add", "!addPlaylist", "!pause", "!play", "!clear", "!come", "!update", "!skip" };
        /// <summary>
        /// Tuple(FilePath, Video Name, Duration, Requested by)
        /// </summary>
        private Queue<Tuple<string, string, string, string>> _queue;

       /* private bool Pause
        {
            get => _internal_Pause;
            set
            {
                new Thread(() => _tsc.TrySetResult(value)).Start();
                _internal_Pause = value;
            }
        }*/
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

        public object _tsc { get; private set; }

        private bool _internal_Skip;

        public bool isDisposed;

        //9GagSubBot 9gagBot;
        static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();
        
        public static void Print(string message,ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }
        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();

            _client.Log += Log;
            _client.MessageReceived += OnMessageReceived;

            //-
            _client.Disconnected += Disconnected;

            //+
            _client.Connected += Connected;
            _client.Ready += OnReady;
            //TOKEN STORING LOGIC 
            //Store the token in a txt file to avoid hardcoding it 
            //User confirmation needed
            string assumedToken = string.Empty;
            string token = string.Empty;
            bool hasTokenChanged = false;
            
            //First check if file exists 
            if (File.Exists("token.txt"))
            {
                //read the token and ask the user to confirm it 
                //if it is not the same token keep asking for the token until it is the right one

                assumedToken = File.ReadAllText("token.txt");
                Console.WriteLine("Confirm token : " + assumedToken);
                Console.WriteLine("Y/N");      
                
                while (Console.ReadLine() == "N")
                {
                    hasTokenChanged = true;
                    Console.WriteLine("Enter your token below :");
                    assumedToken = Console.ReadLine();
                    Console.WriteLine("Confirm token : " + assumedToken);
                    Console.WriteLine("Y/N");
                }
                //finally assign the true value for the token
                //and if the token changed update the file

                token = assumedToken;
                if (hasTokenChanged)
                    using (StreamWriter stream = new StreamWriter("myfile.txt"))
                    {
                        stream.WriteLine(token);
                    }
            }
            //if it does not exist it means it's the first time the user boots 
            else
            {
                //ask for the token and the for the confirmation 
                //if it is wrong keep asking for it until it is the right one

                Console.WriteLine("Enter your token below :");
                assumedToken = Console.ReadLine();
                Console.WriteLine("Confirm token : " + assumedToken);
                Console.WriteLine("Y/N");

                while(Console.ReadLine() == "N")
                {
                    Console.WriteLine("Enter your token below :");
                    assumedToken = Console.ReadLine();
                    Console.WriteLine("Confirm token : " + assumedToken);
                    Console.WriteLine("Y/N");
                }
                //finally assign the true value for the token
                //also create the file to store the token and write the token to it
                token = assumedToken;
                var tokenFile = File.Create("token.txt");
                tokenFile.Close();

                using (StreamWriter stream = new StreamWriter("token.txt"))
                {
                    stream.WriteLine(token);
                }
            }


            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            //Block this task until the program is closed
            await Task.Delay(-1);
        }
        //Pass MessageReceived Event on to Async Method (so nothing blocks)
        private Task OnMessageReceived(SocketMessage message)
        {
            MessageReceived(message);
            return Task.CompletedTask;
        }
        /*private async Task MessageReceived(SocketMessage message)
        {
            Print($"User \"{message.Author}\" wrote: \"{message.Content}\"", ConsoleColor.Magenta);

            if (message.Content == Command.PING_COMMAND)
                await message.Channel.SendMessageAsync(Command.PING_ANSWER);
            else
            if (Command.isCommand(Command.ROBERT_COMMANDS, message.Content))
                await message.Channel.SendMessageAsync(Command.ROBERT_ANSWER);
            else
            if (Command.getIndex(Command.GAG_COMMANDS, message.Content) != -1)
            {
                int gagCommand = Command.getIndex(Command.GAG_COMMANDS, message.Content);
                //await message.Channel.SendMessageAsync("MEMAGE https://www.youtube.com/watch?v=-Qh41BTkqGU");
                await message.Channel.SendFileAsync(Command.GAG_ANSWERS(gagCommand)[0]);
            }
            if (message.Content == "!play Mia")
                await message.Channel.SendFileAsync("Timeflies - Mia Khalifa Lyrics-BcKKSH_aOEU.mp4");
        }*/
        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        #region Events

        //Connection Lost
        private static Task Disconnected(Exception arg)
        {
            Print($"Connection lost! ({arg.Message})", ConsoleColor.Red);
            return Task.CompletedTask;
        }

        //Connected
        private static Task Connected()
        {
            //Console.Title = "Music Bot (Connected)";

            Print("Connected!", ConsoleColor.Green);
            return Task.CompletedTask;
        }


        //Pass Ready Event on to Async Method (so nothing blocks)
        private Task OnReady()
        {
            Ready();
            return Task.CompletedTask;
        }

        //On Bot ready
        private async void Ready()
        {
            Print("Ready!", ConsoleColor.Green);

            //"Playing Nothing :/"
            await _client.SetGameAsync("Nothing :/");

            //Get Guilds / Servers
            try
            {
                //Server
                PrintServers();
                SocketGuild guild = _client.Guilds.FirstOrDefault(g => g.Name == Information.ServerName);

                //Text Channel
                _textChannel = guild.TextChannels.FirstOrDefault(t => t.Name == Information.TextChannelName);
                Print($"Using Text Channel: \"#{_textChannel.Name}\"", ConsoleColor.Cyan);

                //Voice Channel
                _voiceChannel = guild.VoiceChannels.FirstOrDefault(t => t.Name == Information.VoiceChannelName);
                Print($"Using Voice Channel: \"{_voiceChannel.Name}\"", ConsoleColor.Cyan);
                _audio = await _voiceChannel.ConnectAsync();
            }
            catch (Exception e)
            {
                Print("Could not join Voice/Text Channel (" + e.Message + ")", ConsoleColor.Red);
            }
        }
        private void PrintServers()
        {
            //Print added Servers
            Print("\n\rAdded Servers:", ConsoleColor.Cyan);
            foreach (SocketGuild server in _client.Guilds)
            {
                Print(server.Name == Information.ServerName
                    ? $" [x] {server.Name}"
                    : $" [ ] {server.Name}", ConsoleColor.Cyan);
            }
            Print("", ConsoleColor.Cyan);
        }
       

        //On Message Received (async)
        private async void MessageReceived(SocketMessage socketMsg)
        {
            try
            {
                #region Message Filtering

                //Avoid receiving own messages
                /*if (socketMsg.Author.Id == _client.CurrentUser.Id)
                {
                    return;
                }*/

                Print($"User \"{socketMsg.Author}\" wrote: \"{socketMsg.Content}\"", ConsoleColor.Magenta);

                //Shorter var name
                string msg = socketMsg.Content;
                //Is MusicBot Command

                bool isCmd = _commands.Any(c => msg.StartsWith(c));

                //If is a supported command
                /*if (isCmd)
                {
                    //Avoid Spam in #general if Channel is #general
                    if (socketMsg.Channel.Name == "general")
                    {
                        await socketMsg.DeleteAsync();
                        //await e.Channel.SendMessage("Wrong Channel!");
                        return;
                    }
                }
                //If not a supported command
                else
                {
                    if (socketMsg.Channel.Name == Information.TextChannelName)
                    {
                        //Not a command
                        await socketMsg.DeleteAsync();
                    }
                    return;
                }
                */
                #endregion

                //Direct Message Channel to Message Author
                RestDMChannel dm = (RestDMChannel)await socketMsg.Author.GetOrCreateDMChannelAsync();

                //Delete Message to avoid Spam
                try
                {
                    await socketMsg.DeleteAsync();
                }
                catch
                {
                    // not allowed
                }

                #region For All Users

                if (msg.StartsWith("!help"))
                {
                    Print("User requested: Help", ConsoleColor.Magenta);
                    //Print Available Commands
                    await dm.SendMessageAsync(
                        $"Use these *Commands* by sending me a **private Message**, or writing in **#{Information.TextChannelName}**!" + ImABot,
                        embed: GetHelp(socketMsg.Author.ToString()));
                    return;
                }
                else if (msg.StartsWith("!queue"))
                {
                    Print("User requested: Queue", ConsoleColor.Magenta);
                    //Print Song Queue
                    await SendQueue(_textChannel);
                    return;
                }

                #endregion

                #region Only with Roles

                if (!_permittedUsers.Contains(socketMsg.Author.ToString()))
                {
                    await dm.SendMessageAsync("Sorry, but you're not allowed to do that!" + ImABot);
                    return;
                }

                string[] split = msg.Split(' ');
                string command = split[0].ToLower();
                string parameter = null;
                if (split.Length > 1)
                    parameter = split[1];



                switch (command)
                {
                    #region !add

                    case "!add":
                        //Add Song to Queue
                        if (parameter != null)
                        {
                            using (_textChannel.EnterTypingState())
                            {

                                //Test for valid URL
                                bool result = Uri.TryCreate(parameter, UriKind.Absolute, out Uri uriResult)
                                          && (uriResult.Scheme == "http" || uriResult.Scheme == "https");

                                //Answer
                                if (result)
                                {
                                    try
                                    {
                                        Print("Downloading Video...", ConsoleColor.Magenta);

                                        Tuple<string, string> info = await DownloadHelper.GetInfo(parameter);
                                        await SendMessage($"<@{socketMsg.Author.Id}> requested \"{info.Item1}\" ({info.Item2})! Downloading now..." +
                                                          ImABot);

                                        //Download
                                        string file = await DownloadHelper.Download(parameter);
                                        var vidInfo = new Tuple<string, string, string, string>(file, info.Item1, info.Item2, socketMsg.Author.ToString());

                                        _queue.Enqueue(vidInfo);
                                        //Pause = false;
                                        Print($"Song added to playlist! ({vidInfo.Item2} ({vidInfo.Item3}))!", ConsoleColor.Magenta);
                                    }
                                    catch (Exception ex)
                                    {
                                        Print($"Could not download Song! {ex.Message}", ConsoleColor.Red);
                                        await SendMessage(
                                            $"Sorry <@{socketMsg.Author.Id}>, unfortunately I can't play that Song!" +
                                            ImABot);
                                    }
                                }
                                else
                                {
                                    await _textChannel.SendMessageAsync(
                                        $"Sorry <@{socketMsg.Author.Id}>, but that was not a valid URL!" + ImABot);
                                }
                            }
                        }
                        break;

                    #endregion

                    #region !addPlaylist

                    case "!addPlaylist":
                        //Add Song to Queue
                        if (parameter != null)
                        {
                            using (_textChannel.EnterTypingState())
                            {

                                //Test for valid URL
                                bool result = Uri.TryCreate(parameter, UriKind.Absolute, out Uri uriResult)
                                              && (uriResult.Scheme == "http" || uriResult.Scheme == "https");

                                //Answer
                                if (result)
                                {
                                    try
                                    {
                                        Print("Downloading Playlist...", ConsoleColor.Magenta);

                                        Tuple<string, string> info = await DownloadHelper.GetInfo(parameter);
                                        await SendMessage($"<@{socketMsg.Author.Id}> requested Playlist \"{info.Item1}\" ({info.Item2})! Downloading now..." +
                                                          ImABot);

                                        //Download
                                        string file = await DownloadHelper.DownloadPlaylist(parameter);
                                        var vidInfo = new Tuple<string, string, string, string>(file, info.Item1, info.Item2, socketMsg.Author.ToString());

                                        _queue.Enqueue(vidInfo);
                                        //Pause = false;
                                        Print($"Playlist added to playlist! (\"{vidInfo.Item2}\" ({vidInfo.Item2}))!", ConsoleColor.Magenta);
                                    }
                                    catch (Exception ex)
                                    {
                                        Print($"Could not download Playlist! {ex.Message}", ConsoleColor.Red);
                                        await SendMessage(
                                            $"Sorry <@{socketMsg.Author.Id}>, unfortunately I can't play that Playlist!" +
                                            ImABot);
                                    }
                                }
                                else
                                {
                                    await _textChannel.SendMessageAsync(
                                        $"Sorry <@{socketMsg.Author.Id}>, but that was not a valid URL!" + ImABot);
                                }
                            }
                        }
                        break;

                    #endregion

                    #region !pause

                    case "!pause":
                        //Pause Song Playback
                        //Pause = true;
                        Print("Playback paused!", ConsoleColor.Magenta);
                        await _textChannel.SendMessageAsync($"<@{socketMsg.Author}> paused playback!" + ImABot);
                        break;

                    #endregion

                    #region !play

                    case "!play":
                        //Continue Song Playback
                        //Pause = false;
                        Print("Playback continued!", ConsoleColor.Magenta);
                        await _textChannel.SendMessageAsync($"<@{socketMsg.Author}> resumed playback!" + ImABot);
                        break;

                    #endregion

                    #region !clear

                    case "!clear":
                        //Clear Queue
                        //Pause = true;
                        _queue.Clear();
                        Print("Playlist cleared!", ConsoleColor.Magenta);
                        await SendMessage(
                            $"<@{socketMsg.Author.Id}> cleared the Playlist!" + ImABot);
                        break;

                    #endregion

                    #region !come

                    case "!come":
                        _audio?.Dispose();
                        _voiceChannel = (socketMsg.Author as IGuildUser)?.VoiceChannel;
                        if (_voiceChannel == null)
                        {
                            Print("Error joining Voice Channel!", ConsoleColor.Red);
                            await socketMsg.Channel.SendMessageAsync($"I can't connect to your Voice Channel <@{socketMsg.Author}>!" + ImABot);
                        }
                        else
                        {
                            Print($"Joined Voice Channel \"{_voiceChannel.Name}\"", ConsoleColor.Magenta);
                            _audio = await _voiceChannel.ConnectAsync();
                        }
                        break;

                    #endregion

                    #region !update

                    case "!update":
                        //Update Config
                        ReadConfig();
                        Print("User Config Updated!", ConsoleColor.Magenta);
                        await dm.SendMessageAsync("Updated Permitted Users List!");
                        break;

                    #endregion

                    #region !skip

                    case "!skip":
                        Print("Song Skipped!", ConsoleColor.Magenta);
                        await _textChannel.SendMessageAsync($"<@{socketMsg.Author}> skipped **{_queue.Peek().Item2}**!");
                        //Skip current Song
                        Skip = true;
                        //Pause = false;
                        break;

                    #endregion

                    default:
                        // no command
                        break;
                }

                #endregion

            }
            catch (Exception ex)
            {
                Print(ex.Message, ConsoleColor.Red);
            }
        }

        #endregion

        //Return Bot Help
        public Embed GetHelp(string user)
        {
            EmbedBuilder builder = new EmbedBuilder()
            {
                Title = "Music Bot Help",
                Description = _permittedUsers.Contains(user) ?
                                    "You are allowed to use **every** command." :
                                    "You are only allowed to use `!help` and `!queue`",
                Color = new Color(102, 153, 255)
            };
            //builder.ThumbnailUrl = "https://raw.githubusercontent.com/mrousavy/DiscordMusicBot/master/DiscordMusicBot/disc.png"; //Music Bot Icon
            builder.Url = "http://github.com/mrousavy/DiscordMusicBot";

            builder.AddField("`!help`", "Prints available Commands and usage");
            builder.AddField("`!queue`", "Prints all queued Songs & their User");

            builder.AddField("`!add [url]`", "Adds a single Song to Music-queue");
            builder.AddField("`!addPlaylist [url]`", "Adds whole playlist to Music-queue");
            builder.AddField("`!pause`", "Pause the queue and current Song");
            builder.AddField("`!play`", "Resume the queue and current Song");
            builder.AddField("`!clear`", "Clear queue and current Song");
            builder.AddField("`!come`", "Let Bot join your Channel");
            builder.AddField("`!update`", "Updates Permitted Clients from File");


            return builder.Build();
        }
        //Send Song queue in channel
        private async Task SendQueue(IMessageChannel channel)
        {
            EmbedBuilder builder = new EmbedBuilder()
            {
                Author = new EmbedAuthorBuilder { Name = "Music Bot Song Queue" },
                Footer = new EmbedFooterBuilder() { Text = "(I don't actually sing)" },
                //Color = Pause ? new Color(244, 67, 54) /*Red*/ : new Color(00, 99, 33) /*Green*/
            };
            //builder.ThumbnailUrl = "some cool url";
            builder.Url = "http://github.com/mrousavy/DiscordMusicBot";

            if (_queue.Count == 0)
            {
                await channel.SendMessageAsync("Sorry, Song Queue is empty! Add some songs with the `!add [url]` command!" + ImABot);
            }
            else
            {
                foreach (Tuple<string, string, string, string> song in _queue)
                {
                    builder.AddField($"{song.Item2} ({song.Item3})", $"by {song.Item4}");
                }

                await channel.SendMessageAsync("", embed: builder.Build());
            }
        }
        //Send Message to channel
        public async Task SendMessage(string message)
        {
            if (_textChannel != null)
                await _textChannel.SendMessageAsync(message);
        }

        //Read Config from File
        public void ReadConfig()
        {
            if (!File.Exists("users.txt"))
                File.Create("users.txt").Dispose();

            _permittedUsers = new List<string>(File.ReadAllLines("users.txt"));


            string msg = _permittedUsers.Aggregate("Permitted Users:\n\r    ", (current, user) => current + (user + ", "));
            Print(msg, ConsoleColor.Cyan);
        }

    }
}

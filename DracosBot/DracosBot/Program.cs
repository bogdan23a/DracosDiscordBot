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


        //9GagSubBot 9gagBot;
        static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        public static void Print(string message, ConsoleColor color)
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
                    using (StreamWriter stream = new StreamWriter("token.txt"))
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

                while (Console.ReadLine() == "N")
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
        private async Task MessageReceived(SocketMessage message)
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
                _9gagCommand.clickDreaptaDelete(Command.GAG_ANSWERS(gagCommand)[0]);
            }
        }
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

    }
}
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;

namespace DracosBot
{
    class Program
    {
        private DiscordSocketClient _client;
        static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();
        
        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();

            _client.Log += Log;
            _client.MessageReceived += MessageReceived;

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

        private async Task MessageReceived(SocketMessage message)
        {
            if(message.Content == "!ping")
            {
                await message.Channel.SendMessageAsync("Pong!");
            }
        }
        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}

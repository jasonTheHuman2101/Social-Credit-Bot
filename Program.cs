using System;
using System.Threading.Tasks;
using System.Threading;
using Discord;
using Discord.WebSocket;

namespace SocCredBotV1
{
    internal class Program
    {
        private static DiscordSocketClient _client;
        public static bool isReadyForConnection = false;
        public static string token = "NzYyMjcwOTcxNDk4ODU2NDYw.X3muKQ.xshaiwieur cw4gBeKeSMfnyj67M4q0er2CM84"; //edit before use

        public static async Task Main(string[] args)
        {
            new Program().MainAsync();

            while (isReadyForConnection == false)
            {

            }
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Ready. Enter a command at any time to run it.");
            Console.ForegroundColor = ConsoleColor.White;

            while (true)
            {
                string command = Console.ReadLine();

                switch (command)
                {
                    case "start":
                        await _client.StartAsync();
                        Console.WriteLine("Attempting to log in and start...");
                        break;
                    case "stop":
                        await _client.StopAsync();
                        Console.WriteLine("Attempting to stop client...");
                        break;
                    case "gen":
                        CreditScoresIO.genfile();
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Unknown Command");
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                }
            }
        }

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();

            _client.Log += Log;

            //  You can assign your bot token to a string, and pass that in to connect.
            //  This is, however, insecure, particularly if you plan to have your code hosted in a public repository.
            var token = "NzYyMjcwOTcxNDk4ODU2NDYw.X3muKQ.cw4gBeKeSMfnyj67M4q0er2CM84";

            // Some alternative options would be to keep your token in an Environment Variable or a standalone file.
            // var token = Environment.GetEnvironmentVariable("NameOfYourEnvironmentVariable");
            // var token = File.ReadAllText("token.txt");
            // var token = JsonConvert.DeserializeObject<AConfigurationClass>(File.ReadAllText("config.json")).Token;

            await _client.LoginAsync(TokenType.Bot, token);

            isReadyForConnection = true;

            _client.MessageReceived += Messagerecv;
            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        private Task Messagerecv(SocketMessage arg)//This thing will run on every message recieved.
        {
            Console.WriteLine(arg.Content);
            Console.WriteLine(arg.Author);
            //string sender = arg.Author.Replace("?", "");
            string sender = arg.Author.Username;

            if (sender == "Giga Credit") { }//do nowt
            else
            {
                scResult pointstoremove = StringChecker.CheckString(arg.Content);
                if (pointstoremove.score == 0) { Console.WriteLine("None to remove"); }
                else
                {
                    int newScore = CreditScoresIO.ModifyCreditScoreValue(sender, pointstoremove.score);
                    //If there are points to remove, they will be here
                    arg.Channel.SendMessageAsync("ATTENTION CITIZEN: YOU HAVE BEEN DEDUCTED " + pointstoremove.score + " POINTS FOR SAYING:");
                    foreach(string word in pointstoremove.words)
                    {
                        arg.Channel.SendMessageAsync(word);
                    }
                    arg.Channel.SendMessageAsync("You are now on " + newScore + " points. Be careful, citizen.");
                }
            }

            return Task.CompletedTask;
        }

        private Task Log(LogMessage msg)
        {
            Console.ForegroundColor = System.ConsoleColor.Cyan;
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
            Console.ForegroundColor = System.ConsoleColor.White;
        }
    }
}

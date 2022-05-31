using System;
using System.Threading.Tasks;
using System.Threading;
using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using System.IO;

namespace SocCredBotV1
{
    internal class Program
    {
        public static DiscordSocketClient _client;
        public static ulong id = Convert.ToUInt64(File.ReadAllText("channel.txt"));
        public static bool isReadyForConnection = false;
        //public static string token = "NzYyMjcwOTcxNDk4ODU2NDYw.X3muKQ.7XBGVTxwfZhRF0zzxdOY8tF4AWk"; //edit before use
        public static string token = File.ReadAllText("token.txt");
        public static bool HasDailySummaryPosted = false;

        public static async Task Main(string[] args)
        {
            new Program().MainAsync();

            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 60000;
            timer.Elapsed += CheckForTime;
                        
            while (isReadyForConnection == false)
            {

            }
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Ready. Enter a command at any time to run it.");
            Console.ForegroundColor = ConsoleColor.White;

            while (true)//Commands
            {
                //Console.WriteLine(DateTime.Now.Hour.ToString());
                string command = Console.ReadLine();

                switch (command)
                {
                    case "start":
                        timer.Enabled = true;
                        await _client.StartAsync();
                        Console.WriteLine("Attempting to log in and start...");
                        break;
                    case "stop":
                        timer.Enabled = false;
                        await _client.StopAsync();
                        Console.WriteLine("Attempting to stop client...");
                        break;
                    case "gen":
                        string path = WeeklySummaryGenerator.generateImage();
                        var chnl = _client.GetChannel(id) as IMessageChannel; // 4
                        await chnl.SendFileAsync(path);
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Unknown Command");
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                }
            }
        }

        private static void CheckForTime(object sender, System.Timers.ElapsedEventArgs e)
        {
            DoWeNeedADailySummaryYet();
        }

        public static void DoWeNeedADailySummaryYet()
        {
            //get the time
            DateTime time = DateTime.Now;
            int hours = time.Hour;
            int minutes = time.Minute;

            //Check if the daily summary has been posted
            if (HasDailySummaryPosted == false)
            {
                if (hours == 18 && minutes == 30)
                {
                    if (time.DayOfWeek == DayOfWeek.Sunday)
                    {
                        //Weekly Summary
                        string path = WeeklySummaryGenerator.generateImage();
                        var chnl = _client.GetChannel(id) as IMessageChannel;
                        chnl.SendFileAsync(path);
                        ResetCreditScores();
                    }
                    else
                    {
                        //Daily summary
                        var chnl = _client.GetChannel(id) as IMessageChannel;
                        chnl.SendMessageAsync("Daily Credit Score Summary");
                        chnl.SendMessageAsync(DailySummaryCreator.GenerateSummaryText());
                    }
                    //HasDailySummaryPosted = true;
                }
                
            }
            //else{ if (hours == 18 && minutes == 0) { } else{ HasDailySummaryPosted = false; } }
        }

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();

            _client.Log += Log;

            //  You can assign your bot token to a string, and pass that in to connect.
            //  This is, however, insecure, particularly if you plan to have your code hosted in a public repository.
            //var token = "NzYyMjcwOTcxNDk4ODU2NDYw.X3muKQ.cw4gBeKeSMfnyj67M4q0er2CM84";

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

            if (sender == "LADPoints") { }//do nowt
            else
            {
                if (arg.Channel.Id == 780786104139120651) { }//ignore SCC
                else
                {
                    scResult pointstoremove = StringChecker.CheckString(arg.Content.ToLower());
                    if (pointstoremove.score == 0) { Console.WriteLine("None to remove"); }
                    else
                    {
                        int newScore = CreditScoresIO.ModifyCreditScoreValue(sender, pointstoremove.score);
                        //If there are points to remove, they will be here
                        arg.Channel.SendMessageAsync("Alrite there lad, blud finna lose " + pointstoremove.score + " cred for sayin:");
                        Thread.Sleep(250);
                        foreach (string word in pointstoremove.words)
                        {
                            arg.Channel.SendMessageAsync(word);
                            Thread.Sleep(250);
                        }
                        arg.Channel.SendMessageAsync("Mans on " + newScore + " creds now. Watch it, capiche?");
                    }
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

        private static void ResetCreditScores()
        {
            string json = File.ReadAllText("scores.json");
            CreditScoreFile scoreFile = JsonConvert.DeserializeObject<CreditScoreFile>(json);

            foreach(CreditScores cs in scoreFile.CreditScores)
            {
                cs.score = 100;
            }

            string newFile = JsonConvert.SerializeObject(scoreFile);
            File.WriteAllText("scores.json", newFile);
        }
    }
}

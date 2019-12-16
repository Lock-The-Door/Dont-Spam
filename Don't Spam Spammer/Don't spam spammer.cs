using Discord;
using System.Timers;
using System.Threading.Tasks;
using System.Collections.Generic;
using Discord.WebSocket;
using System;

namespace Discord_Bot_Template
{
    public static class DontSpamSpammer
    {
        public static List<Timer> spamTimers = new List<Timer>();
        public static Dictionary<int, Timer> userPunishmentLengths = new Dictionary<int, Timer>();
        public static Dictionary<int, System.Diagnostics.Stopwatch> punishmentIdElapsedTimePairs = new Dictionary<int, System.Diagnostics.Stopwatch>();
        public static Dictionary<IMessageChannel, int> channelTreatmentIdPairs = new Dictionary<IMessageChannel, int>();
        public static Dictionary<IUser, int> userPunishmentIdPairs = new Dictionary<IUser, int>();

        public static void SpamChannel(IMessageChannel spamChannel)
        {
            spamTimers.Add(new Timer(10000));

            int timerNumber = spamTimers.Count - 1;

            channelTreatmentIdPairs.Add(spamChannel, timerNumber);

            spamTimers[timerNumber].Elapsed += delegate (object sender, ElapsedEventArgs e) { SpamChannelTimer_Elapsed(sender, e, spamChannel); };

            spamTimers[timerNumber].Start();
        }

        public static void SpamUser(IUser infringer, double length, SocketMessage commandUsage)
        {
            //set up spam timer
            spamTimers.Add(new Timer(5000));
            int timerNumber = spamTimers.Count - 1;
            //set up time up timer
            int userTimerNumber;
            if (userPunishmentIdPairs.TryGetValue(infringer, out int oldId))
            {
                //modify the user id to match the new one
                userPunishmentIdPairs[infringer] = timerNumber;
            }
            else
            {
                //link the id to the user
                userPunishmentIdPairs.Add(infringer, timerNumber);
            }
            try
            {
                
                userPunishmentLengths.Add(timerNumber, new Timer(length));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                commandUsage.Channel.SendMessageAsync("I encountered an error: " + e.Message);
                return;
            }
            //set reference number
            userTimerNumber = userPunishmentLengths.Count - 1;
            //turn off auto-reset
            userPunishmentLengths[timerNumber].AutoReset = false;
            //subscribe to events
            spamTimers[timerNumber].Elapsed += delegate (object sender, ElapsedEventArgs e) { SpamUserTimer_Elapsed(sender, e, infringer); };
            userPunishmentLengths[timerNumber].Elapsed += delegate (object sender, ElapsedEventArgs e) { SpamUserStop(timerNumber, infringer, $"your {punishmentIdElapsedTimePairs[timerNumber].ElapsedMilliseconds / 1000} seconds is up!"); };
            //setup and start stopwatch
            punishmentIdElapsedTimePairs.Add(timerNumber, new System.Diagnostics.Stopwatch());
            punishmentIdElapsedTimePairs[timerNumber].Start();
            //start spam timer
            spamTimers[timerNumber].Start();
            //start time up timer
            userPunishmentLengths[timerNumber].Start();
        }

        public static void StackSpamUser(IUser infringer, double increasedTimeTotal, SocketMessage commandUsage)
        {
            //get id
            int id = userPunishmentIdPairs[infringer];
            //modify time up timer
            try
            {
                userPunishmentLengths[id].Interval = increasedTimeTotal;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                commandUsage.Channel.SendMessageAsync("I encountered an error: " + e.Message);
                return;
            }
        }

        public static void StopSpam(int id)
        {
            spamTimers[id].Stop();
        }

        public static Embed ViewSpam()
        {
            EmbedBuilder instancesEmbedBuilder = new EmbedBuilder
            {

            };

            return instancesEmbedBuilder.Build();
        }

        private static void SpamChannelTimer_Elapsed(object sender, ElapsedEventArgs e, IMessageChannel spamChannel)
        {
            spamChannel.SendMessageAsync("dont spam!!");
            
        }
        private static void SpamUserTimer_Elapsed(object sender, ElapsedEventArgs e, IUser infringer)
        {
            infringer.SendMessageAsync("dont spam!!");
        }
        public static void SpamUserStop(int id, IUser infringer, string reason)
        {
            //note the infringer
            infringer.SendMessageAsync($"The anti-spam treatment is now over, {reason}");

            //stop stopwatch
            punishmentIdElapsedTimePairs[id].Stop();

            //stop spamming
            StopSpam(id);

            //get end timer
            userPunishmentLengths.TryGetValue(id, out Timer timer);

            if (reason.Contains("stopped the treatment!"))
            {
                //if mod stopped stop the timers manually
                timer.Stop();
            }
        }
    }
}

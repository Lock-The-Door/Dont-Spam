using Discord;
using System.Timers;
using System.Threading.Tasks;
using System.Collections.Generic;
using Discord.WebSocket;

namespace Discord_Bot_Template
{
    public static class DontSpamSpammer
    {
        private static IMessageChannel _spamChannel;
        static Timer spamTimer = new Timer(10000);
        public static List<Timer> spamTimers = new List<Timer>();
        public static Dictionary<int, Timer> userPunishmentLengths = new Dictionary<int, Timer>();
        public static Dictionary<int, IMessageChannel> treatmentIdChannelPairs = new Dictionary<int, IMessageChannel>();
        public static Dictionary<int, IUser> punishmentIdUserPairs = new Dictionary<int, IUser>();


        public static void Spam(IMessageChannel spamChannel, SocketGuild guild)
        {
            _spamChannel = spamChannel;
            spamTimer.Elapsed += delegate (object sender, ElapsedEventArgs e) { SpamChannelTimer_Elapsed(sender, e, spamChannel); };
            spamTimer.Start();
        }

        public static void SpamChannel(IMessageChannel spamChannel)
        {
            spamTimers.Add(new Timer(10000));

            int timerNumber = spamTimers.Count - 1;

            treatmentIdChannelPairs.Add(timerNumber, spamChannel);

            spamTimers[timerNumber].Elapsed += delegate (object sender, ElapsedEventArgs e) { SpamChannelTimer_Elapsed(sender, e, spamChannel); };

            spamTimers[timerNumber].Start();
        }

        public static void SpamUser(IUser infringer, ulong length)
        {
            //set up spam timer
            spamTimers.Add(new Timer(10000));
            int timerNumber = spamTimers.Count - 1;
            //set up time up timer
            userPunishmentLengths.Add(timerNumber, new Timer(length * 1000));
            int userTimerNumber = userPunishmentLengths.Count - 1;
            userPunishmentLengths[timerNumber].AutoReset = false;
            //link the id to the user
            punishmentIdUserPairs.Add(timerNumber, infringer);
            //finish and start spam timer
            spamTimers[timerNumber].Elapsed += delegate (object sender, ElapsedEventArgs e) { SpamUserTimer_Elapsed(sender, e, infringer); };
            spamTimers[timerNumber].Start();
            //finish and start time up timer
            userPunishmentLengths[timerNumber].Elapsed += delegate (object sender, ElapsedEventArgs e) { SpamUserStop(timerNumber, infringer, $"your {userPunishmentLengths[timerNumber].Interval / 1000} seconds is up!"); };
            userPunishmentLengths[timerNumber].Start();
        }

        public static void StopSpam(int id)
        {
            //stop timer
            spamTimers[id].Stop();
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

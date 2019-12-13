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

            spamTimers[timerNumber].Elapsed += delegate (object sender, ElapsedEventArgs e) { SpamChannelTimer_Elapsed(sender, e, spamChannel); };

            spamTimers[timerNumber].Start();
        }

        public static void SpamUser(IUser infringer, ulong length)
        {
            spamTimers.Add(new Timer(10000));

            int timerNumber = spamTimers.Count;
            userPunishmentLengths.Add(timerNumber, new Timer(length * 1000));

            spamTimers[timerNumber].Elapsed += delegate (object sender, ElapsedEventArgs e) { SpamUserTimer_Elapsed(sender, e, infringer); };

            spamTimers[timerNumber].Start();
        }

        public static void StopSpam(int id)
        {
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
    }
}

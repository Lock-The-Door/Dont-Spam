using Discord;
using System.Timers;
using System.Threading.Tasks;

namespace Discord_Bot_Template
{
    public static class DontSpamSpammer
    {
        private static IMessageChannel _spamChannel;
        static Timer spamTimer = new Timer(10000);


        public static void Spam(IMessageChannel spamChannel)
        {
            _spamChannel = spamChannel;
            _spamChannel.SendMessageAsync("dont spam!!");
            spamTimer.Elapsed += SpamTimer_Elapsed;
            spamTimer.Start();
        }

        public static void StopSpam()
        {
            spamTimer.Stop();
        }

        private static void SpamTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _spamChannel.SendMessageAsync("dont spam!!");
        }
    }
}

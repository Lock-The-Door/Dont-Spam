using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace Discord_Bot_Template
{
    [Group("tests")]
    [RequireOwner(ErrorMessage = "This is a test command and is not intended for public use", Group = "Owner command")]
    [Summary("Group for test commands")]
    public class TestModule : ModuleBase<SocketCommandContext>
    {
        [Command("test")]
        [Summary("A test command")]
        public async Task Test()
        {
            await Context.Channel.SendMessageAsync("test");
        }

        [Command("inputTest")]
        [Summary("tests input by sending a echo")]
        public async Task InputTest(string input)
        {
            await Context.Channel.SendMessageAsync($"You said: \"{input}\"");
        }
    }

    [Group("spam")]
    public class AntiSpamModule : ModuleBase<SocketCommandContext>
    {
        [Command("start")]
        [Summary("Starts spamming \"dont spam!!\"")]
        public async Task startSpam(IMessageChannel spamChannel)
        {
            await ReplyAsync("Activating spam treatment in #" + spamChannel.Name);
            await spamChannel.SendMessageAsync("dont spam!!");
            DontSpamSpammer.Spam(spamChannel);
        }
        [Command("stop")]
        [Summary("Stops spamming \"dont spam!!\"")]
        public async Task stopSpam()
        {
            await ReplyAsync("Stoping spam treatment");
            DontSpamSpammer.StopSpam();
        }
    }
}

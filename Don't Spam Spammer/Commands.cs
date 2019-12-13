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
    [RequireUserPermission(GuildPermission.Administrator, ErrorMessage = "You aren't allowed to use this command!")]
    public class AntiSpamModule : ModuleBase<SocketCommandContext>
    {
        [Command("start")]
        [Summary("Starts spamming \"dont spam!!\"")]
        public async Task startSpam(IMessageChannel spamChannel = null)
        {
            if (spamChannel is null)
            {
                await ReplyAsync("You have to enter a channel to start the treatment!");
                return;
            }
            await ReplyAsync("Activating spam treatment in #" + spamChannel.Name);
            DontSpamSpammer.SpamChannel(spamChannel);
            await spamChannel.SendMessageAsync($"Treatment started, to stop, do $stop {DontSpamSpammer.spamTimers.Count}");
            //await spamChannel.SendMessageAsync("dont spam!!");
        }
        [Command("stop")]
        [Summary("Stops spamming \"dont spam!!\"")]
        public async Task stopSpam(int id)
        {
            await ReplyAsync("Stoping spam treatment");
            DontSpamSpammer.StopSpam(id);
        }
    }

    public class PunishModule : ModuleBase<SocketCommandContext>
    {
        [Command("punish")]
        [Summary("Starts sending DM messages to the use specified as a punishment")]
        public async Task startDmSpam(IUser infringer, ulong punishmentTime)
        {
            await ReplyAsync($"Giving a private session of anti-spam therapy to {infringer.Mention}");
        }
    }
}

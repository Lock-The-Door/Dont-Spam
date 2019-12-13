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
    [RequireUserPermission(GuildPermission.Administrator, ErrorMessage = "You aren't allowed to use this command!", Group = "Admin Command", NotAGuildErrorMessage = "This is a guild command and cannot be used here!")]
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
            await ReplyAsync("Activating spam treatment in " + spamChannel.Name);
            DontSpamSpammer.SpamChannel(spamChannel);
            string message = $"Treatment started, to stop, do $spam stop {DontSpamSpammer.spamTimers.Count - 1}";
            if (spamChannel == Context.Channel) await spamChannel.SendMessageAsync(message); //only send the extra message if the spam is in a different channel
            await ReplyAsync(message);
        }

        [Command("stop")]
        [Summary("Stops spamming \"dont spam!!\"")]
        public async Task stopSpam(int id)
        {
            //check and give response
            if (!DontSpamSpammer.treatmentIdChannelPairs.TryGetValue(id, out IMessageChannel spamChannel))
            { await ReplyAsync("That is not a valid channel treatment id!"); return; }
            
            await ReplyAsync($"Stopping spam treatment in {spamChannel.Name} ({id})");
            DontSpamSpammer.StopSpam(id);
        }
    }

    [RequireUserPermission(GuildPermission.Administrator, ErrorMessage = "You aren't allowed to use this command!", Group = "Admin Command", NotAGuildErrorMessage = "This is a guild command and connot be used here!")]
    public class PunishModule : ModuleBase<SocketCommandContext>
    {
        [Command("punish")]
        [Summary("Starts sending DM messages to the use specified as a punishment")]
        public async Task startDmSpam(IUser infringer, ulong punishmentTime)
        {
            //give response
            await ReplyAsync($"Giving a private session of anti-spam therapy to {infringer.Mention} for {punishmentTime} seconds");

            //alert infringer
            await infringer.SendMessageAsync($"Moderator {Context.User.Username} gave you a ***FREE!!!*** private session of anti-spam therapy to you for {punishmentTime} seconds!");

            //start the treatment
            DontSpamSpammer.SpamUser(infringer, punishmentTime);

            //give stop id
            await ReplyAsync($"Private session started, do $override {DontSpamSpammer.spamTimers.Count - 1} to override the timer.");
        }

        [Command("override")]
        [Summary("Manually stop sending DM spam messages")]
        public async Task stopDmSpam(int id = -1)
        {
            //get user first as required
            if (!DontSpamSpammer.punishmentIdUserPairs.TryGetValue(id, out IUser infringer))
            { await ReplyAsync("That is not a valid user punishment id!"); return; }
            //respond
            await ReplyAsync($"Stopping {infringer.Username}'s private anti-spam therapy seesion");
            //stop the treatment
            DontSpamSpammer.SpamUserStop(id, infringer, $"moderator, {Context.User.Username} stopped the treatment!");
        }
    }
}

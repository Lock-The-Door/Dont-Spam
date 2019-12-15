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
            //ensure that the channel is not already being spammed
            //get treatment id from channel
            int treatmentId = -1;
            if (DontSpamSpammer.channelTreatmentIdPairs.TryGetValue(spamChannel, out treatmentId) && DontSpamSpammer.spamTimers[treatmentId].Enabled)
            {
                await ReplyAsync($"{(spamChannel as ITextChannel).Mention} is already being spammed!");
                return;
            }

            //start treatment
            await ReplyAsync($"Activating spam treatment in {(spamChannel as ITextChannel).Mention}");
            DontSpamSpammer.SpamChannel(spamChannel);
            //give stop command
            string message = $"Treatment started, to stop, do $spam stop {(spamChannel as ITextChannel).Mention}";
            if (spamChannel != Context.Channel) await spamChannel.SendMessageAsync(message); //only send the extra message if the spam is in a different channel
            await ReplyAsync(message);
        }

        [Command("stop")]
        [Summary("Stops spamming \"dont spam!!\"")]
        public async Task stopSpam(IMessageChannel spamChannel = null)
        {
            //see if channel entered is valid
            if (spamChannel == null)
            { await ReplyAsync($"{(spamChannel as ITextChannel).Mention} is not a valid channel"); return; }

            //check if being treated and give response
            if (!DontSpamSpammer.channelTreatmentIdPairs.TryGetValue(spamChannel, out int id) || !DontSpamSpammer.spamTimers[id].Enabled)
            { await ReplyAsync($"{(spamChannel as ITextChannel).Mention} is not being treated right now!"); return; }

            await ReplyAsync($"Stopping spam treatment in {(spamChannel as ITextChannel).Mention} ({id})");
            DontSpamSpammer.StopSpam(id);
        }

        [Command("running")]
        [Summary("Displays running spam instances")]
        public async Task viewSpam()
        {

        }
    }

    [RequireUserPermission(GuildPermission.Administrator, ErrorMessage = "You aren't allowed to use this command!", Group = "Admin Command", NotAGuildErrorMessage = "This is a guild command and connot be used here!")]
    public class PunishModule : ModuleBase<SocketCommandContext>
    {
        [Command("punish")]
        [Summary("Starts sending DM messages to the use specified as a punishment")]
        public async Task startDmSpam(IUser infringer, ulong punishmentTime)
        {
            //check if stack is required
            if (DontSpamSpammer.userPunishmentIdPairs.TryGetValue(infringer, out int id) && DontSpamSpammer.spamTimers[id].Enabled)
            {
                //if it is stack the punishment instead of creating a new one
                //give response
                await ReplyAsync($"{infringer.Mention} already has a punishment! The punishment will be stacked");
                await stackDmSpam(infringer, punishmentTime);
                return;
            }

            //give response
            await ReplyAsync($"Giving a private session of anti-spam therapy to {infringer.Mention} for {punishmentTime} seconds");
            //alert infringer
            await infringer.SendMessageAsync($"Moderator {Context.User.Username} gave you a ***FREE!!!*** private session of anti-spam therapy to you for {punishmentTime} seconds!");

            //start the treatment
            DontSpamSpammer.SpamUser(infringer, punishmentTime);

            //give stop command
            await ReplyAsync($"Private session started, do $override {infringer.Mention} to override the timer.");
        }

        [Command("stack")]
        [Summary("Stacks an existing punishment and increases the time")]
        public async Task stackDmSpam(IUser infringer, ulong punishmentTime)
        {
            //get user and check if valid
            if (!DontSpamSpammer.userPunishmentIdPairs.TryGetValue(infringer, out int id))
            { await ReplyAsync("That is not a valid user punishment!"); return; }
            else if (!DontSpamSpammer.spamTimers[id].Enabled)
            { await ReplyAsync("That is not a valid user punishment!"); return; }
            //give response
            await ReplyAsync($"Increasing {infringer.Mention}'s punishment time by {punishmentTime} seconds");
            //alert infringer
            await infringer.SendMessageAsync($"Moderator {Context.User.Username} increased your private session of anti-spam therapy by {punishmentTime} seconds for ***FREE!!!***");

            //increase treatment
            DontSpamSpammer.StackSpamUser(infringer, punishmentTime);

            //give stop command
            await ReplyAsync($"Private session time increased, do $override {infringer.Mention} to override the timer.");
        }

        [Command("override")]
        [Summary("Manually stop sending DM spam messages")]
        public async Task stopDmSpam(IUser infringer)
        {
            //get user first as required and check if valid
            if (!DontSpamSpammer.userPunishmentIdPairs.TryGetValue(infringer, out int id))
            { await ReplyAsync("That is not a valid user punishment!"); return; }
            else if (!DontSpamSpammer.spamTimers[id].Enabled)
            { await ReplyAsync("That is not a valid user punishment!"); return; }
            //respond
            await ReplyAsync($"Stopping {infringer.Username}'s private anti-spam therapy session");
            //stop the treatment
            DontSpamSpammer.SpamUserStop(id, infringer, $"moderator {Context.User.Username} stopped the treatment!");
        }
    }


}

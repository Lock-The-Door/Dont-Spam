using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Bot_Template
{
    public class Command_Handler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;

        private Dictionary<IUser, Stopwatch> userStopwatchPairs = new Dictionary<IUser, Stopwatch>();
        private Dictionary<IUser, ulong> userInfractionsPairs = new Dictionary<IUser, ulong>();

        // Retrieve client and CommandService instance via ctor
        public Command_Handler(DiscordSocketClient client, CommandService commands)
        {
            _commands = commands;
            _client = client;
        }

        public async Task InstallCommandsAsync()
        {
            // Hook the MessageReceived event into our command handler
            _client.MessageReceived += HandleCommandAsync;

            // Here we discover all of the command modules in the entry 
            // assembly and load them. Starting from Discord.NET 2.0, a
            // service provider is required to be passed into the
            // module registration method to inject the 
            // required dependencies.
            //
            // If you do not use Dependency Injection, pass null.
            // See Dependency Injection guide for more information.
            await _commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(),
                                            services: null);
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            // Don't process the command if it was a system message
            var message = messageParam as SocketUserMessage;
            if (message == null) return;

            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;

            // Determine if the message is a command based on the prefix and make sure no bots trigger commands
            if (!(message.HasCharPrefix('$', ref argPos) ||
                message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
                message.Author.IsBot)
                return;

            // Create a WebSocket-based command context based on the message
            var context = new SocketCommandContext(_client, message);

            //detect if user is spammming
            Stopwatch userLastSent;
            if (!userStopwatchPairs.TryGetValue(message.Author, out userLastSent))
            {
                //user does not have stopwatch
                userStopwatchPairs.Add(message.Author, new Stopwatch());

                userStopwatchPairs[message.Author].Start();
                //don't count first message
            } //else user has stop watch
            {

                //see if spamming
                if (userLastSent.ElapsedMilliseconds < 1000)
                {
                    //add infraction since spamming
                    ulong infractions;
                    if (!userInfractionsPairs.TryGetValue(message.Author, out infractions))
                    { //if not made create now
                        userInfractionsPairs.Add(message.Author, 1);
                    }
                    else
                    {
                        userInfractionsPairs[message.Author]++;
                    }

                    //if got more than 4 infractions, spam them for 30 seconds
                    if (userInfractionsPairs[message.Author] > 4)
                    {
                        DontSpamSpammer.SpamUser(message.Author, 30, message);
                    }
                }
                //if not spamming remove one infraction
                {
                    //add infraction since spamming
                    ulong infractions;
                    if (!userInfractionsPairs.TryGetValue(message.Author, out infractions))
                    { //if not made create now
                        userInfractionsPairs.Add(message.Author, 0);
                    }
                    else if (infractions != 0) //don't allow negatives
                    {
                        userInfractionsPairs[message.Author]--;
                    }
                }
            }

            // Execute the command with the command context we just
            // created, along with the service provider for precondition checks.
            await _commands.ExecuteAsync(
                context: context,
                argPos: argPos,
                services: null);
        }
    }
}

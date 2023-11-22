using DSharpPlus.ButtonCommands;
using DSharpPlus.CommandsNext;
using DSharpPlus;
using DSharpPlus.ModalCommands;
using DSharpPlus.Interactivity;
using Leaf_Village_Bot.Config;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.ButtonCommands.Extensions;
using DSharpPlus.ModalCommands.Extensions;
using DSharpPlus.SlashCommands;
using Leaf_Village_Bot.Commands;
using Leaf_Village_Bot.Commands.LMPF;
using Leaf_Village_Bot.Commands.Profile;
using Microsoft.Extensions.Logging;
using Leaf_Village_Bot.DBUtil.ReportTicket;
using DSharpPlus.Entities;
using Leaf_Village_Bot.Commands.Ranked;
using Leaf_Village_Bot.DBUtil.RPRequest;
using Leaf_Village_Bot.Commands.Hokage;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.SlashCommands.Attributes;
using Leaf_Village_Bot.DBUtil.Profile;
using Leaf_Village_Bot.Commands.Raid;

namespace Leaf_Village_Bot
{
    public sealed class Program
    {
        public static DiscordClient Client { get; private set; }
        public static CommandsNextExtension Commands { get; private set; }
        public static ButtonCommandsExtension ButtonCommands { get; private set; }
        public static ModalCommandsExtension ModalCommands { get; private set; }

        static async Task Main(string[] args)
        {
            // 1. Retrieving details of the BotConfig.json & DBConfig.json files through JSONReader
            var configJSON = new JSONReader();
            await configJSON.ReadBotConfigJSONAsync();

            // 2. Creating Discord Bot Configuration
            var discordConfig = new DiscordConfiguration()
            {
                Intents = DiscordIntents.All,
                Token = configJSON.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Debug
            };

            // 3. Apply Config to the DiscordClient
            Client = new DiscordClient(discordConfig);

            // 4. Setting default timeout for Commands that use Interactivity
            Client.UseInteractivity(new InteractivityConfiguration()
            {
                Timeout = TimeSpan.FromMinutes(5)
            });

            // 5. Creating Task Handler
            Client.Ready += OnClientReady;
            Client.ComponentInteractionCreated += OnClientComponentInteractionCreated;
            Client.ModalSubmitted += OnClientModalSubmitted;

            // 6. Setup Commands Configuration
            var prefixCommandsConfig = new CommandsNextConfiguration()
            {
                StringPrefixes = new string[] { configJSON.Prefix},
                EnableMentionPrefix = true,
                EnableDms = true,
                EnableDefaultHelp = true,
            };

            var buttonCommandsConfig = new ButtonCommandsConfiguration()
            {
                ArgumentSeparator = "-",
                Prefix = "/"
            };

            var modalCommandsConfig = new ModalCommandsConfiguration()
            {
                Seperator = "-",
                Prefix = "/"
            };

            Commands = Client.UseCommandsNext(prefixCommandsConfig);
            ButtonCommands = Client.UseButtonCommands(buttonCommandsConfig);
            ModalCommands = Client.UseModalCommands(modalCommandsConfig);
            var slashCommands = Client.UseSlashCommands();

            // 7. Register Commands
            Commands.RegisterCommands<PrefixCommands>();
            Commands.RegisterCommands<PrefixCommands_Hokage>();
            Commands.RegisterCommands<PrefixCommands_LMPF>();
            Commands.RegisterCommands<PrefixCommands_Profile>();
            Commands.RegisterCommands<PrefixCommands_Ranked>();
            Commands.RegisterCommands<Prefix_Commands_Raid>();


            slashCommands.RegisterCommands<SlashCommands_Profile>();
            slashCommands.RegisterCommands<SlashCommands_Hokage>();

            ButtonCommands.RegisterButtons<ButtonCommands_Profile>();
            ButtonCommands.RegisterButtons<ButtonCommands_LMPF>();
            ButtonCommands.RegisterButtons<ButtonCommands_Ranked>();
            ButtonCommands.RegisterButtons<ButtonCommands_Hokage>();

            ModalCommands.RegisterModals<ModalCommands_Profile>();
            ModalCommands.RegisterModals<ModalCommands_Ranked>();
            ModalCommands.RegisterModals<ModalCommands_LMPF>();

            Commands.CommandErrored += OnCommandErrored;
            slashCommands.SlashCommandErrored += OnSlashCommandErrored;

            // 8. Button Commands Event Handler
            ButtonCommands.ButtonCommandExecuted += ButtonCommands_ButtonCommandExecuted;
            ButtonCommands.ButtonCommandErrored += ButtonCommands_ButtonCommandErrored;

            // 9. Modal Commands Event Handler
            ModalCommands.ModalCommandExecuted += ModalCommands_ModalCommandExecuted;
            ModalCommands.ModalCommandErrored += ModalCommands_ModalCommandErrored;

            // 10. Connecting... Bot Online
            await Client.ConnectAsync();
            await Task.Delay(-1);
        }

        private static async Task OnSlashCommandErrored(SlashCommandsExtension sender, DSharpPlus.SlashCommands.EventArgs.SlashCommandErrorEventArgs args)
        {
            if (args.Exception is SlashExecutionChecksFailedException exception)
            {
                string timeLeft = string.Empty;
                foreach (var check in exception.FailedChecks)
                {
                    var cooldown = (SlashCooldownAttribute)check;
                    timeLeft = cooldown.GetRemainingCooldown(args.Context).ToString(@"hh\:mm\:ss");
                }

                var coolDownMessage = new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Red,
                    Title = "Please wait for the cooldown to end.",
                    Description = $"Time: {timeLeft}"
                };
                
                await args.Context.CreateResponseAsync(coolDownMessage);
            }

            
        }

        private static Task OnCommandErrored(CommandsNextExtension sender, CommandErrorEventArgs args)
        {
            args.Context.Client.Logger.LogError(args.Exception,
                    $"{args.Context.Member} has used button command {args.Command} ({args.Command.Name}) which threw an exception: {args.Exception.Message}");
            return Task.CompletedTask;
        }

        private static Task ModalCommands_ModalCommandErrored(ModalCommandsExtension sender, DSharpPlus.ModalCommands.EventArgs.ModalCommandErrorEventArgs args)
        {
            args.Context.Client.Logger.LogError(args.Exception,
                    $"{args.Context.Member} has used button command {args.CommandName} ({args.ModalId}) which threw an exception: {args.Exception.Message}");
            return Task.CompletedTask;
        }

        private static Task ModalCommands_ModalCommandExecuted(ModalCommandsExtension sender, DSharpPlus.ModalCommands.EventArgs.ModalCommandExecutionEventArgs args)
        {
            args.Context.Client.Logger.LogInformation(
                    $"{args.Context.User} has used button command {args.CommandName} ({args.ModalId})");
            return Task.CompletedTask;
        }

        private static Task ButtonCommands_ButtonCommandErrored(ButtonCommandsExtension sender, DSharpPlus.ButtonCommands.EventArgs.ButtonCommandErrorEventArgs args)
        {
            args.Context.Client.Logger.LogError(args.Exception,
                    $"{args.Context.Member} has used button command {args.CommandName} ({args.ButtonId}) which threw an exception: {args.Exception.Message}");
            return Task.CompletedTask;
        }

        private static Task ButtonCommands_ButtonCommandExecuted(ButtonCommandsExtension sender, DSharpPlus.ButtonCommands.EventArgs.ButtonCommandExecutionEventArgs args)
        {
            args.Context.Client.Logger.LogInformation(
                    $"{args.Context.User} has used button command {args.CommandName} ({args.ButtonId})");
            return Task.CompletedTask;
        }

        private static Task OnClientModalSubmitted(DiscordClient sender, DSharpPlus.EventArgs.ModalSubmitEventArgs e)
        {
            return Task.CompletedTask;
        }

        private static async Task OnClientComponentInteractionCreated(DiscordClient sender, DSharpPlus.EventArgs.ComponentInteractionCreateEventArgs e)
        {
            var DBUtil_ReportTicket = new DBUtil_ReportTicket();
            var DBUtil_RPRequest = new DBUtil_RPRequest();
            var DBUtil_Profile = new DBUtil_Profile();

            if (e.Interaction.Data.ComponentType == ComponentType.StringSelect)
            {
                switch (e.Interaction.Data.CustomId)
                {
                    case "dpdwn_GetTicket":
                        string ticketSelected = e.Interaction.Data.Values[0];
                        var ticketID_Selected = ulong.Parse(ticketSelected);
                        var serverid_ticket = e.Interaction.Guild.Id;

                        var ticketResult = await DBUtil_ReportTicket.GetReportTicketAsync(ticketID_Selected, serverid_ticket);

                        if(ticketResult.Item1 == false)
                        {
                            var failed = new DiscordEmbedBuilder()
                            {
                                Color = DiscordColor.Red,
                                Title = "Failed to retreive the ticket!"
                            };
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().AddEmbed(failed));
                            break;
                        }

                        var ticket = ticketResult.Item2;
                        DiscordMember ticketMember = await e.Guild.GetMemberAsync(ticket.MemberID);

                        var displayTicket = new DiscordMessageBuilder()
                            .AddEmbed(new DiscordEmbedBuilder()
                                .WithColor(DiscordColor.SpringGreen)
                                .WithTitle("Leaf Military Police Report System")
                                .WithImageUrl(ticketMember.AvatarUrl)
                                .WithThumbnail(Global.LeafSymbol_URL)
                                .AddField("Plantiff:", ticket.Plantiff)
                                .AddField("Defendant:", ticket.Defendant)
                                .AddField("Date:", ticket.Date)
                                .AddField("Details:", ticket.Details)
                                );
                        await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(displayTicket));
                        break;

                    case "dpdwn_DeleteTicket":
                        string ticketDeleted = e.Interaction.Data.Values[0];
                        var ticketID_Deleted = ulong.Parse(ticketDeleted);

                        var isTicketDeleted = await DBUtil_ReportTicket.DeleteReportTicketAsync(ticketID_Deleted);

                        if(isTicketDeleted)
                        {
                            var success = new DiscordEmbedBuilder()
                            {
                                Color = DiscordColor.SpringGreen,
                                Title = "Successfully delete the ticket!"
                            };

                            await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().AddEmbed(success));
                        } else
                        {
                            var failed = new DiscordEmbedBuilder()
                            {
                                Color = DiscordColor.Red,
                                Title = "Failed to delete the ticket!"
                            };
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().AddEmbed(failed));
                        }
                        break;

                    case "dpdwnGetRequest":
                        string requestSelected = e.Interaction.Data.Values[0];
                        var requestID = ulong.Parse(requestSelected);
                        var serverid_request = e.Interaction.Guild.Id;

                        var requestResult = await DBUtil_RPRequest.GetRequestAsync(requestID, serverid_request);
                        var request = requestResult.Item2;

                        DiscordMember requestMember = await e.Guild.GetMemberAsync(request.MemberID);

                        var displayRequest = new DiscordMessageBuilder()
                        .AddEmbed(new DiscordEmbedBuilder()
                            .WithColor(DiscordColor.SpringGreen)
                                        .WithTitle($"RP Mission Request for {request.IngameName}")
                                        .WithImageUrl(requestMember.AvatarUrl)
                                        .WithThumbnail(Global.LeafSymbol_URL)
                                        .AddField("IGN:", request.IngameName, true)
                                        .AddField("RP Mission:", request.RPMission, true)
                                        .AddField("Timezone:", request.Timezone, true)
                                        .AddField("Attendees:", request.Attendees)
                                        .WithFooter($"Request ID: {request.RequestID}"));

                        await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(displayRequest));
                        break;

                    case "dpdwnDeleteRequest":
                        string requestDeleted = e.Interaction.Data.Values[0];
                        var requestIDDeleted = ulong.Parse(requestDeleted);
                        var serverid_deleted = e.Interaction.Guild.Id;

                        var isRequestDeleted = await DBUtil_RPRequest.DeleteRequestAsync(requestIDDeleted, serverid_deleted);

                        if (isRequestDeleted)
                        {
                            var success = new DiscordEmbedBuilder()
                            {
                                Color = DiscordColor.SpringGreen,
                                Title = "Successfully deleted the request!"
                            };

                            await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().AddEmbed(success));
                        }
                        else
                        {
                            var failed = new DiscordEmbedBuilder()
                            {
                                Color = DiscordColor.Red,
                                Title = "Failed to delete the request!"
                            };
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().AddEmbed(failed));
                        }

                        break;

                    case "dpdwn_ClanEmoji":
                        await e.Interaction.DeferAsync(true);
                        string clanSelected = e.Interaction.Data.Values[0];
                        var isClanUpdated = await DBUtil_Profile.UpdateClanAsync(e.User.Id, clanSelected);

                        if(isClanUpdated)
                        {   
                            await e.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Successfully updated your clan!"));
                        } else
                        {
                            await e.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Failed to update your clan!"));
                        }

                        break;

                    case "dpdwn_MasteryEmoji":
                        await e.Interaction.DeferAsync(true);

                        var masterySelected = e.Interaction.Data.Values.ToArray();

                        var masteries = string.Join(",", masterySelected);
                        var queryMasteries = String.Join(",", masteries.Split(",").Select(x => string.Format("'{0}'", x)));

                        var isMasteryUpdated = await DBUtil_Profile.UpdateMasteriesAsync(e.User.Id, queryMasteries);

                        if(isMasteryUpdated)
                        {
                            await e.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Successfully updated your masteries!"));
                        } else
                        {
                            await e.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Failed to update your masteries!"));
                        }


                        break;
                }
            }
        }

        private static Task OnClientReady(DiscordClient sender, DSharpPlus.EventArgs.ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }
    }
}
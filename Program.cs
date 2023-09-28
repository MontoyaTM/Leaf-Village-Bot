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
                Timeout = TimeSpan.FromMinutes(2)
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
            ButtonCommands.RegisterButtons<ButtonCommands_Profile>();
            ButtonCommands.RegisterButtons<ButtonCommands_LMPF>();
            ModalCommands.RegisterModals<ModalCommands_Profile>();
            ModalCommands.RegisterModals<ModalCommands_LMPF>();

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

        private static Task ModalCommands_ModalCommandErrored(ModalCommandsExtension sender, DSharpPlus.ModalCommands.EventArgs.ModalCommandErrorEventArgs args)
        {
            args.Context.Client.Logger.LogError(args.Exception,
                    $"{args.Context.Member} has used button command {args.CommandName} ({args.ModalId}) which threw an exception");
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
                    $"{args.Context.Member} has used button command {args.CommandName} ({args.ButtonId}) which threw an exception");
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

            if (e.Interaction.Data.ComponentType == ComponentType.StringSelect)
            {
                switch (e.Interaction.Data.CustomId)
                {
                    case "dpdwnLMPFGetTicket":
                        string selectedOption = e.Interaction.Data.Values[0];
                        var ticketNoSelected = int.Parse(selectedOption);

                        var result = await DBUtil_ReportTicket.GetReportTicketAsync(ticketNoSelected);
                        var ticket = result.Item2;

                        var displayTicket = new DiscordMessageBuilder()
                            .AddEmbed(new DiscordEmbedBuilder()
                                .WithColor(DiscordColor.SpringGreen)
                                .WithTitle("Leaf Military Police Report System")
                                .WithImageUrl("https://static.wikia.nocookie.net/naruto/images/f/f2/Military_Police_Symbol.svg/revision/latest/scale-to-width-down/150?cb=20160212025523")
                                .WithThumbnail("https://www.ninonline.com/forum/uploads/monthly_2018_07/LeafSymbolX.png.8c5645dae2146b2745cde4c4a48f7583.png")
                                .AddField("Reporter:", ticket.Plantiff)
                                .AddField("Accused:", ticket.Defendant)
                                .AddField("Date:", ticket.Date)
                                .AddField("Details:", ticket.Details)
                                .AddField("Screenshot URL:", ticket.Screenshots)
                                );
                        await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(displayTicket));
                        break;

                    case "dpdwnLMPFDeleteTicket":
                        string deletedOption = e.Interaction.Data.Values[0];
                        var ticketNoDeleted = int.Parse(deletedOption);

                        await DBUtil_ReportTicket.DeleteReportTicketAsync(ticketNoDeleted);

                        var success = new DiscordEmbedBuilder()
                        {
                            Color = DiscordColor.SpringGreen,
                            Title = "Successfully deleted the ticket!"
                        };

                        await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().AddEmbed(success));
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
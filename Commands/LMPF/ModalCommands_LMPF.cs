using DSharpPlus;
using DSharpPlus.ButtonCommands;
using DSharpPlus.ButtonCommands.Extensions;
using DSharpPlus.ModalCommands;
using DSharpPlus.ModalCommands.Attributes;
using Leaf_Village_Bot.DBUtil.ReportTicket;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leaf_Village_Bot.Commands.LMPF
{
    internal class ModalCommands_LMPF : ModalCommandModule
    {
        private string LeafSymbolURL = "https://i.imgur.com/spjhOGb.png";
        private string LMPFSymbolURL = "https://i.imgur.com/QbBzNiR.png";

        [ModalCommand("LMPFReportModal")]
        public async Task SubmitLMPFReport(ModalContext ctx)
        {
            await ctx.Interaction.DeferAsync();

            var modalValues = ctx.Values;
            var DBUtil_TicketSystem = new DBUtil_ReportTicket();

            ButtonCommandsExtension buttonCommand = ctx.Client.GetButtonCommands();

            var ticketInfo = new DBReportTicket
            {
                UserName = ctx.Interaction.User.Username,
                Plantiff = modalValues.ElementAt(0),
                Defendant = modalValues.ElementAt(1),
                Date = modalValues.ElementAt(2),
                Details = modalValues.ElementAt(3),
                Screenshots = modalValues.ElementAt(4),
            };

            var isStored = await DBUtil_TicketSystem.StoreReportAsync(ticketInfo);

            if (isStored == true)
            {
                var createDiscussionChannel = await ctx.Interaction.Guild.CreateChannelAsync($"Report Discussion: {ticketInfo.UserName}", ChannelType.Text, ctx.Interaction.Channel.Parent);

                var embedTicket = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithColor(DiscordColor.SpringGreen)
                                    .WithTitle("Leaf Military Police Report System")
                                    .WithImageUrl(LMPFSymbolURL)
                                    .WithThumbnail(LeafSymbolURL)
                                    .AddField("Plantiff:", ticketInfo.Plantiff)
                                    .AddField("Defendant:", ticketInfo.Defendant)
                                    .AddField("Date:", ticketInfo.Date)
                                    .AddField("Details:", ticketInfo.Details)
                                    .AddField("Screenshot URL:", ticketInfo.Screenshots))
                        .AddComponents(
                            new DiscordButtonComponent(ButtonStyle.Primary, buttonCommand.BuildButtonId("btnLMPFCloseTicket"), "Close Ticket")
                        );

                await createDiscussionChannel.SendMessageAsync(embedTicket);

                await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Succesfully created ticket for {ctx.Interaction.User.Username}"));
            }
            else
            {
                await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Failed to created ticket for {ctx.Interaction.User.Username}"));
            }
        }
    }
}

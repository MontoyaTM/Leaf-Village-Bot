using DSharpPlus;
using DSharpPlus.ButtonCommands;
using DSharpPlus.ButtonCommands.Extensions;
using DSharpPlus.Entities;
using DSharpPlus.ModalCommands;
using DSharpPlus.ModalCommands.Attributes;
using Leaf_Village_Bot.DBUtil.ReportTicket;

namespace Leaf_Village_Bot.Commands.LMPF
{
    internal class ModalCommands_LMPF : ModalCommandModule
    {
        [ModalCommand("LMPFReportModal")]
        public async Task SubmitLMPFReport(ModalContext ctx)
        {
            await ctx.Interaction.DeferAsync(true);

            var modalValues = ctx.Values;
            var DBUtil_TicketSystem = new DBUtil_ReportTicket();

            ButtonCommandsExtension buttonCommand = ctx.Client.GetButtonCommands();

            var ticketInfo = new DBReportTicket
            {
                TicketID = Global.CreateID(),
                UserName = ctx.Interaction.User.Username,
                MemberID = ctx.Interaction.User.Id,
                ServerID = ctx.Interaction.Guild.Id,
                Plantiff = modalValues.ElementAt(0),
                Defendant = modalValues.ElementAt(1),
                Date = modalValues.ElementAt(2),
                Details = modalValues.ElementAt(3)
            };

            var isStored = await DBUtil_TicketSystem.StoreReportAsync(ticketInfo);

            if (isStored)
            {
                var createDiscussionChannel = await ctx.Interaction.Guild.CreateChannelAsync($"{ticketInfo.Plantiff} v. {ticketInfo.Defendant}", ChannelType.Text, ctx.Interaction.Channel.Parent);
                DiscordMember member = await ctx.Guild.GetMemberAsync(ticketInfo.MemberID);

                var embedTicket = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithColor(DiscordColor.SpringGreen)
                                    .WithTitle("Leaf Military Police Report System")
                                    .WithImageUrl(member.AvatarUrl)
                                    .WithThumbnail(Global.LeafSymbol_URL)
                                    .AddField("Plantiff:", ticketInfo.Plantiff, true)
                                    .AddField("Defendant:", ticketInfo.Defendant, true)
                                    .AddField("Date:", ticketInfo.Date, true)
                                    .AddField("Details:", ticketInfo.Details))
                        .AddComponents(
                            new DiscordButtonComponent(ButtonStyle.Primary, buttonCommand.BuildButtonId("btn_CloseTicket"), "Close Ticket")
                        );

                await createDiscussionChannel.SendMessageAsync(embedTicket);

                await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Succesfully created ticket for {ctx.Interaction.User.Username}"));
            }
            else
            {
                await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Failed to created ticket for {ctx.Interaction.User.Username}, try again."));
            }
        }
    }
}

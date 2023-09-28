using DSharpPlus;
using DSharpPlus.ButtonCommands;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.ModalCommands;
using Leaf_Village_Bot.DBUtil.ReportTicket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Leaf_Village_Bot.Commands.LMPF
{
    public class ButtonCommands_LMPF : ButtonCommandModule
    {
        [ButtonCommand("btnLMPFCreateReport")]
        public async Task CreateReportTicket(ButtonContext ctx)
        {
            var modalTicketReport = ModalBuilder.Create("LMPFReportModal")
                .WithTitle("LMPF Report System")
                .AddComponents(new TextInputComponent("Plantiff:", "reporterTextBox", "Name of Reporter", null, true, TextInputStyle.Short))
                .AddComponents(new TextInputComponent("Defendant:", "accusedTextBox", "Name(s) of Accused", null, true, TextInputStyle.Short))
                .AddComponents(new TextInputComponent("Date:", "dateTextBox", "Date of Incident (mm/dd/yyyy):", null, true, TextInputStyle.Short))
                .AddComponents(new TextInputComponent("Details:", "detailsTextBox", "Please provided a detailed explainination of the situation", null, true, TextInputStyle.Paragraph))
                .AddComponents(new TextInputComponent("ScreenShot URL:", "URLTextBox", "Please provide a link to any screenshots to back up your claim.", null, true, TextInputStyle.Paragraph));

            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modalTicketReport);
        }

        [ButtonCommand("btnLMPFCloseTicket")]
        public async Task CloseReportTicket(ButtonContext ctx)
        {
            await ctx.Interaction.DeferAsync();

            var hasLMPFRole = ctx.Member.Roles.Any(x => x.Name == "LMPF");

            if (hasLMPFRole)
            {
                await ctx.Channel.DeleteAsync();
            } else
            {
                await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Unable to close ticket for {ctx.Interaction.User.Username}, please check required roles."));
            }
        }

        [ButtonCommand("btnLMPFViewTicket")]
        public async Task ViewReportTickets(ButtonContext ctx)
        {
            await ctx.Interaction.DeferAsync(true);

            var DBUtil_ReportTicket = new DBUtil_ReportTicket();

            var isEmpty = await DBUtil_ReportTicket.isEmptyAsync();

            if (isEmpty)
            {
                var embedFailed = new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Red,
                    Title = "There are no tickets to be viewed!"
                };
                await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedFailed));
                return;
            }

            var hasLMPFRole = ctx.Member.Roles.Any(x => x.Name == "LMPF");

            if (hasLMPFRole)
            {
                var retrieveTickets = await DBUtil_ReportTicket.GetAllReportTicketsAsync();

                if (retrieveTickets.Item1 == false)
                {
                    var embedFailed = new DiscordEmbedBuilder()
                    {
                        Color = DiscordColor.Red,
                        Title = "Database retrieval of all tickets has returned false!"
                    };
                    await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedFailed));
                    return;
                }

                var ticketsArray = retrieveTickets.Item2.ToArray();

                string[] outputArray = new string[ticketsArray.Length];
                int i = 0;
                foreach (var item in ticketsArray)
                {
                    outputArray[i] = $"TicketNo: **{item.TicketNo}**, Plantiff: **{item.Plantiff}**, Defendant: **{item.Defendant}**";
                    i++;
                }
               
                var embedTickets = new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.SpringGreen,
                    Title = "Leaf Miltary Police Force Active Reports",
                    Description = string.Join("\n", outputArray)
                };

                await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedTickets));
            } else
            {
                await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Unable to view ticket for {ctx.Interaction.User.Username}, please check required roles."));
            }
        }

        [ButtonCommand("btnLMPFGetTicket")]
        public async Task GetReportTicket(ButtonContext ctx)
        {
            await ctx.Interaction.DeferAsync(true);

            var DBUtil_ReportTicket = new DBUtil_ReportTicket();

            var isEmpty = await DBUtil_ReportTicket.isEmptyAsync();

            if (isEmpty)
            {
                var embedFailed = new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Red,
                    Title = "There are no tickets to be retrieved!"
                };

                await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedFailed));
                return;
            }

            var hasLMPFRole = ctx.Member.Roles.Any(x => x.Name == "LMPF");

            if (hasLMPFRole)
            {
                var retrieveOptions = await GetSelectComponentOptions();

                if (retrieveOptions.Item1 == false)
                {
                    var embedFailed = new DiscordEmbedBuilder()
                    {
                        Color = DiscordColor.Red,
                        Title = "Database retrieval of all tickets has returned false!"
                    };
                    await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedFailed));
                    return;
                }

                var createDropDown = new DiscordSelectComponent("dpdwnLMPFGetTicket", null, retrieveOptions.Item2, false, 0, retrieveOptions.Item2.Count);

                var dropdownTicketEmbed = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithTitle("Select the ID of the ticket you want to retrieve.")
                        .WithColor(DiscordColor.SpringGreen))
                    .AddComponents(createDropDown);

                await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder(dropdownTicketEmbed));
            } else
            {
                await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Unable to retrieve  ticket for {ctx.Interaction.User.Username}, please check required roles."));
            }
        }

        [ButtonCommand("btnLMPFDeleteTicket")]
        public async Task DeleteReportTicket(ButtonContext ctx)
        {
            await ctx.Interaction.DeferAsync(true);
            var DBUtil_ReportTicket = new DBUtil_ReportTicket();
            
            var isEmpty = await DBUtil_ReportTicket.isEmptyAsync();

            if (isEmpty)
            {
                var embedFailed = new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Red,
                    Title = "There are no tickets to be deleted!"
                };
                await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedFailed));
                return;
            }

            var hasLMPFRole = ctx.Member.Roles.Any(x => x.Name == "LMPF");

            if(hasLMPFRole)
            {
                var retrieveOptions = await GetSelectComponentOptions();

                if(retrieveOptions.Item1 == false)
                {
                    var embedFailed = new DiscordEmbedBuilder()
                    {
                        Color = DiscordColor.Red,
                        Title = "Database retrieval of all tickets has returned false!"
                    };
                    await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedFailed));
                    return;
                }

                var createDropDown = new DiscordSelectComponent("dpdwnLMPFDeleteTicket", null, retrieveOptions.Item2, false, 0, retrieveOptions.Item2.Count);

                var dropdownTicketEmbed = new DiscordMessageBuilder()
                        .AddEmbed(new DiscordEmbedBuilder()
                            .WithTitle("Select the ID of the ticket you want to delete.")
                            .WithColor(DiscordColor.SpringGreen))
                        .AddComponents(createDropDown);

                await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder(dropdownTicketEmbed));
            } else
            {
                await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Unable to delete ticket for {ctx.Interaction.User.Username}, please check required roles."));
            }
        }

        public async Task<(bool, List<DiscordSelectComponentOption>)> GetSelectComponentOptions()
        {
            var dropdownOptions = new List<DiscordSelectComponentOption>();
            var DBUtil_ReportTicket = new DBUtil_ReportTicket();
            var retrieveTickets = await DBUtil_ReportTicket.GetAllReportTicketsAsync();

            if (retrieveTickets.Item1 == false)
            {
                return (false, null);
            }

            foreach (var ticket in retrieveTickets.Item2)
            {
                var selectOptions = new DiscordSelectComponentOption(
                    $"TicketNo: {ticket.TicketNo}", $"{ticket.TicketNo}", $"Plantiff: {ticket.Plantiff} v. Defendant: {ticket.Defendant}"
                    );
                dropdownOptions.Add(selectOptions);
            }

            return (true, dropdownOptions);
        }



    }
}

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

            var user = (DiscordMember)ctx.Interaction.User;
            var userRoles = user.Roles.ToArray();

            foreach (var role in userRoles)
            {
                if (role.Name == "LMPF")
                {
                    await ctx.Channel.DeleteAsync();
                }
                else
                {
                    await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Unable to close ticket for {ctx.Interaction.User.Username}, please check required roles."));
                    break;
                }
            }
        }

        [ButtonCommand("btnLMPFViewTicket")]
        public async Task ViewReportTickets(ButtonContext ctx)
        {
            await ctx.Interaction.DeferAsync(true);

            var DBUtil_ReportTicket = new DBUtil_ReportTicket();
            var user = (DiscordMember)ctx.Interaction.User;
            var userRoles = user.Roles.ToArray();

            foreach(var role in userRoles)
            {
                if(role.Name == "LMPF")
                {
                    var isEmpty = await DBUtil_ReportTicket.isEmptyAsync();

                    if(isEmpty)
                    {
                        var embedFailed = new DiscordEmbedBuilder()
                        {
                            Color = DiscordColor.Red,
                            Title = "There are no tickets to be viewed!"
                        };
                        await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedFailed));
                        return;
                    }

                    var tickets = await DBUtil_ReportTicket.GetAllReportTicketsAsync();
                    var ticketsArray = tickets.Item2.ToArray();

                    string[] outputArray = new string[ticketsArray.Length];
                    int i = 0;
                    foreach(var item in ticketsArray)
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
                    return;
                }
            }
            await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Unable to view ticket for {ctx.Interaction.User.Username}, please check required roles."));
        }

        [ButtonCommand("btnLMPFGetTicket")]
        public async Task GetReportTicket(ButtonContext ctx)
        {
            await ctx.Interaction.DeferAsync(true);

            var DBUtil_ReportTicket = new DBUtil_ReportTicket();
            var user = (DiscordMember)ctx.Interaction.User;
            var userRole = user.Roles.ToArray();

            foreach(var role in userRole)
            {
                if(role.Name == "LMPF")
                {
                    var dropdownOptions = new List<DiscordSelectComponentOption>();
                    var getTickets = await DBUtil_ReportTicket.GetAllReportTicketsAsync();
                    var tickets = getTickets.Item2.ToArray();

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

                    foreach(var ticket in tickets)
                    {
                        var selectOptions = new DiscordSelectComponentOption(
                            $"TicketNo: {ticket.TicketNo}", $"{ticket.TicketNo}", $"Plantiff: {ticket.Plantiff} v. Defendant: {ticket.Defendant}"
                            );
                        dropdownOptions.Add(selectOptions);
                    }

                    var createDropDown = new DiscordSelectComponent("dpdwnLMPFGetTicket", null, dropdownOptions, false, 0, dropdownOptions.Count());

                    var dropdownTicketEmbed = new DiscordMessageBuilder()
                        .AddEmbed(new DiscordEmbedBuilder()
                            .WithTitle("Select the ID of the ticket you want to retrieve.")
                            .WithColor(DiscordColor.SpringGreen))
                        .AddComponents(createDropDown);

                    await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder(dropdownTicketEmbed));
                    return;
                }
            }
            await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Unable to retrieve  ticket for {ctx.Interaction.User.Username}, please check required roles."));
        }

        [ButtonCommand("btnLMPFDeleteTicket")]
        public async Task DeleteReportTicket(ButtonContext ctx)
        {
            await ctx.Interaction.DeferAsync(true);

            var DBUtil_ReportTicket = new DBUtil_ReportTicket();
            var user = (DiscordMember)ctx.Interaction.User;
            var userRoles = user.Roles.ToArray();

            foreach(var role in userRoles)
            {
                if(role.Name == "LMPF")
                {
                    var dropdownOptions = new List<DiscordSelectComponentOption>();
                    var getTickets = await DBUtil_ReportTicket.GetAllReportTicketsAsync();
                    var tickets = getTickets.Item2.ToArray();

                    var isEmpty = await DBUtil_ReportTicket.isEmptyAsync();

                    if(isEmpty)
                    {
                        var embedFailed = new DiscordEmbedBuilder()
                        {
                            Color = DiscordColor.Red,
                            Title = "There are no tickets to be deleted!"
                        };
                        await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedFailed));
                        return;
                    }

                    foreach(var ticket in tickets)
                    {
                        var selectOptions = new DiscordSelectComponentOption(
                            $"TicketNo: {ticket.TicketNo}", $"{ticket.TicketNo}", $"Plantiff: {ticket.Plantiff} v. Defendant: {ticket.Defendant}"
                            );
                        dropdownOptions.Add(selectOptions);
                    }

                    var createDropDown = new DiscordSelectComponent("dpdwnLMPFDeleteTicket", null, dropdownOptions, false, 0, dropdownOptions.Count());

                    var dropdownTicketEmbed = new DiscordMessageBuilder()
                        .AddEmbed(new DiscordEmbedBuilder()
                            .WithTitle("Select the ID of the ticket you want to delete.")
                            .WithColor(DiscordColor.SpringGreen))
                        .AddComponents(createDropDown);

                    await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder(dropdownTicketEmbed));
                    return;
                }
            }

            await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Unable to delete ticket for {ctx.Interaction.User.Username}, please check required roles."));
        }



    }
}

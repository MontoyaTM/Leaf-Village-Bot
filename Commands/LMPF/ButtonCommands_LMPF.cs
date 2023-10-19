using DSharpPlus;
using DSharpPlus.ButtonCommands;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.ModalCommands;
using Leaf_Village_Bot.DBUtil.ReportTicket;
using Leaf_Village_Bot.DBUtil.Profile;
using System.Threading.Channels;

namespace Leaf_Village_Bot.Commands.LMPF
{
    public class ButtonCommands_LMPF : ButtonCommandModule
    {
        [ButtonCommand("btn_CreateTicket")]
        public async Task CreateReportTicket(ButtonContext ctx)
        {
            var modalTicketReport = ModalBuilder.Create("LMPFReportModal")
                .WithTitle("LMPF Report System")
                .AddComponents(new TextInputComponent("Plantiff:", "PlantiffTextBox", "Name of Reporter", null, true, TextInputStyle.Short))
                .AddComponents(new TextInputComponent("Defendant:", "DefendantTextBox", "Name(s) of Accused", null, true, TextInputStyle.Short))
                .AddComponents(new TextInputComponent("Date:", "DateTextBox", "Date of Incident (mm/dd/yyyy):", null, true, TextInputStyle.Short))
                .AddComponents(new TextInputComponent("Details:", "DtailsTextBox", "Please provided a detailed explainination of the situation", null, true, TextInputStyle.Paragraph));

            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modalTicketReport);
        }

        [ButtonCommand("btn_CloseTicket")]
        public async Task CloseReportTicket(ButtonContext ctx)
        {
            await ctx.Interaction.DeferAsync();

            var hasLMPFRole = ctx.Member.Roles.Any(x => x.Name == "LMPF");
            
            if (hasLMPFRole)
            {
                var DBUtil_Profile = new DBUtil_Profile();
                var guildChannels = await ctx.Interaction.Guild.GetChannelsAsync();
                var recordsChannel = guildChannels.FirstOrDefault(x => x.Name == "report-records");

                if (recordsChannel == null)
                {
                    await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
                        .WithContent($"Channel: report-records does not exist, please create the channel to store records."));
                    return;
                }

                var embedMessage = ctx.Message.Embeds.First();
                var geninRole = ctx.Guild.Roles.FirstOrDefault(x => x.Value.Name == "Genin");

                var OverWriteBuilderList = new DiscordOverwriteBuilder[] { new DiscordOverwriteBuilder(geninRole.Value).Deny(Permissions.SendMessages) };

                await ctx.Channel.ModifyAsync(x => x.PermissionOverwrites = OverWriteBuilderList);

                var embedReason = new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.SpringGreen,
                    Title = "Please enter the conclusion of the investigation as the next message in this channel."
                };

                var followupMessage = await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedReason));

                var reason = await ctx.Channel.GetNextMessageAsync();

                var embedFields = embedMessage.Fields;
                var profileImage = await DBUtil_Profile.GetProfileImageAsync(ctx.Interaction.User.Id);
                var embedFieldLists = new List<string>();

                foreach (var field in embedFields)
                {
                    embedFieldLists.Add(field.Value);
                }


                var embedReportRecord = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.SpringGreen)
                    .WithTitle($"Leaf Military Police Force Report")
                    .WithImageUrl(profileImage.Item2)
                    .WithThumbnail(Global.LeafSymbol_URL)
                    .AddField("Plantiff:", embedFieldLists[0], true)
                    .AddField("Defendant:", embedFieldLists[1], true)
                    .AddField("Date", embedFieldLists[2], true)
                    .AddField("Details", embedFieldLists[3])
                    .AddField("Report Closed by:", ctx.Interaction.User.Username)
                    .WithFooter($"Verdict: \n{reason.Result.Content}\n")
                    );

                await ctx.Client.SendMessageAsync(await ctx.Client.GetChannelAsync(recordsChannel.Id), embedReportRecord);

                await ctx.Interaction.EditFollowupMessageAsync(followupMessage.Id, new DiscordWebhookBuilder()
                    .WithContent($"Your response was sent to the report-records channel."));

                await ctx.Channel.DeleteAsync();
            } else
            {
                await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Unable to close ticket for {ctx.Interaction.User.Username}, please check required roles."));
            }
        }

        [ButtonCommand("btn_ViewTicket")]
        public async Task ViewReportTickets(ButtonContext ctx)
        {
            await ctx.Interaction.DeferAsync(true);

            var DBUtil_ReportTicket = new DBUtil_ReportTicket();
            var serverid = ctx.Interaction.Guild.Id;

            var isEmpty = await DBUtil_ReportTicket.isEmptyAsync(serverid);

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
                var retrieveTickets = await DBUtil_ReportTicket.GetAllReportTicketsAsync(serverid);

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
                    outputArray[i] = $"TicketNo: **{item.TicketID}** \nPlantiff: **{item.Plantiff}** v. Defendant: **{item.Defendant}**\n";
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

        [ButtonCommand("btn_GetTicket")]
        public async Task GetReportTicket(ButtonContext ctx)
        {
            await ctx.Interaction.DeferAsync(true);

            var DBUtil_ReportTicket = new DBUtil_ReportTicket();
            var serverid = ctx.Interaction.Guild.Id;

            var isEmpty = await DBUtil_ReportTicket.isEmptyAsync(serverid);

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
                var retrieveOptions = await GetSelectComponentOptions(serverid);

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

                var createDropDown = new DiscordSelectComponent("dpdwn_GetTicket", null, retrieveOptions.Item2, false, 0, retrieveOptions.Item2.Count);

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

        [ButtonCommand("btn_DeleteTicket")]
        public async Task DeleteReportTicket(ButtonContext ctx)
        {
            await ctx.Interaction.DeferAsync(true);

            var DBUtil_ReportTicket = new DBUtil_ReportTicket();
            var serverid = ctx.Interaction.Guild.Id;

            var isEmpty = await DBUtil_ReportTicket.isEmptyAsync(serverid);

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
                var retrieveOptions = await GetSelectComponentOptions(serverid);

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

                var createDropDown = new DiscordSelectComponent("dpdwn_DeleteTicket", null, retrieveOptions.Item2, false, 0, retrieveOptions.Item2.Count);

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

        public async Task<(bool, List<DiscordSelectComponentOption>)> GetSelectComponentOptions(ulong serverid)
        {
            var dropdownOptions = new List<DiscordSelectComponentOption>();
            var DBUtil_ReportTicket = new DBUtil_ReportTicket();
            var retrieveTickets = await DBUtil_ReportTicket.GetAllReportTicketsAsync(serverid);

            if (retrieveTickets.Item1 == false)
            {
                return (false, null);
            }

            foreach (var ticket in retrieveTickets.Item2)
            {
                var selectOptions = new DiscordSelectComponentOption(
                    $"TicketID: {ticket.TicketID}", $"{ticket.TicketID}", $"Plantiff: {ticket.Plantiff} v. Defendant: {ticket.Defendant}"
                    );
                dropdownOptions.Add(selectOptions);
            }

            return (true, dropdownOptions);
        }
    }
}

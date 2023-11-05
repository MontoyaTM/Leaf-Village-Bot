using DSharpPlus;
using DSharpPlus.ButtonCommands;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.ModalCommands;
using DSharpPlus.Net;
using Leaf_Village_Bot.DBUtil.Profile;
using Leaf_Village_Bot.DBUtil.RPRequest;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leaf_Village_Bot.Commands.Ranked
{
    internal class ButtonCommands_Ranked : ButtonCommandModule
    {
        [ButtonCommand("btn_CreateRequest")]
        public async Task CreateRPRequest(ButtonContext ctx)
        {
            var modalRPRequest = ModalBuilder.Create("RPRequestModal")
                .WithTitle("Leaf Village RP Request")
                .AddComponents(new TextInputComponent("IGN:", "ingamenameTextBoxRP", "Name of Character", null, true, TextInputStyle.Short))
                .AddComponents(new TextInputComponent("RP Mission", "missionTextBoxRP", "RP Mission (I-VII)", null, true, TextInputStyle.Short))
                .AddComponents(new TextInputComponent("Timezone:", "timezoneTextBoxRP", "Timezone", null, true, TextInputStyle.Short))
                .AddComponents(new TextInputComponent("Attendees:", "attendeesTextBoxRP", "IGN, Ninja 2, Ninja 3", null, true, TextInputStyle.Short));

            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modalRPRequest);

        }

        [ButtonCommand("btn_CloseRequest")]
        public async Task CloseRPRequest(ButtonContext ctx)
        {
            await ctx.Interaction.DeferAsync();

            var hasRankedRole = ctx.Member.Roles.Any(x => x.Name == "Chunin" || x.Name == "Jonin");

            if (hasRankedRole)
            {
                var interactivity = ctx.Client.GetInteractivity();
                var DBUtil_RPRequest = new DBUtil_RPRequest();
                var DBUtil_Profile = new DBUtil_Profile();

                var guildChannels = await ctx.Interaction.Guild.GetChannelsAsync();
                var recordsChannel = guildChannels.FirstOrDefault(x => x.Name == "rp-records");

                if (recordsChannel == null)
                {
                    await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
                        .WithContent($"Channel: rp-records does not exist, please create the channel to store records."));
                    return;
                }

                var embedMessage = ctx.Message.Embeds.First();
                var geninRole = ctx.Guild.Roles.FirstOrDefault(x => x.Value.Name == "Genin");

                var OverWriteBuilderList = new DiscordOverwriteBuilder[] { new DiscordOverwriteBuilder(geninRole.Value).Deny(Permissions.SendMessages) };
                await ctx.Channel.ModifyAsync(x => x.PermissionOverwrites = OverWriteBuilderList);

                var embedMessage1 = new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.SpringGreen,
                    Title = "Please provide a (screenshot and a detailed description) of your RP Mission as the next message in this channel."
                };
                
                var followupMessage_1 = await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedMessage1));

                var details = await interactivity.WaitForMessageAsync(x => x.Author.Id == ctx.Interaction.User.Id, TimeSpan.FromMinutes(5));

                if (details.Result.Attachments.Count ==  0)
                {
                    await ctx.Interaction.EditFollowupMessageAsync(followupMessage_1.Id, new DiscordWebhookBuilder().WithContent("There was no screenshot, try again!"));
                    return;                
                }

                DiscordAttachment image = details.Result.Attachments[0];

                HttpClient client = new HttpClient();
                Stream stream = await client.GetStreamAsync(image.Url);

                using (var fileStream = new FileStream("Images/output.png", FileMode.Create))
                {
                    stream.CopyTo(fileStream);
                }

                FileStream file = new FileStream("Images/output.png", FileMode.Open, FileAccess.Read);

                var embedImage = new DiscordMessageBuilder()
                    .AddFile(file);

                

                var embedMessage2 = new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.SpringGreen,
                    Title = "Please write the names of the 3 Ninjas who participated in the RP Mission as the next message in this channel. " +
                            "\n\n Ex. Ninja1, Ninja2, Ninja3"
                };
                await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedMessage2));
                var attendees = await interactivity.WaitForMessageAsync(x => x.Author.Id == ctx.Interaction.User.Id, TimeSpan.FromMinutes(5));

                var embedMessage3 = new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.SpringGreen,
                    Title = "Please write the date the RP Mission was completed as the next message in the channel. " +
                            "\n\nmm/day/yyyy"
                };
                await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedMessage3));
                var datetime = await interactivity.WaitForMessageAsync(x => x.Author.Id == ctx.Interaction.User.Id, TimeSpan.FromMinutes(5));

                var embedFields = embedMessage.Fields;
                var profileImage = await DBUtil_Profile.GetProfileImageAsync(ctx.Interaction.User.Id);
                var embedFieldLists = new List<string>();

                foreach (var field in embedFields)
                {
                    embedFieldLists.Add(field.Value);
                }

                var embedRPRecord = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithColor(DiscordColor.SpringGreen)
                        .WithTitle($"RP Mission Request Record")
                        .WithImageUrl(profileImage.Item2)
                        .WithThumbnail(Global.LeafSymbol_URL)
                        .AddField("Proctor:", ctx.Interaction.User.Username, true)
                        .AddField("RP Mission:", embedFieldLists[1], true)
                        .AddField("Date/Time:", datetime.Result.Content)
                        .AddField("Attendees", attendees.Result.Content)
                        .AddField("Details:", details.Result.Content)
                        .WithFooter($"{ctx.Interaction.User.Username} had successfully proctored an RP mission and has incremented their proctored missions stat! Congratulations!")
                    );

                await ctx.Client.SendMessageAsync(await ctx.Client.GetChannelAsync(recordsChannel.Id), embedRPRecord);
                await ctx.Client.SendMessageAsync(await ctx.Client.GetChannelAsync(recordsChannel.Id), embedImage);

                await DBUtil_RPRequest.UpdateProctoredMissionsAsync(ctx.Interaction.User.Id);

                await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Your response was sent to the rp-records channel."));

                await ctx.Channel.DeleteAsync();
            } else
            {
                await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Unable to close request for {ctx.Interaction.User.Username}, please check required roles."));
            }
        }

        [ButtonCommand("btn_ViewRequest")]
        public async Task ViewRPRequest(ButtonContext ctx)
        {
            await ctx.Interaction.DeferAsync(true);

            var DBUtil_RPRequest = new DBUtil_RPRequest();
            var serverid = ctx.Interaction.Guild.Id;

            var isEmpty = await DBUtil_RPRequest.isEmptyAsync(serverid);

            if (isEmpty)
            {
                var embedFailed = new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Red,
                    Title = "There are no requests to be viewed!"
                };
                await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedFailed));
                return;
            }

            var hasRankedRole = ctx.Member.Roles.Any(x => x.Name == "Chunin" || x.Name == "Jonin");

            if (hasRankedRole)
            {
                var retrieveRequest = await DBUtil_RPRequest.GetAllReportTicketsAsync(serverid);

                if (retrieveRequest.Item1 == false)
                {
                    var embedFailed = new DiscordEmbedBuilder()
                    {
                        Color = DiscordColor.Red,
                        Title = "Database retrieval of all request has returned false!"
                    };
                    await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedFailed));
                    return;
                }

                var requestsArray = retrieveRequest.Item2.ToArray();

                string[] outputArray = new string[requestsArray.Length];
                int i = 0;
                foreach (var item in requestsArray)
                {
                    outputArray[i] = $"**RequestID:** {item.RequestID}, \n**IGN:** {item.IngameName}, **RP Mission:** {item.RPMission}\n";
                    i++;
                }

                var embedRequests = new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.SpringGreen,
                    Title = "Leaf Village Active RP Requests",
                    Description = string.Join("\n", outputArray)
                };

                await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedRequests));
            } else
            {
                await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Unable to view request for {ctx.Interaction.User.Username}, please check required roles."));
            }
        }

        [ButtonCommand("btn_GetRequest")]
        public async Task GetRPRequest(ButtonContext ctx)
        {
            await ctx.Interaction.DeferAsync(true);

            var DBUtil_DBRequest = new DBUtil_RPRequest();
            var serverid = ctx.Interaction.Guild.Id;

            var isEmpty = await DBUtil_DBRequest.isEmptyAsync(serverid);

            if (isEmpty)
            {
                var embedFailed = new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Red,
                    Title = "There are no requests to be retrieved!"
                };

                await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedFailed));
                return;
            }

            var hasRankedRole = ctx.Member.Roles.Any(x => x.Name == "Chunin" || x.Name == "Jonin");

            if (hasRankedRole)
            {
                var retrieveOptions = await GetSelectComponentOptions(serverid);

                if (retrieveOptions.Item1 == false)
                {
                    var embedFailed = new DiscordEmbedBuilder()
                    {
                        Color = DiscordColor.Red,
                        Title = "Database retrieval of all requests has returned false!"
                    };
                    await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedFailed));
                    return;
                }

                var createDropDown = new DiscordSelectComponent("dpdwnGetRequest", null, retrieveOptions.Item2, false, 0, retrieveOptions.Item2.Count);

                var dropdownTicketEmbed = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithTitle("Select the ID of the request you want to retrieve.")
                        .WithColor(DiscordColor.SpringGreen))
                    .AddComponents(createDropDown);

                await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder(dropdownTicketEmbed));
            } else
            {
                await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Unable to retrieve  request for {ctx.Interaction.User.Username}, please check required roles."));
            }
        }

        [ButtonCommand("btn_DeleteRequest")]
        public async Task DeleteRPRequest(ButtonContext ctx)
        {
            await ctx.Interaction.DeferAsync(true);

            var DBUtil_DBRequest = new DBUtil_RPRequest();
            var serverid = ctx.Interaction.Guild.Id;

            var isEmpty = await DBUtil_DBRequest.isEmptyAsync(serverid);

            if (isEmpty)
            {
                var embedFailed = new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Red,
                    Title = "There are no requests to be retrieved!"
                };

                await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedFailed));
                return;
            }

            var hasRankedRole = ctx.Member.Roles.Any(x => x.Name == "Chunin" || x.Name == "Jonin");

            if (hasRankedRole)
            {
                var retrieveOptions = await GetSelectComponentOptions(serverid);

                if (retrieveOptions.Item1 == false)
                {
                    var embedFailed = new DiscordEmbedBuilder()
                    {
                        Color = DiscordColor.Red,
                        Title = "Database retrieval of all requests has returned false!"
                    };
                    await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedFailed));
                    return;
                }

                var createDropDown = new DiscordSelectComponent("dpdwnDeleteRequest", null, retrieveOptions.Item2, false, 0, retrieveOptions.Item2.Count);

                var dropdownTicketEmbed = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithTitle("Select the ID of the request you want to delete.")
                        .WithColor(DiscordColor.SpringGreen))
                    .AddComponents(createDropDown);

                await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder(dropdownTicketEmbed));
            }

        }

        public async Task<(bool, List<DiscordSelectComponentOption>)> GetSelectComponentOptions(ulong serverid)
        {
            var dropdownOptions = new List<DiscordSelectComponentOption>();
            var DBUtil_RPRequest = new DBUtil_RPRequest();
            var retrieveRequests = await DBUtil_RPRequest.GetAllReportTicketsAsync(serverid);

            if (retrieveRequests.Item1 == false)
            {
                return (false, null);
            }

            foreach (var request in retrieveRequests.Item2)
            {
                var selectOptions = new DiscordSelectComponentOption(
                    $"RequesID: {request.RequestID}", $"{request.RequestID}", $"{request.IngameName} — {request.RPMission}"
                    );
                dropdownOptions.Add(selectOptions);
            }
            return (true, dropdownOptions);

        }
    }
}

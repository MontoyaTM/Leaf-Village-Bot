using DSharpPlus.ButtonCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leaf_Village_Bot.DBUtil.Profile;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus;
using DSharpPlus.Net.Models;
using DSharpPlus.Interactivity;

namespace Leaf_Village_Bot.Commands.Hokage
{
    public class ButtonCommands_Hokage : ButtonCommandModule
    {
        [ButtonCommand("btn_DeleteApplication")]
        public async Task DeleteApplication(ButtonContext ctx)
        {
            await ctx.Interaction.DeferAsync(true);

            var hasRole = ctx.Member.Roles.Any(x => x.Name == "Hokage" || x.Name == "Council");

            if (hasRole)
            {
                var DBUtil_Profile = new DBUtil_Profile();
                var interactivity = ctx.Client.GetInteractivity();

                var guildChannels = await ctx.Guild.GetChannelsAsync();
                var recordsChannel = guildChannels.FirstOrDefault(x => x.Name == "hokage-records");

                if (recordsChannel == null)
                {
                    await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent("Channel: Record Channel does not exist!"));
                    return;
                }

                var isMemberIdRetrieved = await RetrieveMemberID(ctx,  interactivity);

                if(isMemberIdRetrieved.Item1)
                {
                    ulong memberID = isMemberIdRetrieved.Item2;

                    var isDeleted = await DBUtil_Profile.DeleteVillagerApplicationAsync(memberID);

                    if(isDeleted)
                    {
                        var dateTime = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt");

                        var embedDeleted = new DiscordMessageBuilder()
                        .AddEmbed(new DiscordEmbedBuilder()
                            .WithColor(DiscordColor.SpringGreen)
                            .WithTitle("Leaf Village — Delete Application")
                            .WithDescription($"Successfully deleted **{isMemberIdRetrieved.Item4.Username}**'s villager application from the database.")
                            .WithFooter("• " + dateTime + "     • Executed by: " + ctx.Interaction.User.Username)
                        );

                        await ctx.Interaction.EditFollowupMessageAsync(isMemberIdRetrieved.Item3.Id, new DiscordWebhookBuilder()
                            .WithContent($"Successfully deleted {isMemberIdRetrieved.Item4.Username}'s villager application!"));

                        await ctx.Client.SendMessageAsync(recordsChannel, embedDeleted);

                    } else
                    {
                        await ctx.Interaction.EditFollowupMessageAsync(isMemberIdRetrieved.Item3.Id, new DiscordWebhookBuilder()
                            .WithContent($"Failed to delete villager application!"));
                    }
                } else
                {
                    return;
                }
            } else
            {
                await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
                    .WithContent($"Unable to delete application for {ctx.Interaction.User.Username}, please check required roles."));
            }
        }

        [ButtonCommand("btn_GetAltList")]
        public async Task GetAltList(ButtonContext ctx)
        {
            await ctx.Interaction.DeferAsync(true);

            var hasRole = ctx.Member.Roles.Any(x => x.Name == "Hokage" || x.Name == "Council");

            if (hasRole)
            {
                var DBUtil_Profile = new DBUtil_Profile();
                var interactivity = ctx.Client.GetInteractivity();

                var guildChannels = await ctx.Guild.GetChannelsAsync();
                var recordsChannel = guildChannels.FirstOrDefault(x => x.Name == "hokage-records");

                var isMemberIdRetrieved = await RetrieveMemberID(ctx, interactivity);

                if(isMemberIdRetrieved.Item1)
                {
                    ulong memberID = isMemberIdRetrieved.Item2;

                    var isAltsRetrieved = await DBUtil_Profile.GetAltsListAsync(memberID);

                    if(isAltsRetrieved.Item1)
                    {
                        var dateTime = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt");

                        var embedAlts = new DiscordMessageBuilder()
                        .AddEmbed(new DiscordEmbedBuilder()
                            .WithColor(DiscordColor.SpringGreen)
                            .WithTitle($"{isMemberIdRetrieved.Item4.Username} Alt(s) List:")
                            .WithDescription(isAltsRetrieved.Item2)
                            .WithThumbnail(Global.LeafSymbol_URL)
                            .WithImageUrl(isMemberIdRetrieved.Item4.AvatarUrl)
                            .WithFooter("• " + dateTime + "     • Executed by: " + ctx.Interaction.User.Username)
                        );

                        await ctx.Interaction.EditFollowupMessageAsync(isMemberIdRetrieved.Item3.Id, new DiscordWebhookBuilder()
                            .WithContent($"Successfully deleted {isMemberIdRetrieved.Item4.Username}'s villager application!"));

                        await ctx.Client.SendMessageAsync(recordsChannel, embedAlts);
                    } else
                    {
                        await ctx.Interaction.EditFollowupMessageAsync(isMemberIdRetrieved.Item3.Id, new DiscordWebhookBuilder()
                            .WithContent($"Unable to retrieve alts!"));
                    }

                } else
                {
                    return;
                }
            } else
            {
                await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
                    .WithContent($"Unable to delete application for {ctx.Interaction.User.Username}, please check required roles."));
            }
        }

        [ButtonCommand("btn_AcceptApplicant")]
        public async Task AcceptVillager(ButtonContext ctx)
        {
            await ctx.Interaction.DeferAsync(true);

            var DBUtil_Profile = new DBUtil_Profile();

            var hasLMPFRole = ctx.Member.Roles.Any(x => x.Name == "Hokage" || x.Name == "Council");

            if (hasLMPFRole)
            {
                var embedMessage = ctx.Message.Embeds.First();

                var geninRole = ctx.Guild.Roles.FirstOrDefault(x => x.Value.Name == "Genin");

                var memberID = ulong.Parse(embedMessage.Footer.Text);

                DiscordMember member = await ctx.Guild.GetMemberAsync(memberID);

                await member.GrantRoleAsync(geninRole.Value);

                var guildChannels = await ctx.Interaction.Guild.GetChannelsAsync();
                var acceptedChannel = guildChannels.FirstOrDefault(x => x.Name == "application-accepted");

                if (acceptedChannel == null)
                {
                    await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
                        .WithContent($"Channel: application-accepted does not exist, please create the channel to store records."));
                    return;
                }

                await ctx.Client.SendMessageAsync(await ctx.Client.GetChannelAsync(acceptedChannel.Id), embedMessage);

                await ctx.Message.DeleteAsync();

                await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
                                    .WithContent($"Successfully granted Genin role to {member.Username}!"));

            }
            else
            {
                await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
                    .WithContent($"Unable to accept villager for {ctx.Interaction.User.Username}, please check required roles."));
            }
        }

        [ButtonCommand("btn_DeclineApplicant")]
        public async Task DeclineVillager(ButtonContext ctx)
        {
            await ctx.Interaction.DeferAsync(true);

            var DBUtil_Profile = new DBUtil_Profile();

            var hasLMPFRole = ctx.Member.Roles.Any(x => x.Name == "Hokage" || x.Name == "Council");

            if (hasLMPFRole)
            {
                var interactivity = ctx.Client.GetInteractivity();
                var embedMessage = ctx.Message.Embeds.First();

                var memberID = ulong.Parse(embedMessage.Footer.Text);

                DiscordMember member = await ctx.Guild.GetMemberAsync(memberID);

                var embedReason = new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.SpringGreen,
                    Title = "Please enter the reason for denying application as the next message."
                };

                var followupMessage = await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedReason));
                var reason = await interactivity.WaitForMessageAsync(x => x.Author.Id == ctx.Interaction.User.Id, TimeSpan.FromMinutes(5));

                var embedDenied = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.Red)
                    .WithTitle($"Application Denied")
                    .WithImageUrl(member.AvatarUrl)
                    .WithThumbnail(Global.LeafSymbol_URL)
                    .WithDescription($"{reason.Result.Content}")
                );

                await member.SendMessageAsync(embedDenied);

                await ctx.Message.DeleteAsync();
                await ctx.Interaction.EditFollowupMessageAsync(followupMessage.Id, new DiscordWebhookBuilder().WithContent($"Your response was sent to {member.Username}."));

                await ctx.Channel.DeleteMessageAsync(reason.Result);

                var guildChannels = await ctx.Interaction.Guild.GetChannelsAsync();
                var deniedChannel = guildChannels.FirstOrDefault(x => x.Name == "application-denied");

                if (deniedChannel == null)
                {
                    await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
                        .WithContent($"Channel: application-denied does not exist, please create the channel to store records."));
                    return;
                }

                var embedFields = embedMessage.Fields;
                var embedFieldLists = new List<string>();

                foreach (var field in embedFields)
                {
                    embedFieldLists.Add(field.Value);
                }

                var embedApplicationDenied = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.Red)
                    .WithTitle($"Leaf Village Application")
                    .WithImageUrl(member.AvatarUrl)
                    .WithThumbnail(Global.LeafSymbol_URL)
                    .AddField("IGN:", embedFieldLists[0])
                    .AddField("Introduction:", embedFieldLists[1])
                    .AddField("Alt(s):", embedFieldLists[2])
                    .WithFooter($"{memberID}\nApplication Denied: \n{reason.Result.Content}\n")
                    );

                await ctx.Client.SendMessageAsync(await ctx.Client.GetChannelAsync(deniedChannel.Id), embedApplicationDenied);
            }
            else
            {
                await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Unable to deny applicant for {ctx.Interaction.User.Username}, please check required roles."));
            }
        }

        public async Task<(bool, ulong, DiscordMessage, DiscordMember?)> RetrieveMemberID(ButtonContext ctx, InteractivityExtension interactivity)
        {
            var embedMessage = new DiscordEmbedBuilder()
            {
                Color = DiscordColor.SpringGreen,
                Title = "Enter the MemberID of the applicant as the next message in this channel."
            };

            var embedMemberID = await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedMessage));
            var memberIdResponse = await interactivity.WaitForMessageAsync(x => x.Author.Id == ctx.Interaction.User.Id, TimeSpan.FromMinutes(2));

            DiscordMember? member;
            ulong memberID;

            if(ulong.TryParse(memberIdResponse.Result.Content, out memberID))
            {
                await ctx.Channel.DeleteMessageAsync(memberIdResponse.Result);
            }
            else
            {
                await ctx.Interaction.EditFollowupMessageAsync(embedMemberID.Id, new DiscordWebhookBuilder()
                    .WithContent("Unable to convert your response to a numerical value, please check your submission!"));
                await ctx.Channel.DeleteMessageAsync(memberIdResponse.Result);
                return (false, 0, embedMemberID, null);
            }

            try
            {
                member = await ctx.Guild.GetMemberAsync(memberID);
            } catch (Exception ex)
            {
                await ctx.Interaction.EditFollowupMessageAsync(embedMemberID.Id, new DiscordWebhookBuilder()
                            .WithContent("MemberID does not match any user inside this server, please check your response!"));
                return (false, 0, embedMemberID, null);
            }

            return (true, memberID, embedMemberID, member);
        }
    }
}


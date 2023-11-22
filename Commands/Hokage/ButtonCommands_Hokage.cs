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

                var isRetrievedResponse = await ResponseMemberID(ctx, interactivity);

                DiscordMember member;
                ulong memberID;

                if(isRetrievedResponse.Item1)
                {
                    memberID = isRetrievedResponse.Item2;

                    try
                    {
                        member = await ctx.Guild.GetMemberAsync(memberID);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);

                        await ctx.Interaction.EditFollowupMessageAsync(isRetrievedResponse.Item3.Id, new DiscordWebhookBuilder()
                            .WithContent("MemberID does not match any existing villager application, please check your response!"));
                        return;
                    }
                } else
                {
                    await ctx.Interaction.EditFollowupMessageAsync(isRetrievedResponse.Item3.Id, new DiscordWebhookBuilder().WithContent("Unable to convert your response to a numerical value, please check your submission!"));
                    return;
                }
                
                var isDeleted = await DBUtil_Profile.DeleteVillagerApplicationAsync(memberID);

                if (isDeleted)
                {
                    await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Successfully deleted {member.Username}'s villager application!"));
                } else
                {
                    await ctx.Interaction.EditFollowupMessageAsync(isRetrievedResponse.Item3.Id, new DiscordWebhookBuilder().WithContent($"Failed to delete {member.Username}'s villager application!"));
                }
            } else
            {
                await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Unable to delete application for {ctx.Interaction.User.Username}, please check required roles."));
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

                var isRetrievedResponse = await ResponseMemberID(ctx, interactivity);

                DiscordMember member;
                ulong memberID;

                if (isRetrievedResponse.Item1)
                {
                    memberID = isRetrievedResponse.Item2;

                    try
                    {
                        member = await ctx.Guild.GetMemberAsync(memberID);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);

                        await ctx.Interaction.EditFollowupMessageAsync(isRetrievedResponse.Item3.Id, new DiscordWebhookBuilder()
                            .WithContent("MemberID does not match any existing villager application, please check your response!"));
                        return;
                    }
                }
                else
                {
                    await ctx.Interaction.EditFollowupMessageAsync(isRetrievedResponse.Item3.Id, new DiscordWebhookBuilder().WithContent("Unable to convert your response to a numerical value, please check your submission!"));
                    return;
                }

                var isRetrievedAlts = await DBUtil_Profile.GetAltsListAsync(memberID);
                
                var embedAlts = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.Red)
                    .WithTitle($"{member.Username} Alt(s) List:")
                    .WithDescription(isRetrievedAlts.Item2)
                    .WithThumbnail(Global.LeafSymbol_URL)
                    .WithImageUrl(member.AvatarUrl)
                );

                await ctx.Interaction.EditFollowupMessageAsync(isRetrievedResponse.Item3.Id, new DiscordWebhookBuilder(embedAlts));
            }
        }

        public async Task<(bool, ulong, DiscordMessage)> ResponseMemberID(ButtonContext ctx, InteractivityExtension interactivity)
        {
            var embedMessage = new DiscordEmbedBuilder()
            {
                Color = DiscordColor.SpringGreen,
                Title = "Enter the MemberID of the applicant as the next message in this channel."
            };

            var followupMessage = await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedMessage));

            var nextMessage = await interactivity.WaitForMessageAsync(x => x.Author.Id == ctx.Interaction.User.Id, TimeSpan.FromMinutes(5));

            ulong memberID;

            if (ulong.TryParse(nextMessage.Result.Content, out memberID))
            {
                await ctx.Channel.DeleteMessageAsync(nextMessage.Result);
                return (true, memberID, followupMessage);
            }
            else
            {
                await ctx.Channel.DeleteMessageAsync(nextMessage.Result);
                return (false, 0, followupMessage);
            }
        }
    }
}


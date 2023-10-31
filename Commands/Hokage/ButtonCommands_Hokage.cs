﻿using DSharpPlus.ButtonCommands;
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

namespace Leaf_Village_Bot.Commands.Hokage
{
    public class ButtonCommands_Hokage : ButtonCommandModule
    {
        [ButtonCommand("btn_DeleteApplication")]
        public async Task DeleteApplication(ButtonContext ctx)
        {
            await ctx.Interaction.DeferAsync(true);

            var DBUtil_Profile = new DBUtil_Profile();

            var hasLMPFRole = ctx.Member.Roles.Any(x => x.Name == "Hokage");

            if(hasLMPFRole)
            {
                var embedMessage = new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.SpringGreen,
                    Title = "Enter the MemberID of the application you wish to delete as the next message in this channel."
                };
                var followupMessage = await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedMessage));

                var nextMessage = await ctx.Channel.GetNextMessageAsync();
                var memberID = ulong.Parse(nextMessage.Result.Content);
                DiscordMember member = null;

                try
                {
                     member = await ctx.Guild.GetMemberAsync(memberID);
                } catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);

                    var invalidUser = new DiscordEmbedBuilder()
                    {
                        Color = DiscordColor.Red,
                        Title = $"MemberID does not match any existing villager application, please double check the MemberID!"
                    };
                    await ctx.Channel.DeleteMessageAsync(nextMessage.Result);
                    await ctx.Interaction.EditFollowupMessageAsync(followupMessage.Id, new DiscordWebhookBuilder().AddEmbed(invalidUser));
                    return;
                }

                var isDeleted = await DBUtil_Profile.DeleteVillagerApplication(memberID);

                await ctx.Channel.DeleteMessageAsync(nextMessage.Result);

                if (isDeleted)
                {
                    var success = new DiscordEmbedBuilder()
                    {
                        Color = DiscordColor.SpringGreen,
                        Title = $"Successfully deleted {member.Username}'s villager application!"
                    };

                    await ctx.Interaction.EditFollowupMessageAsync(followupMessage.Id, new DiscordWebhookBuilder().AddEmbed(success));
                } else
                {
                    var failed = new DiscordEmbedBuilder()
                    {
                        Color = DiscordColor.Red,
                        Title = $"Failed to delete {member.Username}'s villager application!"
                    };

                    await ctx.Interaction.EditFollowupMessageAsync(followupMessage.Id, new DiscordWebhookBuilder().AddEmbed(failed));
                }
            } else
            {
                await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Unable to delete application for {ctx.Interaction.User.Username}, please check required roles."));
            }
        }

        [ButtonCommand("btn_VillageRaid")]
        public async Task VillageRaid(ButtonContext ctx)
        {
            await ctx.Interaction.DeferAsync(true);

            var hasHokageRole = ctx.Member.Roles.Any(x => x.Name == "Hokage");

            if (hasHokageRole)
            {
                var DBUtil_Profile = new DBUtil_Profile();

                var guildChannels = await ctx.Guild.GetChannelsAsync();
                var raidChannel = guildChannels.FirstOrDefault(x => x.Name == "Village Raid");

                if (raidChannel == null)
                {
                    var embedFailed = new DiscordEmbedBuilder()
                    {
                        Title = "Villager Raid Failed",
                        Color = DiscordColor.Red,
                        Description = "Channel: Village Raid does not exist!"
                    };

                    await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedFailed));
                    return;
                }

                var Members = raidChannel.Users.ToList();

                foreach(var member in Members)
                {
                    var isUpdated =  await DBUtil_Profile.UpdateRaid(member.Id);

                    if (!isUpdated)
                    {
                        await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"{member.Nickname} raid stat was not updated!"));
                    }
                }

                await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Successfully increased all Member's Raids stat!"));

            } else
            {
                await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Unable to delete application for {ctx.Interaction.User.Username}, please check required roles."));
            }

                
            
        }
    }
}


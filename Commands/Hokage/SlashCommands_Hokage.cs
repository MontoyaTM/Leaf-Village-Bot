using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Net.Models;
using DSharpPlus.SlashCommands;
using Leaf_Village_Bot.DBUtil.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leaf_Village_Bot.Commands.Hokage
{
    public class SlashCommands_Hokage : ApplicationCommandModule
    {
        [SlashCommand("update_user_organization", "Updates a user's Organization & Rank title on their profile.")]
        public async Task UpdateOrg(InteractionContext ctx, [Option("User", "The user you wish to assign Organization and Rank.")] DiscordUser User,

                                                            [Choice("12 Guardians", "12 Guardians")]
                                                            [Choice("Leaf Military Police Force", "Leaf Military Police Force")]
                                                            [Choice("Lead Medical Corp", "Lead Medical Corp")]
                                                            [Choice("Leaf ANBU", "Leaf ANBU")]
                                                            [Option("Organization", "The organization to assign the user.")] string Organization, 
            
                                                            [Option("Rank", "The rank to assign the user.")] string Rank)
        {
            await ctx.DeferAsync(true);

            var hasRole = ctx.Member.Roles.Any(x => x.Name == "Hokage" || x.Name == "Org Leader");

            if (hasRole)
            {
                DBUtil_Profile dBUtil_Profile = new DBUtil_Profile();

                var Member = (DiscordMember)User;
                var isAssigned = await dBUtil_Profile.UpdateOrgAsync(Member.Id, Organization, Rank);

                if(isAssigned)
                {
                    await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Successfully assigned {Organization} {Rank} to {User.Username}!"));
                } else
                {
                    await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Failed to assign {Organization} {Rank} to {User.Username}!"));
                }

            } else
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("You do not have the role to access this command!"));
            }

        } 


        [SlashCommand("moveuser_villageraid", "Moves a user from the Raid Lobby voice channel to the Village Raid voice channel.")]
        public async Task MoveUser_VC(InteractionContext ctx, [Option("User", "The user to move to the Village Raid voice channel.")] DiscordUser User)
        {
            await ctx.DeferAsync(true);

            var hasRole = ctx.Member.Roles.Any(x => x.Name == "Hokage" || x.Name == "Raid Leader");

            if (hasRole)
            {
                var guildChannels = await ctx.Guild.GetChannelsAsync();
                var raidChannel = guildChannels.FirstOrDefault(x => x.Name == "Village Raid");

                var Member = (DiscordMember)User;

                if (Member.VoiceState != null)
                {
                    await Member.ModifyAsync(delegate (MemberEditModel Edit)
                    {
                        Edit.VoiceChannel = raidChannel;
                    });

                    await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Successfully moved {User.Username} to the Village Raid!"));
                }
                else
                {
                    await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Failed to move {User.Username} to the Village Raid. Please make sure user is in a voice channel!"));
                }
            }
            else
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("You do not have the role to access this command!"));
            }
        }

        [SlashCommand("moveall_villageraid", "Moves all users in the Raid Lobby voice channel to the Village Raid voice channel.")]
        public async Task MoveAll_VC(InteractionContext ctx)
        {
            await ctx.DeferAsync(true);

            var hasRole = ctx.Member.Roles.Any(x => x.Name == "Hokage" || x.Name == "Raid Leader");

            if (hasRole)
            {
                var guildChannels = await ctx.Guild.GetChannelsAsync();
                var raidLobby = guildChannels.FirstOrDefault(x => x.Name == "Raid Lobby");
                var raidChannel = guildChannels.FirstOrDefault(x => x.Name == "Village Raid");

                if (raidLobby == null)
                {
                    var embedFailed = new DiscordEmbedBuilder()
                    {
                        Title = "Village Raid Failed",
                        Color = DiscordColor.Red,
                        Description = "Channel: Raid Lobby does not exist!"
                    };

                    await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedFailed));
                    return;
                }

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

                var Members = raidLobby.Users.ToList();

                foreach (var member in Members)
                {
                    if (member.VoiceState != null)
                    {
                        await member.ModifyAsync(delegate (MemberEditModel Edit)
                        {
                            Edit.VoiceChannel = raidChannel;
                        });
                    }
                }

                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Successfully moved all users to the Village Raid!"));
            }
            else
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("You do not have the role to access this command!"));
            }
        }
    }
}

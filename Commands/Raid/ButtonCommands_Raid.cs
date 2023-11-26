using DSharpPlus.ButtonCommands;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Net.Models;
using Leaf_Village_Bot.DBUtil.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leaf_Village_Bot.Commands.Raid
{
    public class ButtonCommands_Raid : ButtonCommandModule
    {
        [ButtonCommand("btn_VillageRaid")]
        public async Task RaidCounter(ButtonContext ctx)
        {
            await ctx.Interaction.DeferAsync(true);

            var hasRole = ctx.Member.Roles.Any(x => x.Name == "Hokage" || x.Name == "Council" || x.Name == "Raid Leader");

            if (hasRole)
            {
                var DBUtil_Profile = new DBUtil_Profile();

                var guildChannels = await ctx.Guild.GetChannelsAsync();
                var raidChannel = guildChannels.FirstOrDefault(x => x.Name == "Village Raid");
                var recordsChannel = guildChannels.FirstOrDefault(x => x.Name == "raid-records");

                if (raidChannel == null)
                {
                    var embedFailed = new DiscordEmbedBuilder()
                    {
                        Title = "Raid++ Failed",
                        Color = DiscordColor.Red,
                        Description = "Channel: Village Raid does not exist!"
                    };

                    await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedFailed));
                    return;
                }

                if (recordsChannel == null)
                {
                    var embedFailed = new DiscordEmbedBuilder()
                    {
                        Title = "Raid++ Failed",
                        Color = DiscordColor.Red,
                        Description = "Channel: raid-records does not exist!"
                    };

                    await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedFailed));
                    return;
                }

                if (raidChannel.Users.Count() == 0)
                {
                    await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"There are no users in the Village Raid voice channel!"));
                    return;
                }

                var Members = raidChannel.Users.ToList();

                string[] username = new string[Members.Count];

                var i = 0;

                foreach (var member in Members)
                {
                    var userExists = await DBUtil_Profile.UserExistsAsync(member.Id);

                    if(userExists)
                    {
                        var isUpdated = await DBUtil_Profile.UpdateRaidAsync(member.Id);

                        username[i] = member.Username;
                        i++;
                    }
                }

                var dateTime = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt");
                var profileImage = await DBUtil_Profile.GetProfileImageAsync(ctx.Interaction.User.Id);

                var embedRaid = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithColor(DiscordColor.SpringGreen)
                        .WithTitle("Leaf Village — Raid++")
                        .WithDescription(string.Join("\n", username))
                        .WithFooter("• " + dateTime + "     • Raid Leader: " + ctx.Interaction.User.Username)
                    );

                await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Successfully increased all Member's Raids stat!"));

                await ctx.Client.SendMessageAsync(await ctx.Client.GetChannelAsync(recordsChannel.Id), embedRaid);

            }
            else
            {
                await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
                    .WithContent($"Unable to use button command Raid++ for {ctx.Interaction.User.Username}, please check required roles."));
            }
        }

        [ButtonCommand("btn_RetrieveMasteries")]
        public async Task RetrieveMasteries(ButtonContext ctx)
        {
            await ctx.Interaction.DeferAsync(true);

            var hasRole = ctx.Member.Roles.Any(x => x.Name == "Hokage" || x.Name == "Council" || x.Name == "Raid Leader");

            if (hasRole)
            {
                var DBUtil_Profile = new DBUtil_Profile();
                var guildChannels = await ctx.Guild.GetChannelsAsync();
                var raidChannel = guildChannels.FirstOrDefault(x => x.Name == "Village Raid");
                var warroomChannel = guildChannels.FirstOrDefault(x => x.Name == "war-room");

                if (raidChannel == null || warroomChannel == null)
                {
                    var embedFailed = new DiscordEmbedBuilder()
                    {
                        Title = "Raid++ Failed",
                        Color = DiscordColor.Red,
                        Description = "Channel: Village Raid or War Room does not exist!"
                    };

                    await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedFailed));
                    return;
                }

                if (raidChannel.Users.Count() == 0)
                {
                    await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"There are no users in the Village Raid voice channel!"));
                    return;
                }

                var Members = raidChannel.Users.ToArray();
                List<ulong> MemberIDs = new List<ulong>();

                foreach (var member in Members)
                {
                    MemberIDs.Add(member.Id);
                }

                var isRetrieved = await DBUtil_Profile.GetRaidMasteries(MemberIDs);

                if(isRetrieved.Item1)
                {
                    var embedMasteries = new DiscordMessageBuilder()
                        .AddEmbed(new DiscordEmbedBuilder()
                            .WithColor(DiscordColor.SpringGreen)
                            .WithTitle("Leaf Village — Raid Composition")
                            .WithDescription(string.Join("\n", isRetrieved.Item2))
                        );
                    await ctx.Client.SendMessageAsync(warroomChannel, embedMasteries);
                    await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
                        .WithContent($"Successfully retrieved masteries for those inside voice channel!"));

                } else
                {
                    await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
                        .WithContent($"Unable to retrieve masteries!"));
                }
            } else
            {
                await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
                    .WithContent($"Unable to use button command Retrieve Masteries for {ctx.Interaction.User.Username}, please check required roles."));
            }
        }

        [ButtonCommand("btn_VoiceChannel")]
        public async Task RaidVoiceChannel(ButtonContext ctx)
        {
            await ctx.Interaction.DeferAsync(true);

            var hasRole = ctx.Member.Roles.Any(x => x.Name == "Hokage" || x.Name == "Council" || x.Name == "Raid Leader");

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

                if(Members.Count == 0)
                {
                    await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
                        .WithContent($"There are no users in the Village Lobby voice channel!"));
                    return;
                }

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
                await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Successfully moved all users to the Village Raid!"));

            } else
            {
                await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
                    .WithContent($"Unable to use button command Voice Channel for {ctx.Interaction.User.Username}, please check required roles."));
            }
        }
    }
}

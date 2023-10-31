using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Net.Models;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leaf_Village_Bot.Commands.Hokage
{
    public class SlashCommands_Hokage : ApplicationCommandModule
    {
        [SlashCommand("VillageRaid_User", "Moves the user from the General VC to the Village Raid VC: @Hokage or @Raid Leader role required.")]
        [RequireRoles(RoleCheckMode.Any, "Hokage", "Raid Leader")]
        public async Task MoveUser_VC(InteractionContext ctx, [Option("IGN", "Character's in game name.")] DiscordUser User)
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

        [SlashCommand("VillageRaid_All", "Moves all users in the General VC to the Village Raid VC: @Hokage or @Raid Leader role required.")]
        public async Task MoveAll_VC(InteractionContext ctx)
        {
            await ctx.DeferAsync(true);

            var hasRole = ctx.Member.Roles.Any(x => x.Name == "Hokage" || x.Name == "Raid Leader");

            if (hasRole)
            {
                var guildChannels = await ctx.Guild.GetChannelsAsync();
                var generalChannel = guildChannels.FirstOrDefault(x => x.Name == "General");
                var raidChannel = guildChannels.FirstOrDefault(x => x.Name == "Village Raid");

                if (generalChannel == null)
                {
                    var embedFailed = new DiscordEmbedBuilder()
                    {
                        Title = "Village Raid Failed",
                        Color = DiscordColor.Red,
                        Description = "Channel: General does not exist!"
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

                var Members = generalChannel.Users.ToList();

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

using DSharpPlus.ButtonCommands;
using DSharpPlus.Entities;
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
        public async Task VillageRaid(ButtonContext ctx)
        {
            await ctx.Interaction.DeferAsync(true);

            var hasRole = ctx.Member.Roles.Any(x => x.Name == "Hokage" || x.Name == "Council");

            if (hasRole)
            {
                var DBUtil_Profile = new DBUtil_Profile();

                var guildChannels = await ctx.Guild.GetChannelsAsync();
                var raidChannel = guildChannels.FirstOrDefault(x => x.Name == "Village Raid");

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

                if (raidChannel.Users.Count() == 0)
                {
                    await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"There are no users in the Village Raid voice channel!"));
                    return;
                }

                var Members = raidChannel.Users.ToList();

                foreach (var member in Members)
                {
                    var isUpdated = await DBUtil_Profile.UpdateRaidAsync(member.Id);

                    if (!isUpdated)
                    {
                        await ctx.Channel.SendMessageAsync(new DiscordMessageBuilder().WithContent($"{member.Nickname} raid stat was not updated!"));
                    }
                }
                await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Successfully increased all Member's Raids stat!"));

            }
            else
            {
                await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Unable to delete application for {ctx.Interaction.User.Username}, please check required roles."));
            }
        }
    }
}

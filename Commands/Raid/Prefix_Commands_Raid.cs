using DSharpPlus.ButtonCommands;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus;
using DSharpPlus.ButtonCommands.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leaf_Village_Bot.Commands.Raid
{
    public class Prefix_Commands_Raid : BaseCommandModule
    {
        [Command("raid_dashboard")]
        [RequireRoles(RoleCheckMode.Any, "Administrator")]
        [Description("Leaf Raid Dashboard used to manage village raids.")]
        public async Task HokageDashboard(CommandContext ctx)
        {
            ButtonCommandsExtension buttonCommand = ctx.Client.GetButtonCommands();

            var embedDashboard = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.SpringGreen)
                    .WithTitle("Leaf Village Raid Dashboard")
                    .WithThumbnail(Global.LeafSymbol_URL)
                    .WithImageUrl(Global.MontoyaTM_URL)
                    .AddField("Raid++", "Raid++ increments the Raid stat for each user in the Village Raid voice channel.")
                    .AddField("Retrieve Masteries:", "Return a list of masteries of each user within the Village Raid voice channel to see composition.")
                )
                .AddComponents(new DiscordComponent[]
                {
                    new DiscordButtonComponent(ButtonStyle.Primary, buttonCommand.BuildButtonId("btn_VillageRaid"), "Raid++"),
                    new DiscordButtonComponent(ButtonStyle.Primary, buttonCommand.BuildButtonId("btn_RetrieveMasteries"), "Retrieve Masteries")
                });

            await ctx.Message.DeleteAsync();
            await ctx.Channel.SendMessageAsync(embedDashboard);
        }
    }
}

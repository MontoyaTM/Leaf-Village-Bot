using DSharpPlus.ButtonCommands;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.ButtonCommands.Extensions;
using DSharpPlus.Net.Models;

namespace Leaf_Village_Bot.Commands.Hokage
{
    public class PrefixCommands_Hokage : BaseCommandModule
    {
        [Command("hokage_dashboard")]
        [RequireRoles(RoleCheckMode.Any, "Administrator")]
        [Description("Leaf Hokage Dashboard used to manage village affairs.")]
        public async Task HokageDashboard(CommandContext ctx)
        {
            ButtonCommandsExtension buttonCommand = ctx.Client.GetButtonCommands();

            var embedDashboard = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.SpringGreen)
                    .WithTitle("Leaf Village Hokage Dashboard")
                    .WithThumbnail(Global.LeafSymbol_URL)
                    .WithImageUrl(Global.HokageNPC_URL)
                    .AddField("Delete Application:", "Enter the Member ID of the applicant you wish to delete. This will remove the data stored in the database of the user, allowing the individual to create a new villager application.")
                )
                .AddComponents(new DiscordComponent[]
                {
                    new DiscordButtonComponent(ButtonStyle.Primary, buttonCommand.BuildButtonId("btn_DeleteApplication"), "Delete Application"),
                    new DiscordButtonComponent(ButtonStyle.Primary, buttonCommand.BuildButtonId("btn_VillageRaid"), "++ Raid")
                });

            await ctx.Message.DeleteAsync();
            await ctx.Channel.SendMessageAsync(embedDashboard);
        }

        

    }
}

using DSharpPlus.ButtonCommands.Extensions;
using DSharpPlus.ButtonCommands;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leaf_Village_Bot.Commands.Ranked
{
    internal class PrefixCommands_Ranked : BaseCommandModule
    {
        [Command("ranked_rprequest")]
        [RequireRoles(RoleCheckMode.Any, "Administrator")]
        [Description("Displays an Embed for requesting an RP mission.")]
        public async Task RPRequest(CommandContext ctx)
        {
            ButtonCommandsExtension buttonCommand = ctx.Client.GetButtonCommands();

            var embedSupportForm = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.SpringGreen)
                    .WithTitle("Leaf Village RP Request")
                    .WithDescription(" ")
                    .WithImageUrl(Global.RankedNPC_URL)
                    .WithThumbnail(Global.LeafSymbol_URL)
                    .AddField("IGN:", "The in game name of the character that is requesting the rp mission.")
                    .AddField("RP Mission:", "The RP mission number you are requesting. You can only request one RP mission at a time! \n\n Ex. RP I, RP II, RP II, ...")
                    .AddField("Timezone:", "Timezone more suitable for completion.")
                    .AddField("Attendees:", "The person(s) who are going to be attending the RP mission. If you are missing members, a discussion channel will be created for players to reach out. " +
                                            "\n\nEx. IGN, Ninja2, Ninja3")
                    .WithFooter("A Ranked Ninjas will be reaching out to your request in the channel discussion created. Please be aware that RP missions are done in a 3 man team.")
                    )
                .AddComponents(new DiscordComponent[]
                {
                    new DiscordButtonComponent(ButtonStyle.Primary, buttonCommand.BuildButtonId("btn_CreateRequest"), "Create RP Request")
                });

            await ctx.Channel.SendMessageAsync(embedSupportForm);
            await ctx.Message.DeleteAsync();
        }

        [Command("ranked_dashboard")]
        [RequireRoles(RoleCheckMode.Any, "Administrator")]
        [Description("Displays an Embed for Ranked Ninjas for RP Requests.")]
        public async Task RankedDashboard(CommandContext ctx)
        {
            ButtonCommandsExtension buttonCommand = ctx.Client.GetButtonCommands();

            var embedRankedDashboard = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.SpringGreen)
                    .WithTitle("Leaf Ranked Ninja Dashboard")
                    .WithDescription(" ")
                    .WithThumbnail(Global.LeafSymbol_URL)
                    .WithImageUrl(Global.RankedNPC_URL)
                    .AddField("View RP Requests:", "Displays a list of active RP requests that need to be proctored by Leaf Ranked Ninjas.")
                    .AddField("Get RP Request:", "Displays a dropdown menu of active RP request and returns the selected ticket to be displayed")
                    .AddField("Delete RP Request:", "Displays a dropdown menu of active RP request and deletes the selected RP from active requests.")

                )
                .AddComponents(new DiscordComponent[]
                {
                    new DiscordButtonComponent(ButtonStyle.Primary, buttonCommand.BuildButtonId("btn_ViewRequest"), "View RP Requests"),
                    new DiscordButtonComponent(ButtonStyle.Primary, buttonCommand.BuildButtonId("btn_GetRequest"), "Get RP Requests"),
                    new DiscordButtonComponent(ButtonStyle.Primary, buttonCommand.BuildButtonId("btn_DeleteRequest"), "Delete RP Request"),
                });

            await ctx.Channel.SendMessageAsync(embedRankedDashboard);
            await ctx.Message.DeleteAsync();
        }
    }
}
